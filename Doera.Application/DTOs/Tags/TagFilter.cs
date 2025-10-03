using Doera.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doera.Application.DTOs.Tags {
    public record TagFilter {
        public Guid? TodoListId { get; init; }
        public string? Search { get; init; }
        public SortDirection Direction { get; init; } = SortDirection.Asc;
    }
}
