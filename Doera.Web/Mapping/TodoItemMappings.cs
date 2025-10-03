using Doera.Application.DTOs.TodoItem;
using Doera.Application.DTOs.TodoItem.Requests;
using Doera.Web.Features.TodoItem.ViewModels;
using System;
using System.Linq;

namespace Doera.Web.Mapping {
    public static class TodoItemMappings {
        public static EditTodoItemVM ToEditVM(this TodoItemDto dto) => new() {
            Id = dto.Id,
            TodoListId = dto.TodoListId,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status ?? Core.Enums.TodoStatus.None,
            Priority = dto.Priority ?? Core.Enums.TodoPriority.None,
            StartDate = dto.StartDate,
            DueDate = dto.DueDate,
            TagNames = dto.Tags is null ? null : string.Join(", ", dto.Tags.Select(t => t.DisplayName))
        };

        public static UpdateTodoItemRequest ToUpdateDto(this EditTodoItemVM vm) => new() {
            Id = vm.Id,
            Title = vm.Title,
            Description = vm.Description,
            Status = vm.Status,
            Priority = vm.Priority,
            StartDate = vm.StartDate,
            DueDate = vm.DueDate,
            TagNames = string.IsNullOrWhiteSpace(vm.TagNames)
                ? []
                : vm.TagNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        };
    }
}