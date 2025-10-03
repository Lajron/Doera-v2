namespace Doera.Web.Features.Error.ViewModels;

public record ErrorVM(
    int StatusCode,
    string Title,
    string? Detail,
    string TraceId,
    string? Path,
    string? QueryString,
    string? ExceptionMessage,
    string? StackTrace
);