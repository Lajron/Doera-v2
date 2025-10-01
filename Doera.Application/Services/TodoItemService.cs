using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces.Identity;
using Doera.Application.Interfaces.Services;
using Doera.Application.Specifications;
using Doera.Core.Entities;
using Doera.Core.Interfaces;

namespace Doera.Application.Services {
    public class TodoItemService(
            IUnitOfWork _uof,
            ICurrentUser _currentUser
        ) : ITodoItemService {
        public async Task<Result<Guid>> CreateAsync(CreateTodoItemRequest request) {
            var userId = _currentUser.RequireUserId();

            var todoList = await _uof.TodoLists.FindByIdAsync(request.TodoListId);

            if (todoList is null) return Errors.TodoList.NotFound();

            if (todoList.UserId != userId) return Errors.Common.AccessDenied();

            var itemOrder = await _uof.TodoItems.GetCountForListAsync(request.TodoListId);

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
                TodoItemTags = await _uof.Tags.ResolveTagsAsync(request.TagNames)
            };

            await _uof.TodoItems.AddAsync(todoItem);
            var result = await _uof.CompleteAsync();

            if (result <= 0) return Errors.TodoItem.CreateFailed();

            return todoItem.Id;
        }

        public async Task<Result> UpdateAsync(UpdateTodoItemRequest request) {
            var userId = _currentUser.RequireUserId();

            var modifyTags = request.TagNames is not null;

            var spec = new TodoItemSpecification(request.Id, userId, includeTags: modifyTags);

            var todoItem = await _uof.TodoItems.FindAsync(spec);

            if (todoItem is null) return Errors.TodoItem.NotFound();

            todoItem.Title = request.Title ?? todoItem.Title;
            todoItem.Description = request.Description ?? todoItem.Description;
            todoItem.Status = request.Status ?? todoItem.Status;
            todoItem.Priority = request.Priority ?? todoItem.Priority;
            todoItem.StartDate = request.StartDate ?? todoItem.StartDate;
            todoItem.DueDate = request.DueDate ?? todoItem.DueDate;

            if (modifyTags) await UpdateTagsAsync(todoItem, request.TagNames!);

            todoItem.UpdatedAt = DateTimeOffset.UtcNow;

            await _uof.CompleteAsync();

            await _uof.Tags.CleanupOrphanedTagsAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid todoItemId) {
            var userId = _currentUser.RequireUserId();

            var todoItem = await _uof.TodoItems.FindByIdAsync(todoItemId);

            if (todoItem is null) return Errors.TodoItem.NotFound();

            if (todoItem.UserId != userId) return Errors.Common.AccessDenied();

            _uof.TodoItems.Remove(todoItem);

            await _uof.CompleteAsync();

            await _uof.Tags.CleanupOrphanedTagsAsync();

            return Result.Success();
        }



        // Helper Methods 
        private async Task UpdateTagsAsync(TodoItem item, IEnumerable<string> tagNames) {
            if (tagNames.Any() is false) {
                item.TodoItemTags.Clear();
                return;
            }

            var resolvedTags = await _uof.Tags.ResolveTagsAsync(tagNames);

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