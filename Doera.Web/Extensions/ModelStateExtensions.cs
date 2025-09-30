using Doera.Application.Abstractions.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Doera.Web.Extensions {
    public static class ModelStateExtensions {
        public static void AddResultErrors(this ModelStateDictionary modelState, IEnumerable<Error> errors) {
            foreach (var e in errors) {
                modelState.AddModelError(string.Empty, e.Description);
            }
        }
    }
}