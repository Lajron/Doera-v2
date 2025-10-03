using Doera.Core.Enums;
using System;
using System.Collections.Generic;

namespace Doera.Application.DTOs.TodoItem {
    public record TodoItemFilter {
        public Guid? TodoListId { get; init; }
        public TodoStatus? Status { get; init; }
        public TodoPriority? Priority { get; init; }
        public bool? IsArchived { get; init; }
        public DateTimeOffset? DueAfter { get; init; }
        public DateTimeOffset? DueBefore { get; init; }
        public IEnumerable<Guid>? TagIds { get; init; }
        public string? Search { get; init; }
        public TodoSort SortBy { get; init; } = TodoSort.Order;
        public SortDirection Direction { get; init; } = SortDirection.Asc;
    }
}