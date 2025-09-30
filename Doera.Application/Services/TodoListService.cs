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
                Name = request.Title,
                Order = listOrder,
                UserId = userId
            };

            await _uof.TodoLists.AddAsync(todoList);
            await _uof.CompleteAsync();

            return todoList.Id;
        }
    }
}