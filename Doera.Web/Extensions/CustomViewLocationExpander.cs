using Microsoft.AspNetCore.Mvc.Razor;

namespace Doera.Web.Extensions {
    public class CustomViewLocationExpander : IViewLocationExpander {
        public void PopulateValues(ViewLocationExpanderContext context) { }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations) {
            // {0} = view name, {1} = controller, {2} = area
            var featureLocations = new[] {
                "/Features/{1}/{0}.cshtml",
                "/Features/{1}/Partials/{0}.cshtml",
                "/Features/{1}/Components/{0}.cshtml",
            };

            var sharedLocations = new[] {
                "/Views/Shared/{0}.cshtml",
                "/Views/Shared/Layouts/{0}.cshtml",
                "/Views/Shared/Partials/{0}.cshtml",
                "/Views/Shared/Partials/Common/{0}.cshtml",
                "/Views/Shared/Partials/TodoItem/{0}.cshtml",
                "/Views/Shared/Partials/TodoList/{0}.cshtml",
                "/Views/Shared/Partials/Home/{0}.cshtml",
                "/Views/Shared/Partials/Tags/{0}.cshtml",
                "/Views/Shared/Partials/Account/{0}.cshtml",
                "/Views/Shared/Components/{0}.cshtml"
            };

            if (!string.IsNullOrEmpty(context.AreaName)) {
                var areaLocations = new[] {
                    "/Areas/{2}/Features/{1}/{0}.cshtml",
                    "/Areas/{2}/Views/{1}/{0}.cshtml",
                    "/Areas/{2}/Views/Shared/Layouts/{0}.cshtml",
                    "/Areas/{2}/Views/Shared/Partials/{0}.cshtml",
                    "/Areas/{2}/Views/Shared/Modules/{0}.cshtml",
                    "/Areas/{2}/Views/Shared/Modules/Cards/{0}.cshtml",
                    "/Areas/{2}/Views/Shared/Components/{0}.cshtml",
                };
                return areaLocations.Concat(featureLocations).Concat(sharedLocations).Concat(viewLocations);
            }

            return featureLocations.Concat(sharedLocations).Concat(viewLocations);
        }
    }
}
