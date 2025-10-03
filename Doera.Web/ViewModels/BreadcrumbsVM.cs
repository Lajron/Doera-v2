namespace Doera.Web.ViewModels {
    public record BreadcrumbsVM(params BreadcrumbSegment[] Segments);
    public record BreadcrumbSegment(string Text, string? Url = null, bool Active = false);
}