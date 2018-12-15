using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SessionEndedAttribute : SkillionAttribute
    {  
        public static string Name => "SessionEnded";
    }
}