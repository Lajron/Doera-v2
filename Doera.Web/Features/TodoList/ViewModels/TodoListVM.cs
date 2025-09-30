using Doera.Web.Features.TodoItem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Doera.Web.Features.TodoList.ViewModels {
    public record TodoListVM {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Order { get; init; }
        public IEnumerable<TodoItemSummaryVM> TodoItems { get; init; } = [];
        public int TotalItems => TodoItems?.Count() ?? 0;
    }
}
