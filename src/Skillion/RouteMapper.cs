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

                var controller = RemoveSuffix(type[^1], "Controller");
                var action = RemoveSuffix(method.Name, "Async");

                routeMapDictionary.Add(skillionAttribute.Name, new RouteData(controller, action));
            }

            return routeMapDictionary;
        }
        
        private static string RemoveSuffix(string value, string suffix)
        {
            var lastLocation = value.LastIndexOf(suffix, StringComparison.Ordinal);
            return lastLocation == -1 ? value : value.Remove(lastLocation);
        }
    }
}
