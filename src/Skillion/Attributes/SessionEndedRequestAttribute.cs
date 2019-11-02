using System;
using Alexa.NET.Request.Type;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SessionEndedRequestAttribute : SkillionRequestAttribute
    {
        public override string Name => nameof(SessionEndedRequest);
    }
}