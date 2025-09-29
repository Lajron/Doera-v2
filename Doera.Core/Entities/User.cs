using Doera.Core.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Entities {
    public class User : IdentityUser<Guid>, IAuditable {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public ICollection<TodoItem> TodoItems { get; set; } = [];
        public ICollection<UserTag> UserTags { get; set; } = [];
    }
}
