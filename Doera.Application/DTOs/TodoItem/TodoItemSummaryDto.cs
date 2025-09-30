using Doera.Application.DTOs.Tags;
using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.TodoItem {
    public record TodoItemSummaryDto {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int Order { get; init; }
        public TodoStatus Status { get; init; }
        public TodoPriority Priority { get; init; }
        public DateTimeOffset? StartDate { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public bool IsArchived { get; init; } = false;
        public IEnumerable<TagDto> Tags { get; init; } = [];
    }
}
