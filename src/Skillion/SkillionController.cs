using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Microsoft.AspNetCore.Mvc;

namespace Skillion
{
    public abstract class SkillionController : ControllerBase
    {
        protected Request RequestContext => (Request) HttpContext?.Items["request"];

        protected Context SkillContext => (Context) HttpContext?.Items["context"];

        protected Session SessionContext => (Session) HttpContext?.Items["session"];

        protected virtual bool TryCastRequest<T>(out T request) where T : Request
        {
            if (RequestContext is T castRequest)
            {
                request = castRequest;
                return true;
            }

            request = null;
            return false;
        }
    }
}