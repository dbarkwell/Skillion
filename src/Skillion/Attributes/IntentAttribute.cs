using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IntentAttribute : SkillionAttribute
    {
        public IntentAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; set; }
    }
}