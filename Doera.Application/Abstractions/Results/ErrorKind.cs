namespace Doera.Application.Abstractions.Results {
    public enum ErrorKind {
        Validation,
        NotFound,
        AccessDenied,
        Unauthorized,
        Conflict,
        RateLimited,
        Failed
    }
}