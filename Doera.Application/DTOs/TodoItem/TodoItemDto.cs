using Doera.Application.DTOs.Tags;
using Doera.Core.Enums;
using System;
using System.Collections.Generic;

namespace Doera.Application.DTOs.TodoItem {
    public record TodoItemDto {
        public Guid Id { get; init; }
        public Guid TodoListId { get; init; }    
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int Order { get; init; }
        public TodoStatus? Status { get; init; }
        public TodoPriority? Priority { get; init; }
        public DateTimeOffset? StartDate { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public DateTimeOffset? ArchivedAt { get; init; }
        public IEnumerable<TagDto> Tags { get; init; } = [];
    }
}