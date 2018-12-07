using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class LaunchAttribute : SkillionAttribute
    {  
        public static string Name => "Launch";
    }
}