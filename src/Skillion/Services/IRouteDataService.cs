namespace Skillion.Services
{
    public interface IRouteDataService
    {
        RouteData GetRouteMap(string name);
        
        bool HasRoute(string name);
    }
}