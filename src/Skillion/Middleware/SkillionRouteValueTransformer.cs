using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Skillion.Attributes;
using Skillion.Services;

namespace Skillion.Middleware
{
    public class SkillionRouteValueTransformer : DynamicRouteValueTransformer
    {
        private const long ContentLengthLimit = 1024 * 1024 * 128;
        private readonly IRouteDataService _routeDataService;
        
        public SkillionRouteValueTransformer(IRouteDataService routeDataService)
        {
            _routeDataService = routeDataService;
        }
        
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (httpContext.Request.Path != "/" ||
                httpContext.Request.Method != HttpMethods.Post)
            {
                return null;
            }

            try
            {
                var skill = await ParseBodyToRequestAsync(httpContext.Request);
                var route = GetRouteName(skill.Request);

                if (string.IsNullOrEmpty(route) || !_routeDataService.HasRoute(route))
                    throw new IntentNotFoundException();

                var routeMap = _routeDataService.GetRouteMap(route);
                httpContext.Items["request"] = skill.Request;
                httpContext.Items["context"] = skill.Context;

                if (skill.Request is IntentRequest intent)
                {
                    httpContext.Request.ContentType = "application/json";
                    httpContext.Request.Body = SerializeAttributesToStream(intent.Intent.Slots);
                }

                return new RouteValueDictionary
                {
                    {"controller", routeMap.Controller},
                    {"action", routeMap.Action}
                };
            }
            catch (Exception ex)
            {
                // TODO log exception
                return null;
            }
        }
        
        private static async Task<SkillRequest> ParseBodyToRequestAsync(HttpRequest request)
        {
            if (request.ContentLength == 0)
                throw new InvalidDataException("Request body is missing.");
                    
            if (request.ContentLength > ContentLengthLimit)
                throw new InvalidDataException("Request body is too large.");

            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();
            try
            {
                return JsonConvert.DeserializeObject<SkillRequest>(body);
            }
            catch
            {
                throw new InvalidDataException("Unable to parse body of request.");
            }
        }
        
        private static MemoryStream SerializeAttributesToStream(IDictionary<string, Slot> slots)
        {
            var json = JsonConvert.SerializeObject(slots);
            return new MemoryStream(Encoding.UTF8.GetBytes(json))
            {
                Position = 0
            };
        }
        
        private static string GetRouteName(Request request)
        {
            switch (request.Type)
            {
                case "IntentRequest":
                    return ((IntentRequest)request).Intent.Name;
                case "LaunchRequest":
                    return LaunchAttribute.Name;
                case "SessionEndedRequest":
                    return SessionEndedAttribute.Name;
                default:
                    return string.Empty;
            }     
        }
    }
}