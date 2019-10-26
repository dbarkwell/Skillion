using Alexa.NET.Request.Type;

namespace Skillion.Services
{
    public interface IRouteDataService
    {
        bool TryGetRoute(Request request, out RouteData routeData);
    }
}