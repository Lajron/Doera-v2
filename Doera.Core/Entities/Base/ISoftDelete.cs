using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Core.Entities.Base {
    public interface ISoftDelete {
        DateTimeOffset? DeletedAt { get; set; }

    }
}
