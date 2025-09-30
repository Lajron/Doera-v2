using System;

namespace Doera.Application.DTOs.TodoList {
    public record TodoListSummaryDto {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Order { get; init; }
        public int TotalItems { get; init; }
    }
}