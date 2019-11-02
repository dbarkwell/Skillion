using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class IntentRequestAttribute : SkillionRequestAttribute
    {
        public IntentRequestAttribute(string name)
        {
            Name = name;
        }

        public override string Name { get; }
    }
}