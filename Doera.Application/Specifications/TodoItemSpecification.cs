using Doera.Core.Entities;
using Doera.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.Specifications {
    internal class TodoItemSpecification : BaseSpecification<TodoItem> {
        public TodoItemSpecification(
            Guid todoItemId,
            Guid userId,
            bool includeTags = false
        ) : base(i => i.Id == todoItemId && i.UserId == userId) {
            if (includeTags) {
                AddInclude(i => i.TodoItemTags!);
            }
        }
    }
}
