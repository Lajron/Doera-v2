using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Common;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces.Identity;
using Doera.Application.Interfaces.Services;
using Doera.Application.Specifications;
using Doera.Core.Entities;
using Doera.Core.Interfaces;

namespace Doera.Application.Services {
    public class TodoItemService(
            IUnitOfWork _uow,
            ICurrentUser _currentUser
        ) : ITodoItemService {
        public async Task<Result<Guid>> CreateAsync(CreateTodoItemRequest request) {
            var userId = _currentUser.RequireUserId();

            var todoList = await _uow.TodoLists.FindByIdAsync(request.TodoListId);

            if (todoList is null) return Errors.TodoList.NotFound();

            if (todoList.UserId != userId) return Errors.Common.AccessDenied();

            var itemOrder = await _uow.TodoItems.GetCountForListAsync(request.TodoListId);

            var todoItem = new TodoItem {
                TodoListId = request.TodoListId,
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                Order = itemOrder,
                Status = request.Status,
                Priority = request.Priority,
                StartDate = request.StartDate,
                DueDate = request.DueDate,
                TodoItemTags = await _uow.Tags.ResolveTagsAsync(request.TagNames)
            };

            await _uow.TodoItems.AddAsync(todoItem);
            var result = await _uow.CompleteAsync();

            if (result == 0) return Errors.TodoItem.CreateFailed();

            return todoItem.Id;
        }

        public async Task<Result> UpdateAsync(UpdateTodoItemRequest request) {
            var userId = _currentUser.RequireUserId();

            var modifyTags = request.TagNames is not null;

            var spec = new TodoItemSpecification(request.Id, userId, includeTags: modifyTags);

            var todoItem = await _uow.TodoItems.FindAsync(spec);

            if (todoItem is null) return Errors.TodoItem.NotFound();

            todoItem.Title = request.Title ?? todoItem.Title;
            todoItem.Description = request.Description ?? todoItem.Description;
            todoItem.Status = request.Status ?? todoItem.Status;
            todoItem.Priority = request.Priority ?? todoItem.Priority;
            todoItem.StartDate = request.StartDate ?? todoItem.StartDate;
            todoItem.DueDate = request.DueDate ?? todoItem.DueDate;

            if (modifyTags) await UpdateTagsAsync(todoItem, request.TagNames!);

            todoItem.UpdatedAt = DateTimeOffset.UtcNow;

            try {
                await _uow.ExecuteTransactionAsync(
                    Try: async () => {
                        await _uow.CompleteAsync();
                        await _uow.Tags.ExecuteDeleteUnusedTagsAsync();
                    }
                );
                return Result.Success();
            } catch {
                return Errors.TodoItem.UpdateFailed();
            }
        }

        public async Task<Result> DeleteAsync(Guid todoItemId) {
            var userId = _currentUser.RequireUserId();

            var todoItem = await _uow.TodoItems.FindByIdAsync(todoItemId);

            if (todoItem is null) return Errors.TodoItem.NotFound();

            if (todoItem.UserId != userId) return Errors.Common.AccessDenied();

            _uow.TodoItems.Remove(todoItem);

            await _uow.CompleteAsync();

            try {
                await _uow.ExecuteTransactionAsync(async () => {
                    _uow.TodoItems.Remove(todoItem);     
                    await _uow.CompleteAsync();           
                    await _uow.Tags.ExecuteDeleteUnusedTagsAsync(); 
                });
                return Result.Success();
            } catch {
                return Errors.TodoItem.DeleteFailed();
            }
        }

        public async Task<Result> ReorderAsync(ReorderItemRequest request) {
            var userId = _currentUser.RequireUserId();
            var requestIds = request.ReorderRequests.Select(r => r.Id).ToList();

            if (requestIds.Count != requestIds.Distinct().Count())
                return Errors.Common.Validation("Duplicate Todo Item IDs in request.");

            var todoItems = await _uow.TodoItems.GetAllForUserAsync(userId, request.TodoListId, requestIds);

            if (todoItems.Count() != requestIds.Count)
                return Errors.Common.Validation("One or more item ids are invalid or not owned by the current user.");

            try {
                await _uow.ExecuteTransactionAsync(
                    Try: async () => {
                        var todoItemDict = todoItems.ToDictionary(i => i.Id, i => i);

                        foreach (var reorder in request.ReorderRequests) {
                            todoItemDict[reorder.Id].Order = reorder.NewOrder;
                        }

                        await _uow.CompleteAsync();
                    }
                );
                return Result.Success();
            } catch {
                return Errors.Common.Failed("Could not reorder items.");
            }
        }

        public async Task<Result> UpdateStatusAsync(UpdateTodoItemStatusRequest request) {
            var userId = _currentUser.RequireUserId();

            var result = await _uow.TodoItems.ExecuteUpdateStatusAsync(request.Id, userId, request.Status);

            if (result == 0) return Errors.TodoItem.UpdateFailed();

            return Result.Success();
        }



        // Helper Methods 
        private async Task UpdateTagsAsync(TodoItem item, IEnumerable<string> tagNames) {
            var resolvedTags = await _uow.Tags.ResolveTagsAsync(tagNames);

            if (resolvedTags.Count == 0) {
                item.TodoItemTags.Clear();
                return;
            }

            var desiredTagIds = resolvedTags.Select(l => l.Tag!.Id).ToHashSet();
            var currentTagIds = item.TodoItemTags.Select(l => l.TagId).ToHashSet();

            if (desiredTagIds.SetEquals(currentTagIds)) return;

            var toRemoveTags = item.TodoItemTags.Where(l => !desiredTagIds.Contains(l.TagId)).ToList();
            foreach (var tag in toRemoveTags)
                item.TodoItemTags.Remove(tag);

            var toAddTags = resolvedTags.Where(l => !currentTagIds.Contains(l.Tag!.Id)).ToList();
            foreach (var tag in toAddTags)
                item.TodoItemTags.Add(tag);
        }

    }
}