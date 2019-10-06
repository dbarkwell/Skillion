using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Skillion.Attributes;
using Skillion.Services;

namespace Skillion.Middleware
{
    public static class SkillionMiddlewareExtensions
    {
        public static void AddSkillion(this IServiceCollection services)
        {
            var assembly = Assembly.GetCallingAssembly();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddSingleton<SkillionRouteValueTransformer>();
            services.AddSingleton<IRouteDataService>(MapRoutes(assembly));
        }
        
        public static void UseSkillion(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(e => e.MapDynamicControllerRoute<SkillionRouteValueTransformer>(string.Empty));
        }

        private static RouteDataService MapRoutes(Assembly assembly)
        {
            var methods = assembly.GetTypes().AsParallel()
                 .SelectMany(t => t.GetMethods())
                 .Where(m => m.GetCustomAttributes(typeof(SkillionAttribute), false).Any())
                 .ToArray();
            
            var routeMapDictionary = new Dictionary<string, RouteData>();
            foreach (var method in methods)
            {
                var skillionAttribute = method.GetCustomAttribute<SkillionAttribute>();
                string name;
                switch (skillionAttribute)
                {
                    case IntentAttribute intentAttribute:
                        name = intentAttribute.Name;
                        break;
                    case LaunchAttribute launch:
                        name = LaunchAttribute.Name;
                        break;
                    case SessionEndedAttribute sessionEnded:
                        name = SessionEndedAttribute.Name;
                        break;
                    case FallbackIntentAttribute fallback:
                        name = FallbackIntentAttribute.Name;
                        break;
                    default:
                        continue;
                }
                
                var type = method.ReflectedType.FullName.Split(".");
                var controller = type[^1].Replace("Controller", string.Empty);
                var action = method.Name;
                routeMapDictionary.Add(name, new RouteData(controller, action));
            }

            return new RouteDataService(routeMapDictionary);
        }
    }
}