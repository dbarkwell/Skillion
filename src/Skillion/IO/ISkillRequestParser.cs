using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;

namespace Skillion.IO
{
    public interface ISkillRequestParser
    {
        ValueTask<SkillRequest> ParseHttpRequestAsync(HttpRequest httpRequest);
    }
}