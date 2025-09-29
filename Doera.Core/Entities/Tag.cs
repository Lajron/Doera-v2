using Doera.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Entities {
    public class Tag : Entity<Guid>, IAuditable {
        public required string NormalizedName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }

        public ICollection<UserTag> UserTags { get; set; } = [];
    }
}
