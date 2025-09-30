using Doera.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Doera.Web.Features.TodoItem.ViewModels {
    public record EditTodoItemVM {
        [Required]
        public Guid Id { get; init; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; init; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; init; }

        [EnumDataType(typeof(TodoStatus))]
        public TodoStatus Status { get; init; } = TodoStatus.None;

        [EnumDataType(typeof(TodoPriority))]
        public TodoPriority Priority { get; init; } = TodoPriority.None;

        [DataType(DataType.DateTime)]
        public DateTimeOffset? StartDate { get; init; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset? DueDate { get; init; }

        [Required]
        public Guid TodoListId { get; init; }

        [Display(Name = "Tags (comma-separated)")]
        public string? TagNames { get; init; }
    }
}