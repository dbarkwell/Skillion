using System;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Skillion.Services;

namespace Skillion.Middleware
{
    public class SkillionRouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly ILogger _logger;
        private readonly IRouteDataService _routeDataService;
        private readonly ISkillRequestParser _skillRequestParser;

        public SkillionRouteValueTransformer(
            ISkillRequestParser skillRequestParser,
            IRouteDataService routeDataService,
            ILogger<SkillionRouteValueTransformer> logger = null)
        {
            _skillRequestParser = skillRequestParser;
            _routeDataService = routeDataService;
            _logger = logger;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext,
            RouteValueDictionary values)
        {
            if (httpContext.Request.Path != "/" ||
                httpContext.Request.Method != HttpMethods.Post ||
                !IsValidContentType(httpContext.Request.ContentType))
            {
                _logger?.LogError(
                    $"Incorrect request. Path: {httpContext.Request.Path.ToString()} Method: {httpContext.Request.Method} Content-Type {httpContext.Request.ContentType}");

                return null;
            }

            SkillRequest skill;

            try
            {
                skill = await _skillRequestParser.ParseHttpRequestAsync(httpContext.Request);
            }
            catch (Exception e)
            {
                _logger?.LogError(e.Message);
                return null;
            }

            if (!_routeDataService.TryGetRoute(skill.Request, out var routeData))
            {
                _logger?.LogError($"Unable to find route {_routeDataService.GetRouteName(skill.Request)}");

                return null;
            }

            httpContext.Items["request"] = skill.Request;
            httpContext.Items["context"] = skill.Context;
            httpContext.Items["session"] = skill.Session;
            
            _logger?.LogInformation($"Routing to: controller = {routeData.Controller}, action = {routeData.Action}");
            
            return new RouteValueDictionary
            {
                {"controller", routeData.Controller},
                {"action", routeData.Action}
            };
        }

        private static bool IsValidContentType(string contentType)
        {
            var contentTypeLower = contentType.ToLower();
            return contentTypeLower.Equals("application/json") ||
                   contentTypeLower.Equals("application/json; charset=utf-8");
        }
    }
}