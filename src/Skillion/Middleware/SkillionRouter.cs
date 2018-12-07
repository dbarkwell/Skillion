using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Routing;

using Newtonsoft.Json;
using Skillion.Attributes;
using Skillion.IO;

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

            try
            {
                var request = await ParseBodyToStandardRequest(context.HttpContext.Request);
                string route;
                switch (request.Request.Type)
                {
                    case "IntentRequest":
                        route = request.Request.Intent.Name;
                        break;
                    case "LaunchRequest":
                        route = LaunchAttribute.Name;
                        break;
                    default:
                        await NotFoundResponse(context);
                        return;
                } 
                
                await IntentResponse(context, route, request.Session);
            }
            catch
            {
                // TODO log exception
                await ExceptionThrownResponse(context);
            }
        }

        private async Task IntentResponse(RouteContext context, string route, Session session = null)
        {
            if (!_routing.ContainsKey(route))
                throw new IntentNotFoundException();

            var controllerAction = _routing[route];
            context.RouteData.Values["controller"] = controllerAction.Item1;
            context.RouteData.Values["action"] = controllerAction.Item2;

            if (session != null && !session.New && session.Attributes != null)
            {
                context.HttpContext.Request.ContentType = "application/json";
                context.HttpContext.Request.Body = SerializeAttributesToStream(session.Attributes);
            }

            await _defaultRouter.RouteAsync(context);
        }
        
        private async Task NotFoundResponse(RouteContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await _defaultRouter.RouteAsync(context);
        }
        
        private async Task ExceptionThrownResponse(RouteContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await _defaultRouter.RouteAsync(context);
        }
        
        private static async Task<StandardRequest> ParseBodyToStandardRequest(HttpRequest request)
        {
            VerifyRequest(request);
            
            request.EnableRewind();
            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();
                try
                {
                    return JsonConvert.DeserializeObject<StandardRequest>(body);
                }
                catch
                {
                    throw new InvalidDataException("Unable to parse body of request.");
                }
            }
        }

        private static void VerifyRequest(HttpRequest request)
        {
            if (request.ContentLength == 0)
                throw new InvalidDataException("Request body is missing.");
                    
            if (request.ContentLength > ContentLengthLimit)
                throw new InvalidDataException("Request body is too large.");
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