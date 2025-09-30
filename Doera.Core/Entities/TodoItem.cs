using Doera.Core.Entities.Base;
using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Entities {
    public class TodoItem : Entity<Guid>, IAuditable, ISoftDelete {
        public required string Title { get; set; }
        public string? Description { get; set; }

        public int Order { get; set; } = 0;
        public TodoStatus Status { get; set; } = TodoStatus.None;
        public TodoPriority Priority { get; set; } = TodoPriority.None;

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public DateTimeOffset? ArchivedAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public Guid TodoListId { get; set; }
        public TodoList? TodoList { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
        public ICollection<TodoItemTag> TodoItemTags { get; set; } = [];


    }
}
