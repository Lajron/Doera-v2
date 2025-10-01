using Doera.Application.Abstractions.Results;
using Doera.Application.DTOs.TodoItem.Requests;

namespace Doera.Application.Interfaces.Services {
    public interface ITodoItemService {
        Task<Result<Guid>> CreateAsync(CreateTodoItemRequest request);
        public Task<Result> UpdateAsync(UpdateTodoItemRequest request);

        public Task<Result> DeleteAsync(Guid todoItemId);
    }
}