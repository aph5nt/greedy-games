using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Infrastructure
{
    public static class RouteExtensionMvc
    {
        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Insert(0, new RouteConvention(routeAttribute));
        }
    }
    public class RouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// The _central prefix.
        /// </summary>
        private readonly AttributeRouteModel _centralPrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteConvention"/> class.
        /// </summary>
        /// <param name="routeTemplateProvider">
        /// The route template provider.
        /// </param>
        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _centralPrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        /// <inheritdoc />
        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            this._centralPrefix,
                            selectorModel.AttributeRouteModel);
                    }
                }

                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (!unmatchedSelectors.Any())
                {
                    continue;
                }

                foreach (var selectorModel in unmatchedSelectors)
                {
                    selectorModel.AttributeRouteModel = _centralPrefix;
                }
            }
        }
    }
}
