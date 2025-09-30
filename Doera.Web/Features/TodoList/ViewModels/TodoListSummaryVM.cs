using System;

namespace Doera.Web.Features.TodoList.ViewModels {
    public record TodoListSummaryVM {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Order { get; init; }
        public int TotalItems { get; init; }
    }
}