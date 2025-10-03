using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.Common;
using Doera.Application.DTOs.TodoList.Requests;
using Doera.Application.Interfaces.Identity;
using Doera.Application.Interfaces.Services;
using Doera.Core.Entities;
using Doera.Core.Interfaces;
using Doera.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doera.Application.Services {
    public class TodoListService(
        IUnitOfWork _uow,
        ICurrentUser _currentUser
    ) : ITodoListService {
        public async Task<Result<Guid>> CreateAsync(CreateTodoListRequest request) {
            var userId = _currentUser.RequireUserId();

            var listOrder = await _uow.TodoLists.GetCountForUserAsync(userId);

            var todoList = new TodoList {
                Name = request.Name,
                Order = listOrder,
                UserId = userId
            };

            await _uow.TodoLists.AddAsync(todoList);
            await _uow.CompleteAsync();

            return todoList.Id;
        }

        public async Task<Result> UpdateAsync(UpdateTodoListRequest request) {
            var userId = _currentUser.RequireUserId();

            var todoList = await _uow.TodoLists.FindByIdAsync(request.Id);

            if (todoList is null) return Errors.TodoList.NotFound();

            if (todoList.UserId != userId) return Errors.Common.AccessDenied();

            todoList.Name = request.Name ?? todoList.Name;

            await _uow.CompleteAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id) {
            var userId = _currentUser.RequireUserId();

            var todoList = await _uow.TodoLists.FindByIdAsync(id);
            if (todoList is null) return Errors.TodoList.NotFound();
            if (todoList.UserId != userId) return Errors.Common.AccessDenied();

            var count = await _uow.TodoLists.GetCountForUserAsync(userId);
            if (count == 1) return Errors.Common.Failed("You must have at least one todo list.");

            try {
                await _uow.ExecuteTransactionAsync(async () => {
                    await _uow.TodoItems.ExecuteDeleteByListAsync(id);

                    _uow.TodoLists.Remove(todoList);
                    await _uow.CompleteAsync();

                    await _uow.Tags.ExecuteDeleteUnusedTagsAsync();
                });
                return Result.Success();
            } catch {
                return Errors.TodoList.DeleteFailed();
            }
        }

        public async Task<Result> ReorderAsync(IEnumerable<ReorderRequest> request) {
            var userId = _currentUser.RequireUserId();

            var requestIds = request.Select(r => r.Id).ToList();

            if (requestIds.Count != requestIds.Distinct().Count())
                return Errors.Common.Validation("Duplicate Todo List IDs in request.");

            var todoLists = await _uow.TodoLists.GetAllForUserAsync(userId, requestIds);

            if (todoLists.Count() != requestIds.Count) 
                return Errors.Common.Validation("One or more list ids are invalid or not owned by the current user.");

            try {
                await _uow.ExecuteTransactionAsync(
                    Try: async () => {
                        var todoListDict = todoLists.ToDictionary(l => l.Id, l => l);

                        foreach (var reorder in request) {
                            todoListDict[reorder.Id].Order = reorder.NewOrder;
                        }

                        await _uow.CompleteAsync();
                    }
                );
                return Result.Success();
            } catch {
                return Errors.Common.Failed("Could not reorder lists.");
            }
        }


    }
}

    
