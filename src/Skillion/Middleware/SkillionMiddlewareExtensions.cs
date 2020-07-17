using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skillion.Attributes;
using Skillion.IO;
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
            services.AddScoped<ISkillRequestValidator, SkillRequestValidator>();
            services.AddSingleton<IRouteDataService>(new RouteDataService(RouteMapper.MapRoutes(GetMethods(assembly))));
        }
        
        public static void UseSkillion(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(e => e.MapDynamicControllerRoute<SkillionRouteValueTransformer>("/"));
        }
        
        private static IEnumerable<MethodInfo> GetMethods(Assembly assembly)
        {
            return assembly.GetTypes().AsParallel()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(SkillionRequestAttribute), false).Any());    
        }
    }
}