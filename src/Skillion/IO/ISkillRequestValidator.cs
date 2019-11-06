using System;
using System.Threading.Tasks;
using Alexa.NET.Request;

namespace Skillion.IO
{
    public interface ISkillRequestValidator
    {
        bool IsTimestampValid(SkillRequest skillRequest);
        ValueTask<bool> IsRequestValidAsync(string encodedSignature, Uri signatureCertChainUrl, string request);
    }
}