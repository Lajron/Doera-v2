using Doera.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Entities {
    public class UserTag : Entity<Guid> {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid TagId { get; set; }
        public Tag? Tag { get; set; }

        public required string DisplayName { get; set; }

        public ICollection<TodoItemTag> TodoItemTags { get; set; } = [];

    }
}
