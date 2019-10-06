using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Microsoft.AspNetCore.Mvc;

namespace Skillion
{
    public abstract class SkillionController : ControllerBase
    {
        protected Request RequestContext => (Request) HttpContext?.Items["request"];

        protected Context Context => (Context) HttpContext?.Items["context"];
    }
}