using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Builder;

using Skillion.Attributes;

namespace Skillion.Middleware
{
    public static class SkillionMiddlewareExtensions
    {
        public static void UseSkillion(this IApplicationBuilder app)
        {
            var assembly = Assembly.GetCallingAssembly();

            var methods = assembly.GetTypes().AsParallel()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(IntentAttribute), false).Length > 0)
                .ToArray();
            
            IDictionary<string, Tuple<string, string>> routing = new Dictionary<string, Tuple<string, string>>();
            foreach (var method in methods)
            {
                var intentAttribute = method.GetCustomAttribute<IntentAttribute>();
                var intent = intentAttribute.Name;
                var type = method.ReflectedType.FullName.Split(".");
                var controller = type[type.Length - 1].Replace("Controller", string.Empty);
                var action = method.Name;
                routing.Add(intent, new Tuple<string, string>(controller, action));
            }
            
            app.UseMvc(routes =>
            {
                routes.Routes.Add(new SkillionRouter(routes.DefaultHandler, routing));
            });
        }
    }
}