using System.Collections.Generic;

namespace Skillion.Services
{
    public class RouteDataService : IRouteDataService
    {
        private readonly IDictionary<string, RouteData> _routes;
        
        public RouteDataService(IDictionary<string, RouteData> routes)
        {
            _routes = routes;
        }
        
        public RouteData GetRouteMap(string name)
        {
            return HasRoute(name) ? _routes[name] : null;
        }

        public bool HasRoute(string name)
        {
            return _routes.ContainsKey(name);
        }
    }
}