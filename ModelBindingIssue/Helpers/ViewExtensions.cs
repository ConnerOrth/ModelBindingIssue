using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace ModelBindingIssue.Helpers
{
    public static class ViewExtensions
    {
        // {0} is the component name, {1} is the view name.
        private const string ViewPathFormat = "Components/{0}/{1}";

        /// <summary>
        /// Attempts to locate a view specified by <see cref="viewName"/>. If <see cref="viewName"/> is <c>null</c>,
        /// then an exception is thrown.
        /// </summary>
        /// <param name="context">The <see cref="ViewComponentContext"/> for the current component execution.</param>
        /// <returns>wether or not the specified view with <see cref="viewName"/> exists.</returns>
        public static bool ViewExists(this ViewComponentContext context, string viewName)
        {
            var viewEngine = context.ViewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            var viewContext = context.ViewContext;

            // If view name was passed in is already a path, the view engine will handle this.
            ViewEngineResult result = viewEngine.GetView(viewContext.ExecutingFilePath, viewName, isMainPage: false);
            //IEnumerable<string> originalLocations = result.SearchedLocations;

            if (result?.Success != true)
            {
                // This will produce a string like:
                //
                //  Components/Cart/Default
                //
                // The view engine will combine this with other path info to search paths like:
                //
                //  Views/Shared/Components/Cart/Default.cshtml
                //  Views/Home/Components/Cart/Default.cshtml
                //  Areas/Blog/Views/Shared/Components/Cart/Default.cshtml
                //
                // This supports a controller or area providing an override for component views.
                var qualifiedViewName = string.Format(
                    CultureInfo.InvariantCulture,
                    ViewPathFormat,
                    context.ViewComponentDescriptor.ShortName,
                    viewName);

                result = viewEngine.FindView(viewContext, qualifiedViewName, isMainPage: false);
            }

            return result.Success;
        }
    }
}
