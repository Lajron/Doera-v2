
using Doera.Core.Enums;

namespace Doera.Application.DTOs.TodoItem.Requests {
    public record UpdateTodoItemRequest {
        public required Guid Id { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public TodoStatus? Status { get; init; }
        public TodoPriority? Priority { get; init; }
        public DateTimeOffset? StartDate { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public IEnumerable<string>? TagNames { get; init; }

    }
}
