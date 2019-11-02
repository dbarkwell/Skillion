using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Skillion
{
    public class SkillRequestParser : ISkillRequestParser
    {
        private const long ContentLengthLimit = 1024 * 1024 * 128;
        private readonly SkillionConfiguration _skillionConfiguration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SkillRequestParser(IWebHostEnvironment webHostEnvironment,
            IOptions<SkillionConfiguration> skillionConfiguration)
        {
            _webHostEnvironment = webHostEnvironment;
            _skillionConfiguration = skillionConfiguration.Value;
        }

        public async ValueTask<SkillRequest> ParseHttpRequestAsync(HttpRequest httpRequest)
        {
            if (httpRequest.ContentLength == null || httpRequest.ContentLength <= 0)
                throw new InvalidDataException("Request body is missing.");

            if (httpRequest.ContentLength > ContentLengthLimit)
                throw new InvalidDataException("Request body is too large.");

            if (string.IsNullOrEmpty(_skillionConfiguration.SkillId) && _skillionConfiguration.AlwaysValidateSkillId)
                throw new Exception(
                    "Unable to process request. Always validate skill id is true and skill id is missing");

            var skill = await ParseBodyToRequestAsync(httpRequest.Body);

            if (_webHostEnvironment.IsDevelopment() && !_skillionConfiguration.AlwaysValidateSkillId)
                return skill;

            if (skill.Context.System.Application.ApplicationId != _skillionConfiguration.SkillId)
                throw new Exception("Invalid application id.");

            return skill;
        }

        private static async ValueTask<SkillRequest> ParseBodyToRequestAsync(Stream stream)
        {
            using var streamReader = new HttpRequestStreamReader(stream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(streamReader);
            return (await JObject.LoadAsync(jsonReader)).ToObject<SkillRequest>();
        }
    }
}