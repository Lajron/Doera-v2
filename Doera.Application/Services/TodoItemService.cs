using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoItem.Requests;
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

            if (todoList is null)
                return Errors.TodoList.NotFound();

            if (todoList.UserId != userId)
                return Errors.Common.AccessDenied();

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
                DueDate = request.DueDate,
                TodoItemTags = await uof.Tags.ResolveTagsAsync(request.TagNames)
            };

            await uof.TodoItems.AddAsync(todoItem);
            var result = await uof.CompleteAsync();

            if (result <= 0) {
                return Errors.TodoItem.CreateFailed();
            }

            return todoItem.Id;
        }

        public async Task<Result> UpdateAsync(UpdateTodoItemRequest request) {
            var userId = _currentUser.RequireUserId();
            return Result.Success();
            //var userId = _currentUser.RequireUserId();
            //var todoItem = await uof.TodoItems.FindByIdAsync(request.Id);
            //if (todoItem is null)
            //    return new Error("NotFound", "Todo Item doesn't exist");
            //if (todoItem.UserId != userId)
            //    return new Error("Access Denied", "You don't have permission to update this item");
            //todoItem.Title = request.Title ?? todoItem.Title;
            //todoItem.Description = request.Description ?? todoItem.Description;
            //todoItem.Status = request.Status ?? todoItem.Status;
            //todoItem.Priority = request.Priority ?? todoItem.Priority;
            //todoItem.StartDate = request.StartDate ?? todoItem.StartDate;
            //todoItem.DueDate = request.DueDate ?? todoItem.DueDate;
            //todoItem.ArchivedAt = request.ArchivedAt ?? todoItem.ArchivedAt;
            //todoItem.UpdatedAt = DateTimeOffset.UtcNow;
            //if (request.TagNames is not null) {
            //    todoItem.TodoItemTags = await uof.Tags.ResolveTagsAsync(request.TagNames);
            //}
            //uof.TodoItems.Update(todoItem);
            //var result = await uof.CompleteAsync();
            //if (result <= 0) {
            //    return new Error("NoUpdate", "An error occurred while updating the todo item");
            //}
            //return Result.Success();
        }
    }
}