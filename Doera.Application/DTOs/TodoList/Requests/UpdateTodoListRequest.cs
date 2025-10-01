using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.TodoList.Requests {
    public record UpdateTodoListRequest {
        public required Guid Id { get; init; }
        public string? Name { get; init; }
    }
}
