using System;
using System.Threading.Tasks;
using Alexa.NET.Request;

namespace Skillion.IO
{
    internal class SkillRequestValidator : ISkillRequestValidator
    {
        public bool IsTimestampValid(SkillRequest skillRequest)
        {
            return RequestVerification.RequestTimestampWithinTolerance(skillRequest);
        }

        public async Task<bool> IsRequestValidAsync(string encodedSignature, Uri signatureCertChainUrl,
            string request)
        {
            return await RequestVerification.Verify(encodedSignature, signatureCertChainUrl, request);
        }
    }
}