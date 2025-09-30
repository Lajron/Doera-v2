using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Application.Interfaces;
using Doera.Application.Interfaces.Identity;
using Doera.Application.Interfaces.Services;
using Doera.Core.Entities;
using Doera.Core.Interfaces;

namespace Doera.Application.Services {
    public class TodoItemService(
            IUnitOfWork uof,
            ICurrentUser _currentUser
        ) : ITodoItemService {
        public async Task<Result<Guid>> CreateAsync(CreateTodoItemRequest request) {
            var userId = _currentUser.RequireUserId();

            var todoList = await uof.TodoLists.FindByIdAsync(request.TodoListId);

            if (todoList is null) {
                return Result<Guid>.Failure(new Error("NotFound", "Todo List doesn't exist"));
            }

            if (todoList.UserId != userId) {
                return Result<Guid>.Failure(new Error("Access Denied", "You don't have permission to add items to this list"));
            }

            var itemOrder = await uof.TodoItems.GetCountForListAsync(request.TodoListId);

            var todoItem = new TodoItem {
                TodoListId = request.TodoListId,
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                Order = itemOrder,
                Status = request.Status,
                Priority = request.Priority,
                StartDate = request.StartDate,
                DueDate = request.DueDate
            };

            todoItem.TodoItemTags = await uof.Tags.ResolveTagsAsync(request.TagNames);

            await uof.TodoItems.AddAsync(todoItem);
            var result = await uof.CompleteAsync();

            if (result <= 0) {
                return Result<Guid>.Failure(new Error("NoInsert", "An error occurred while creating the todo item"));
            }

            return todoItem.Id;
        }
    }
}