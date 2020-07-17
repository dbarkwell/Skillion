using System;
using System.Collections.Generic;
using System.Reflection;
using Skillion.Attributes;
using Skillion.Services;

namespace Skillion
{
    internal static class RouteMapper
    {
        public static IDictionary<string, RouteData> MapRoutes(IEnumerable<MemberInfo> methods)
        {
            var routeMapDictionary = new Dictionary<string, RouteData>();
            foreach (var method in methods)
            {
                var skillionAttribute = method.GetCustomAttribute<SkillionRequestAttribute>();

                if (skillionAttribute is null)
                    continue;
                
                var type = method.ReflectedType?.FullName?.Split(".");
                if (type == null || type.Length == 0)
                    continue;

                var controller = type[^1].Replace("Controller", string.Empty);
                var action = RemoveAsyncSuffix(method.Name);

                routeMapDictionary.Add(skillionAttribute.Name, new RouteData(controller, action));
            }

            return routeMapDictionary;
        }
        
        private static string RemoveAsyncSuffix(string action)
        {
            var lastLocation = action.LastIndexOf("Async", StringComparison.Ordinal);
            return lastLocation == -1 ? action : action.Remove(lastLocation);
        }
    }
}
