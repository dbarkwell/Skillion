using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class FallbackIntentRequestAttribute : SkillionRequestAttribute
    {
        public override string Name => "AMAZON.FallbackIntent";
    }
}