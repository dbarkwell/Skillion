using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
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
            services.AddControllers().AddNewtonsoftJson();
            
            var config = services.BuildServiceProvider().GetService<IConfiguration>();
            var section = config.GetSection("Skillion:Configuration");
            services.Configure<SkillionConfiguration>(section);

            services.AddScoped<ISkillRequestParser, SkillRequestParser>();
            services.AddScoped<SkillionRouteValueTransformer>();
            services.AddSingleton<IRouteDataService>(MapRoutes(assembly));
        }
        
        public static void UseSkillion(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(e => e.MapDynamicControllerRoute<SkillionRouteValueTransformer>("/"));
        }

        private static RouteDataService MapRoutes(Assembly assembly)
        {
            var methods = assembly.GetTypes().AsParallel()
                 .SelectMany(t => t.GetMethods())
                 .Where(m => m.GetCustomAttributes(typeof(SkillionRequestAttribute), false).Any())
                 .ToArray();
            
            var routeMapDictionary = new Dictionary<string, RouteData>();
            foreach (var method in methods)
            {
                var skillionAttribute = method.GetCustomAttribute<SkillionRequestAttribute>();
                
                var type = method.ReflectedType?.FullName?.Split(".");
                if (type == null || type.Length == 0) 
                    continue;
                
                var controller = type[^1].Replace("Controller", string.Empty);
                var action = method.Name;

                routeMapDictionary.Add(skillionAttribute.Name, new RouteData(controller, action));
            }

            return new RouteDataService(routeMapDictionary);
        }
    }
}