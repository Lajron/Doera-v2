using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.TodoItem.Requests {
    public record UpdateTodoItemStatusRequest {
        public required Guid Id { get; init; }
        public required TodoStatus Status { get; init; }
    }
}
