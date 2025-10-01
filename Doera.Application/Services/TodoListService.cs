using Doera.Application.Abstractions.Results;
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
        IUnitOfWork _uof,
        ICurrentUser _currentUser
    ) : ITodoListService {
        public async Task<Result<Guid>> CreateAsync(CreateTodoListRequest request) {
            var userId = _currentUser.RequireUserId();

            var listOrder = await _uof.TodoLists.GetCountForUserAsync(userId);

            var todoList = new TodoList {
                Name = request.Name,
                Order = listOrder,
                UserId = userId
            };

            await _uof.TodoLists.AddAsync(todoList);
            await _uof.CompleteAsync();

            return todoList.Id;
        }

        public async Task<Result> UpdateAsync(UpdateTodoListRequest request) {
            var userId = _currentUser.RequireUserId();

            var todoList = await _uof.TodoLists.FindByIdAsync(request.Id);

            if (todoList is null) return Errors.TodoList.NotFound();

            if (todoList.UserId != userId) return Errors.Common.AccessDenied();

            todoList.Name = request.Name ?? todoList.Name;

            await _uof.CompleteAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id) {
            var userId = _currentUser.RequireUserId();

            var todoList = await _uof.TodoLists.FindByIdAsync(id);

            if (todoList is null) return Errors.TodoList.NotFound();

            if (todoList.UserId != userId) return Errors.Common.AccessDenied();

            _uof.TodoLists.Remove(todoList);

            await _uof.CompleteAsync();

            await _uof.Tags.CleanupOrphanedTagsAsync();

            return Result.Success();
        }


    }
}

    
