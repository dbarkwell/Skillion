using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class FallbackIntentAttribute : SkillionAttribute
    {  
        public static string Name => "AMAZON.FallbackIntent";
    }
}