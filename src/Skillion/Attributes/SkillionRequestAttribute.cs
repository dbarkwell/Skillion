using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class SkillionRequestAttribute : Attribute
    {
        public abstract string Name { get; }
    }
}