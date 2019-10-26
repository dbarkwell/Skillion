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
using Skillion.Services;

namespace Skillion.Middleware
{
    public class SkillionRouteValueTransformer : DynamicRouteValueTransformer
    {
        private readonly ISkillRequestParser _skillRequestParser;
        private readonly IRouteDataService _routeDataService;

        public SkillionRouteValueTransformer(ISkillRequestParser skillRequestParser, IRouteDataService routeDataService)
        {
            _skillRequestParser = skillRequestParser;
            _routeDataService = routeDataService;
        }
        
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (httpContext.Request.Path != "/" ||
                httpContext.Request.Method != HttpMethods.Post)
            {
                return null;
            }

            var skill = await _skillRequestParser.ParseHttpRequestAsync(httpContext.Request);
            
            if (!_routeDataService.TryGetRoute(skill.Request, out var routeData))
                throw new IntentNotFoundException();
            
            httpContext.Items["request"] = skill.Request;
            httpContext.Items["context"] = skill.Context;

            if (skill.Request is IntentRequest intent)
            {
                httpContext.Request.ContentType = "application/json";
                httpContext.Request.Body = SerializeAttributesToStream(intent.Intent.Slots);
            }

            return new RouteValueDictionary
            {
                {"controller", routeData.Controller},
                {"action", routeData.Action}
            };
        }
        
        private static MemoryStream SerializeAttributesToStream(IDictionary<string, Slot> slots)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(slots)))
            {
                Position = 0
            };
        }
    }
}