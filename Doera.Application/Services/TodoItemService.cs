using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Common;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces.Identity;
using Doera.Application.Interfaces.Services;
using Doera.Application.Specifications;
using Doera.Core.Entities;
using Doera.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Doera.Application.Services {
    public class TodoItemService(
            IUnitOfWork _uow,
            ICurrentUser _currentUser,
            IValidator<CreateTodoItemRequest> _createValidator,
            ILogger<TodoItemService> _logger
        ) : ITodoItemService {

        public async Task<Result<Guid>> CreateAsync(CreateTodoItemRequest request) {
            _logger.LogDebug("Create TodoItem requested for ListId={ListId}", request.TodoListId);

            var validation = await _createValidator.ValidateAsync(request);

            if (!validation.IsValid) {
                _logger.LogWarning("Create TodoItem validation failed for ListId={ListId}: {Errors}",
                    request.TodoListId,
                    string.Join("; ", validation.Errors.Select(e => $"{e.PropertyName}:{e.ErrorMessage}")));
                return validation.Errors.Select(e => Errors.Common.Validation(e.ErrorMessage)).ToList();
            }

            var userId = _currentUser.RequireUserId();

            var todoList = await _uow.TodoLists.FindByIdAsync(request.TodoListId);
            if (todoList is null) {
                _logger.LogWarning("Create TodoItem failed: list not found ListId={ListId} UserId={UserId}", request.TodoListId, userId);
                return Errors.TodoList.NotFound();
            }
            if (todoList.UserId != userId) {
                _logger.LogWarning("Create TodoItem access denied ListId={ListId} UserId={UserId}", request.TodoListId, userId);
                return Errors.Common.AccessDenied();
            }

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

            if (result == 0) {
                _logger.LogError("Create TodoItem persistence failed ListId={ListId} UserId={UserId}", request.TodoListId, userId);
                return Errors.TodoItem.CreateFailed();
            }

            _logger.LogInformation("Created TodoItem Id={ItemId} ListId={ListId} UserId={UserId}", todoItem.Id, request.TodoListId, userId);
            return todoItem.Id;
        }

        public async Task<Result> UpdateAsync(UpdateTodoItemRequest request) {
            _logger.LogDebug("Update TodoItem requested Id={ItemId}", request.Id);

            var userId = _currentUser.RequireUserId();
            var modifyTags = request.TagNames is not null;
            var spec = new TodoItemSpecification(request.Id, userId, includeTags: modifyTags);

            var todoItem = await _uow.TodoItems.FindAsync(spec);
            if (todoItem is null) {
                _logger.LogWarning("Update TodoItem failed: not found Id={ItemId} UserId={UserId}", request.Id, userId);
                return Errors.TodoItem.NotFound();
            }

            todoItem.Title = request.Title ?? todoItem.Title;
            todoItem.Description = request.Description ?? todoItem.Description;
            todoItem.Status = request.Status ?? todoItem.Status;
            todoItem.Priority = request.Priority ?? todoItem.Priority;
            todoItem.StartDate = request.StartDate ?? todoItem.StartDate;
            todoItem.DueDate = request.DueDate ?? todoItem.DueDate;

            if (modifyTags) {
                _logger.LogDebug("Updating tags for TodoItem Id={ItemId}", request.Id);
                await UpdateTagsAsync(todoItem, request.TagNames!);
            }

            todoItem.UpdatedAt = DateTimeOffset.UtcNow;

            try {
                await _uow.ExecuteTransactionAsync(
                    Try: async () => {
                        await _uow.CompleteAsync();
                        await _uow.Tags.ExecuteDeleteUnusedTagsAsync();
                    }
                );
                _logger.LogInformation("Updated TodoItem Id={ItemId} UserId={UserId}", request.Id, userId);
                return Result.Success();
            } catch (Exception ex) {
                _logger.LogError(ex, "Update TodoItem failed Id={ItemId} UserId={UserId}", request.Id, userId);
                return Errors.TodoItem.UpdateFailed();
            }
        }

        public async Task<Result> DeleteAsync(Guid todoItemId) {
            _logger.LogDebug("Delete TodoItem requested Id={ItemId}", todoItemId);

            var userId = _currentUser.RequireUserId();
            var todoItem = await _uow.TodoItems.FindByIdAsync(todoItemId);

            if (todoItem is null) {
                _logger.LogWarning("Delete TodoItem failed: not found Id={ItemId} UserId={UserId}", todoItemId, userId);
                return Errors.TodoItem.NotFound();
            }
            if (todoItem.UserId != userId) {
                _logger.LogWarning("Delete TodoItem access denied Id={ItemId} UserId={UserId}", todoItemId, userId);
                return Errors.Common.AccessDenied();
            }

            try {
                await _uow.ExecuteTransactionAsync(async () => {
                    _uow.TodoItems.Remove(todoItem);
                    await _uow.CompleteAsync();
                    await _uow.Tags.ExecuteDeleteUnusedTagsAsync();
                });
                _logger.LogInformation("Deleted TodoItem Id={ItemId} UserId={UserId}", todoItemId, userId);
                return Result.Success();
            } catch (Exception ex) {
                _logger.LogError(ex, "Delete TodoItem failed Id={ItemId} UserId={UserId}", todoItemId, userId);
                return Errors.TodoItem.DeleteFailed();
            }
        }

        public async Task<Result> ReorderAsync(ReorderItemRequest request) {
            _logger.LogDebug("Reorder TodoItems requested ListId={ListId} Count={Count}",
                request.TodoListId, request.ReorderRequests.Count());

            var userId = _currentUser.RequireUserId();
            var requestIds = request.ReorderRequests.Select(r => r.Id).ToList();

            if (requestIds.Count != requestIds.Distinct().Count()) {
                _logger.LogWarning("Reorder TodoItems failed: duplicate IDs ListId={ListId}", request.TodoListId);
                return Errors.Common.Validation("Duplicate Todo Item IDs in request.");
            }

            var todoItems = await _uow.TodoItems.GetAllForUserAsync(userId, request.TodoListId, requestIds);
            if (todoItems.Count() != requestIds.Count) {
                _logger.LogWarning("Reorder TodoItems failed: ownership mismatch ListId={ListId}", request.TodoListId);
                return Errors.Common.Validation("One or more item ids are invalid or not owned by the current user.");
            }

            try {
                await _uow.ExecuteTransactionAsync(
                    Try: async () => {
                        var dict = todoItems.ToDictionary(i => i.Id, i => i);
                        foreach (var r in request.ReorderRequests)
                            dict[r.Id].Order = r.NewOrder;
                        await _uow.CompleteAsync();
                    }
                );
                _logger.LogInformation("Reordered {Count} TodoItems ListId={ListId} UserId={UserId}",
                    request.ReorderRequests.Count(), request.TodoListId, userId);
                return Result.Success();
            } catch (Exception ex) {
                _logger.LogError(ex, "Reorder TodoItems failed ListId={ListId} UserId={UserId}", request.TodoListId, userId);
                return Errors.Common.Failed("Could not reorder items.");
            }
        }

        public async Task<Result> UpdateStatusAsync(UpdateTodoItemStatusRequest request) {
            _logger.LogDebug("Update status requested Id={ItemId} NewStatus={Status}", request.Id, request.Status);

            var userId = _currentUser.RequireUserId();
            var affected = await _uow.TodoItems.ExecuteUpdateStatusAsync(request.Id, userId, request.Status);

            if (affected == 0) {
                _logger.LogWarning("Update status failed (no rows) Id={ItemId} UserId={UserId}", request.Id, userId);
                return Errors.TodoItem.UpdateFailed();
            }

            _logger.LogInformation("Updated status for TodoItem Id={ItemId} Status={Status} UserId={UserId}",
                request.Id, request.Status, userId);
            return Result.Success();
        }

        // Helper Methods 
        private async Task UpdateTagsAsync(TodoItem item, IEnumerable<string> tagNames) {
            var resolvedTags = await _uow.Tags.ResolveTagsAsync(tagNames);

            if (resolvedTags.Count == 0) {
                item.TodoItemTags.Clear();
                return;
            }

            // Should have used NormalizedName
            // Resolved tags can return ItemTags that don't have a valid Id yet...
            var desiredTagIds = resolvedTags.Select(l => l.Tag!.Id).ToHashSet();
            var currentTagIds = item.TodoItemTags.Select(l => l.TagId).ToHashSet();

            if (desiredTagIds.SetEquals(currentTagIds)) return;

            var toRemove = item.TodoItemTags.Where(l => !desiredTagIds.Contains(l.TagId)).ToList();
            foreach (var tag in toRemove)
                item.TodoItemTags.Remove(tag);

            var toAdd = resolvedTags.Where(l => !currentTagIds.Contains(l.Tag!.Id)).ToList();
            foreach (var tag in toAdd)
                item.TodoItemTags.Add(tag);
        }
    }
}