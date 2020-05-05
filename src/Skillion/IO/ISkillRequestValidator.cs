using System;
using System.Threading.Tasks;
using Alexa.NET.Request;

namespace Skillion.IO
{
    internal interface ISkillRequestValidator
    {
        bool IsTimestampValid(SkillRequest skillRequest);
        
        Task<bool> IsRequestValidAsync(string encodedSignature, Uri signatureCertChainUrl, string request);
    }
}