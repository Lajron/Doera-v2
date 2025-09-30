using Doera.Core.Enums;
using Doera.Web.Features.Tags.ViewModels;
using System;
using System.Collections.Generic;

namespace Doera.Web.Features.TodoItem.ViewModels {
    public record TodoItemVM {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int Order { get; init; }
        public TodoStatus? Status { get; init; }
        public TodoPriority? Priority { get; init; }
        public DateTimeOffset? StartDate { get; init; }
        public DateTimeOffset? DueDate { get; init; }
        public DateTimeOffset? ArchivedAt { get; init; }
        public IEnumerable<TagVM> Tags { get; init; } = [];
    }
}