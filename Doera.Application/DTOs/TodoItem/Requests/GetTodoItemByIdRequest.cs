using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.TodoItem.Requests {
    public record GetTodoItemByIdRequest {
        public Guid Id { get; init; }

    }
}
