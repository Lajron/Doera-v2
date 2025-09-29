namespace Doera.Application.Abstractions.Results {
    public sealed record Error(string Code, string Description, string? Field = null);
}