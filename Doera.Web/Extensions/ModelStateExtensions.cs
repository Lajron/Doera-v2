using Doera.Application.Abstractions.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Doera.Web.Extensions {
    public static class ModelStateExtensions {
        public static void AddResultErrors(this ModelStateDictionary modelState, IEnumerable<Error> errors) {
            foreach (var e in errors) {
                var key = string.IsNullOrWhiteSpace(e.Field) ? string.Empty : e.Field!;
                modelState.AddModelError(key, e.Description);
            }
        }
    }
}