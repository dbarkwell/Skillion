using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;

namespace Skillion
{
    public interface ISkillRequestParser
    {
        ValueTask<SkillRequest> ParseHttpRequestAsync(HttpRequest httpRequest);
    }
}