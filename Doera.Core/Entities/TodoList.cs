using Doera.Core.Entities.Base;
using System;
using System.Collections.Generic;

namespace Doera.Core.Entities {
    public class TodoList : Entity<Guid>, IAuditable {
        public required string Name { get; set; }
        public int Order { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}