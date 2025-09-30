using System;

namespace Doera.Web.Features.Tags.ViewModels {
    public record TagVM {
        public Guid Id { get; init; }
        public string DisplayName { get; init; } = string.Empty;
    }
}