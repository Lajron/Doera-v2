using Doera.Application.DTOs.TodoItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.TodoList {
    public record TodoListDto {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Order { get; init; } = 0;

        public IEnumerable<TodoItemSummaryDto> TodoItems { get; init; } = [];
    }
}
