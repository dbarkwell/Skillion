using System;
using System.Threading.Tasks;
using Alexa.NET.Request;

namespace Skillion.IO
{
    public class SkillRequestValidator : ISkillRequestValidator
    {
        public bool IsTimestampValid(SkillRequest skillRequest)
        {
            return RequestVerification.RequestTimestampWithinTolerance(skillRequest);
        }

        public async ValueTask<bool> IsRequestValidAsync(string encodedSignature, Uri signatureCertChainUrl, string request)
        {
            return await RequestVerification.Verify(encodedSignature, signatureCertChainUrl, request);
        }
    }
}