using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;

namespace Skillion.IO
{
    internal interface ISkillRequestParser
    {
        Task<SkillRequest> ParseHttpRequestAsync(HttpRequest httpRequest);
    }
}