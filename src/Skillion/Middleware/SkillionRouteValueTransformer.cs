using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
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
                httpContext.Request.Method != HttpMethods.Post ||
                !IsValidContentType(httpContext.Request.ContentType))
                return null;
            
            SkillRequest skill;
           
            try
            {
                skill = await _skillRequestParser.ParseHttpRequestAsync(httpContext.Request);
            }
            catch (JsonException e)
            {
                // log;
                return null;
            }
            
            if (!_routeDataService.TryGetRoute(skill.Request, out var routeData))
                return null;

            httpContext.Items["request"] = skill.Request;
            httpContext.Items["context"] = skill.Context;
            httpContext.Items["session"] = skill.Session;
            
            return new RouteValueDictionary
            {
                {"controller", routeData.Controller},
                {"action", routeData.Action}
            };
        }

        private static bool IsValidContentType(string contentType)
        {
            return contentType.Equals("application/json") ||
                   contentType.Equals("application/json; charset=UTF-8");
        }
    }
}