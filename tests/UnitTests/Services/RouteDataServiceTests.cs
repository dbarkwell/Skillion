using System.Collections.Generic;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Skillion.Services;
using Xunit;

namespace SkillionUnitTests.Services
{
    public class RouteDataServiceTests
    {
        public class TryGetRoute
        {
            [Fact]
            public void IntentRequestDoesNotHaveRoute_ReturnsFalse()
            {
                var routes = new Dictionary<string, RouteData>
                    {{"SomeIntent", new RouteData("TestController", "Index")}};
                var routeDataService = new RouteDataService(routes);

                var hasRoute = routeDataService.TryGetRoute(
                    new IntentRequest {Intent = new Intent {Name = "Test"}, Type = "IntentRequest"}, out var route);

                Assert.False(hasRoute);
            }

            [Fact]
            public void IntentRequestDoesNotHaveRoute_ReturnsNullRouteData()
            {
                var routes = new Dictionary<string, RouteData>
                    {{"SomeIntent", new RouteData("TestController", "Index")}};
                var routeDataService = new RouteDataService(routes);

                routeDataService.TryGetRoute(
                    new IntentRequest {Intent = new Intent {Name = "Test"}, Type = "IntentRequest"}, out var route);

                Assert.Null(route);
            }

            [Fact]
            public void IntentRequestHasRoute_ReturnsRouteData()
            {
                var routes = new Dictionary<string, RouteData> {{"Test", new RouteData("TestController", "Index")}};
                var routeDataService = new RouteDataService(routes);

                routeDataService.TryGetRoute(
                    new IntentRequest {Intent = new Intent {Name = "Test"}, Type = "IntentRequest"}, out var route);

                Assert.True(routes["Test"].Equals(route));
            }

            [Fact]
            public void IntentRequestHasRoute_ReturnsTrue()
            {
                var routes = new Dictionary<string, RouteData> {{"Test", new RouteData("TestController", "Index")}};
                var routeDataService = new RouteDataService(routes);

                var hasRoute = routeDataService.TryGetRoute(
                    new IntentRequest {Intent = new Intent {Name = "Test"}, Type = "IntentRequest"}, out var route);

                Assert.True(hasRoute);
            }

            [Fact]
            public void LaunchRequestHasRoute_ReturnsRouteData()
            {
                var routes = new Dictionary<string, RouteData>
                    {{nameof(LaunchRequest), new RouteData("TestController", "Index")}};
                var routeDataService = new RouteDataService(routes);

                routeDataService.TryGetRoute(new LaunchRequest {Type = "LaunchRequest"}, out var route);

                Assert.True(routes[nameof(LaunchRequest)].Equals(route));
            }

            [Fact]
            public void LaunchRequestHasRoute_ReturnsTrue()
            {
                var routes = new Dictionary<string, RouteData>
                    {{nameof(LaunchRequest), new RouteData("TestController", "Index")}};
                var routeDataService = new RouteDataService(routes);

                var hasRoute = routeDataService.TryGetRoute(new LaunchRequest {Type = "LaunchRequest"}, out var route);

                Assert.True(hasRoute);
            }
        }
    }
}