using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.TodoItem.Requests {
    public record CreateTodoItemRequest {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int Order { get; init; } = 0;

        public TodoStatus Status { get; init; } = TodoStatus.None;
        public TodoPriority Priority { get; init; } = TodoPriority.None;
        public DateTimeOffset? StartDate { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public Guid TodoListId { get; init; }

        public IEnumerable<string> TagNames { get; init; } = [];

    }
}
