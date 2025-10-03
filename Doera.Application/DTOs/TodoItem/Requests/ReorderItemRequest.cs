using Doera.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.TodoItem.Requests {
    public record ReorderItemRequest {
        public Guid TodoListId { get; init; }
        public IEnumerable<ReorderRequest> ReorderRequests { get; init; } = [];
    }
}
