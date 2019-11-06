using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skillion.Middleware;

namespace Skillion.IO
{
    public class SkillRequestParser : ISkillRequestParser
    {
        private const long ContentLengthLimit = 1024 * 24;
        private readonly ILogger _logger;
        private readonly ISkillRequestValidator _requestValidator;
        private readonly SkillionConfiguration _skillionConfiguration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SkillRequestParser(IWebHostEnvironment webHostEnvironment,
            IOptions<SkillionConfiguration> skillionConfiguration,
            ISkillRequestValidator requestValidator,
            ILogger<SkillionRouteValueTransformer> logger = null)
        {
            _webHostEnvironment = webHostEnvironment;
            _skillionConfiguration = skillionConfiguration.Value;
            _requestValidator = requestValidator;
            _logger = logger;
        }

        public async ValueTask<SkillRequest> ParseHttpRequestAsync(HttpRequest httpRequest)
        {
            if (httpRequest.ContentLength == null || httpRequest.ContentLength <= 0)
                throw new InvalidDataException("Request body is missing.");

            if (httpRequest.ContentLength > ContentLengthLimit)
                throw new InvalidDataException("Request body is too large.");

            if (string.IsNullOrEmpty(_skillionConfiguration.SkillId) &&
                _skillionConfiguration.AlwaysValidateSkillRequest)
                throw new Exception(
                    "Unable to process request. Always validate skill id is true and skill id is missing");

            var rawJsonRequest = await ParseBodyToRequestAsync(httpRequest.Body);
            if (_logger?.IsEnabled(LogLevel.Debug) ?? false)
                _logger?.LogDebug(rawJsonRequest.ToString(Formatting.Indented));

            var skill = rawJsonRequest.ToObject<SkillRequest>();

            if (_webHostEnvironment.IsDevelopment() && !_skillionConfiguration.AlwaysValidateSkillRequest)
                return skill;

            if (skill.Context.System.Application.ApplicationId != _skillionConfiguration.SkillId)
                throw new Exception("Invalid application id.");

            if (!await IsValidRequestAsync(httpRequest, skill, rawJsonRequest.ToString(Formatting.None)))
                throw new Exception("Invalid request. Check timestamp or signature.");

            return skill;
        }

        private async ValueTask<bool> IsValidRequestAsync(HttpRequest request, SkillRequest skillRequest, string body)
        {
            if (!request.Headers.TryGetValue("SignatureCertChainUrl", out var signatureCertChainUrl))
                return false;

            if (!request.Headers.TryGetValue("Signature", out var encodedSignature))
                return false;

            var isTimestampValid = _requestValidator.IsTimestampValid(skillRequest);
            var isRequestValid =
                await _requestValidator.IsRequestValidAsync(encodedSignature, new Uri(signatureCertChainUrl), body);


            _logger?.LogInformation($"Timestamp validation {isTimestampValid.ToString()}");
            _logger?.LogInformation($"Request validation {isRequestValid.ToString()}");

            return isTimestampValid && isRequestValid;
        }

        private static async ValueTask<JObject> ParseBodyToRequestAsync(Stream stream)
        {
            using var streamReader = new HttpRequestStreamReader(stream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(streamReader);
            return await JObject.LoadAsync(jsonReader);
        }
    }
}