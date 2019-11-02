using System.Collections.Generic;
using Alexa.NET.Request.Type;

namespace Skillion.Services
{
    internal class RouteDataService : IRouteDataService
    {
        private readonly IDictionary<string, RouteData> _routes;

        public RouteDataService(IDictionary<string, RouteData> routes)
        {
            _routes = routes;
        }

        public bool TryGetRoute(Request request, out RouteData routeData)
        {
            var routeName = GetRouteName(request);
            if (!HasRoute(routeName))
            {
                routeData = null;
                return false;
            }

            routeData = _routes[routeName];
            return true;
        }

        private bool HasRoute(string routeName)
        {
            return !string.IsNullOrEmpty(routeName) && _routes.ContainsKey(routeName);
        }

        private static string GetRouteName(Request request)
        {
            return request.Type switch
            {
                "IntentRequest" => ((IntentRequest) request).Intent.Name,
                _ => request.Type
            };
        }
    }
}