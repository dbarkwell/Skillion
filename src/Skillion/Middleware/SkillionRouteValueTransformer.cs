using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Skillion.Services;

namespace Skillion.Middleware
{
    public class SkillionRouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly IRouteDataService _routeDataService;
        private readonly ISkillRequestParser _skillRequestParser;

        public SkillionRouteValueTransformer(ISkillRequestParser skillRequestParser, IRouteDataService routeDataService)
        {
            _skillRequestParser = skillRequestParser;
            _routeDataService = routeDataService;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext,
            RouteValueDictionary values)
        {
            if (httpContext.Request.Path != "/" ||
                httpContext.Request.Method != HttpMethods.Post)
                return null;

            var skill = await _skillRequestParser.ParseHttpRequestAsync(httpContext.Request);

            if (!_routeDataService.TryGetRoute(skill.Request, out var routeData))
                return null;

            httpContext.Items["request"] = skill.Request;
            httpContext.Items["context"] = skill.Context;
            httpContext.Items["session"] = skill.Session;

            values["controller"] = routeData.Controller;
            values["action"] = routeData.Action;

            return values;
        }
    }
}