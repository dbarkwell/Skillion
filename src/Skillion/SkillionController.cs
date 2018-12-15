using Microsoft.AspNetCore.Mvc;
using Skillion.IO;

namespace Skillion
{
    public abstract class SkillionController : ControllerBase
    {
        protected StandardRequest StandardRequest => (StandardRequest) HttpContext?.Items["standardRequest"];

        protected Context Context => (Context) HttpContext?.Items["context"];
    }
}