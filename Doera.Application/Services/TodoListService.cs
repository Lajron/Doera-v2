using Doera.Application.Abstractions.Results;
using Doera.Application.Caching;
using Doera.Application.DTOs.Common;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces.Caching;
using Doera.Application.Interfaces.Identity;
using Doera.Application.Interfaces.Services;
using Doera.Core.Entities;
using Doera.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Doera.Application.Services {
    public class TodoListService(
        IUnitOfWork _uow,
        ICurrentUser _currentUser,
        IValidator<CreateTodoListRequest> _createValidator,
        ICacheService _cache,
        ILogger<TodoListService> _logger
    ) : ITodoListService {

        public async Task<Result<Guid>> CreateAsync(CreateTodoListRequest request) {
            _logger.LogDebug("Create TodoList requested: Name='{Name}'", request.Name);

            var validation = await _createValidator.ValidateAsync(request);
            if (!validation.IsValid) {
                _logger.LogWarning("Create TodoList validation failed: {Errors}",
                    string.Join("; ", validation.Errors.Select(e => $"{e.PropertyName}:{e.ErrorMessage}")));

                return validation.Errors
                    .Select(e => Errors.Common.Validation($"{e.PropertyName}: {e.ErrorMessage}"))
                    .ToList();
            }

            var userId = _currentUser.RequireUserId();

            var order = await _uow.TodoLists.GetHeighestListOrderAsync(userId);

            var list = new TodoList {
                Name = request.Name,
                Order = order,
                UserId = userId
            };

            await _uow.TodoLists.AddAsync(list);
            await _uow.CompleteAsync();

            _cache.InvalidateTodoLists(_currentUser);
            _logger.LogInformation("Created TodoList Id={ListId} Order={Order} UserId={UserId}", list.Id, list.Order, userId);

            return list.Id;
        }

        public async Task<Result> UpdateAsync(UpdateTodoListRequest request) {
            _logger.LogDebug("Update TodoList requested Id={ListId}", request.Id);

            var userId = _currentUser.RequireUserId();
            var list = await _uow.TodoLists.FindByIdAsync(request.Id);

            if (list is null) {
                _logger.LogWarning("Update TodoList failed (not found) Id={ListId} UserId={UserId}", request.Id, userId);
                return Errors.TodoList.NotFound();
            }
            if (list.UserId != userId) {
                _logger.LogWarning("Update TodoList access denied Id={ListId} UserId={UserId}", request.Id, userId);
                return Errors.Common.AccessDenied();
            }

            if (request.Name is not null && request.Name != list.Name) {
                list.Name = request.Name;
            }

            await _uow.CompleteAsync();
            _cache.InvalidateTodoLists(_currentUser);

            _logger.LogInformation("Updated TodoList Id={ListId} UserId={UserId}", request.Id, userId);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id) {
            _logger.LogDebug("Delete TodoList requested Id={ListId}", id);

            var userId = _currentUser.RequireUserId();
            var list = await _uow.TodoLists.FindByIdAsync(id);

            if (list is null) {
                _logger.LogWarning("Delete TodoList failed (not found) Id={ListId} UserId={UserId}", id, userId);
                return Errors.TodoList.NotFound();
            }
            if (list.UserId != userId) {
                _logger.LogWarning("Delete TodoList access denied Id={ListId} UserId={UserId}", id, userId);
                return Errors.Common.AccessDenied();
            }

            var count = await _uow.TodoLists.GetCountForUserAsync(userId);
            if (count == 1) {
                _logger.LogWarning("Delete TodoList blocked (last list) Id={ListId} UserId={UserId}", id, userId);
                return Errors.Common.Failed("You must have at least one todo list.");
            }

            try {
                await _uow.ExecuteTransactionAsync(async () => {
                    await _uow.TodoItems.ExecuteDeleteByListAsync(id);
                    _uow.TodoLists.Remove(list);
                    await _uow.CompleteAsync();
                    await _uow.Tags.ExecuteDeleteUnusedTagsAsync();
                });

                _cache.InvalidateTodoLists(_currentUser);
                _logger.LogInformation("Deleted TodoList Id={ListId} UserId={UserId}", id, userId);

                return Result.Success();
            } catch (Exception ex) {
                _logger.LogError(ex, "Delete TodoList failed Id={ListId} UserId={UserId}", id, userId);
                return Errors.TodoList.DeleteFailed();
            }
        }

        public async Task<Result> ReorderAsync(IEnumerable<ReorderRequest> request) {
            var reorderList = request.ToList();
            _logger.LogDebug("Reorder TodoLists requested Count={Count}", reorderList.Count);

            var userId = _currentUser.RequireUserId();
            var ids = reorderList.Select(r => r.Id).ToList();

            if (ids.Count != ids.Distinct().Count()) {
                _logger.LogWarning("Reorder TodoLists failed: duplicate IDs UserId={UserId}", userId);
                return Errors.Common.Validation("Duplicate Todo List IDs in request.");
            }

            var existing = await _uow.TodoLists.GetAllForUserAsync(userId, ids);
            if (existing.Count() != ids.Count) {
                _logger.LogWarning("Reorder TodoLists failed: ownership mismatch UserId={UserId}", userId);
                return Errors.Common.Validation("One or more list ids are invalid or not owned by the current user.");
            }

            try {
                await _uow.ExecuteTransactionAsync(
                    Try: async () => {
                        var dict = existing.ToDictionary(l => l.Id, l => l);
                        foreach (var r in reorderList) {
                            dict[r.Id].Order = r.NewOrder;
                        }
                        await _uow.CompleteAsync();
                    }
                );

                _cache.InvalidateTodoLists(_currentUser);
                _logger.LogInformation("Reordered {Count} TodoLists UserId={UserId}", reorderList.Count, userId);

                return Result.Success();
            } catch (Exception ex) {
                _logger.LogError(ex, "Reorder TodoLists failed UserId={UserId}", userId);
                return Errors.Common.Failed("Could not reorder lists.");
            }
        }
    }
}
