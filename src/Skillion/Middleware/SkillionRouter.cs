using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Routing;

using Newtonsoft.Json;

using SkillionTest.IO;

namespace Skillion.Middleware
{
    public class SkillionRouter : IRouter
    {
        private const long ContentLengthLimit = 1024 * 1024 * 128;
        
        private readonly IRouter _defaultRouter;
        private readonly IDictionary<string, Tuple<string, string>> _routing;
        
        public SkillionRouter(IRouter defaultRouteHandler, IDictionary<string, Tuple<string, string>> routing)
        {
            _defaultRouter = defaultRouteHandler;
            _routing = routing;
        }
 
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _defaultRouter.GetVirtualPath(context);
        }
 
        public async Task RouteAsync(RouteContext context)
        {
            if (context.HttpContext.Request.Path != "/" ||
                context.HttpContext.Request.Method != HttpMethods.Post)
            {
                await NotFoundResponse(context);
                return;
            }

            var intent = await ParseBodyToIntentRequest(context.HttpContext.Request);
            
            if (!_routing.ContainsKey(intent.Request.Intent.Name))
                throw new IntentNotFoundException();
            
            var route = _routing[intent.Request.Intent.Name];
            context.RouteData.Values["controller"] = route.Item1;
            context.RouteData.Values["action"] = route.Item2;
            
            if (!intent.Session.New && intent.Session.Attributes != null)
            {
                context.HttpContext.Request.ContentType = "application/json";
                context.HttpContext.Request.Body = SerializeAttributesToStream(intent.Session.Attributes);
            }
            
            await _defaultRouter.RouteAsync(context);
        }

        private async Task NotFoundResponse(RouteContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await _defaultRouter.RouteAsync(context);
        }
        
        private static async Task<IntentRequest> ParseBodyToIntentRequest(HttpRequest request)
        {
            request.EnableRewind();
            if (request.ContentLength > ContentLengthLimit)
                throw new InvalidDataException("Request body too large.");
            
            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();
                try
                {
                    return JsonConvert.DeserializeObject<IntentRequest>(body);
                }
                catch
                {
                    throw new InvalidDataException("Unable to parse body of request.");
                }
            }
        }

        private static MemoryStream SerializeAttributesToStream(IDictionary<string, object> attributes)
        {
            var json = JsonConvert.SerializeObject(attributes);
            return new MemoryStream(Encoding.UTF8.GetBytes(json))
            {
                Position = 0
            };
        }
    }
}