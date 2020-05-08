using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skillion.IO;
using Skillion.Services;

namespace Skillion.Middleware
{
    public static class SkillionMiddlewareExtensions
    {
        public static void AddSkillion(this IServiceCollection services, Dictionary<string, RouteData> routeMap)
        {
            services.AddControllers().AddNewtonsoftJson();

            var config = services.BuildServiceProvider().GetService<IConfiguration>();
            var section = config.GetSection("Skillion:Configuration");
            services.Configure<SkillionConfiguration>(section);

            services.AddScoped<ISkillRequestParser, SkillRequestParser>();
            services.AddScoped<SkillionRouteValueTransformer>();
            services.AddScoped<ISkillRequestValidator, SkillRequestValidator>();
            services.AddSingleton<IRouteDataService>(new RouteDataService(routeMap));
        }

        public static void UseSkillion(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(e => e.MapDynamicControllerRoute<SkillionRouteValueTransformer>("/"));
        }
    }
}