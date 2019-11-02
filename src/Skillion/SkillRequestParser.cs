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

            if (string.IsNullOrEmpty(_skillionConfiguration.SkillId) && _skillionConfiguration.AlwaysValidateSkillRequest)
                throw new Exception(
                    "Unable to process request. Always validate skill id is true and skill id is missing");
            
            var rawJsonRequest = await ParseBodyToRequestAsync(httpRequest.Body);
            var skill = rawJsonRequest.ToObject<SkillRequest>();
            
            if (_webHostEnvironment.IsDevelopment() && !_skillionConfiguration.AlwaysValidateSkillRequest)
                return skill;

            if (skill.Context.System.Application.ApplicationId != _skillionConfiguration.SkillId)
                throw new Exception("Invalid application id.");
            
            if (!await IsValidSignature(httpRequest, skill, rawJsonRequest.ToString(Formatting.None)))
                throw new Exception("Invalid signature in request.");

            return skill;
        }
        
        // TODO Clean this up
        private async ValueTask<bool> IsValidSignature(HttpRequest request, SkillRequest skillRequest, string body)
        {
            var signatureCertChainUrl = new Uri(request.Headers["SignatureCertChainUrl"]);
            request.Headers.TryGetValue("Signature", out var encodedSignature);
            var isTimestampValid = RequestVerification.RequestTimestampWithinTolerance(skillRequest);
            var isRequestValid = await RequestVerification.Verify(encodedSignature, signatureCertChainUrl, body);

            return isTimestampValid && isRequestValid;
        }
        
        private async ValueTask<JObject> ParseBodyToRequestAsync(Stream stream)
        {
            using var streamReader = new HttpRequestStreamReader(stream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(streamReader);
            return await JObject.LoadAsync(jsonReader);
        }
    }
}