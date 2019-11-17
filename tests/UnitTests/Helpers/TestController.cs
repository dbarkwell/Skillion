using Alexa.NET.Response;
using Skillion;

namespace SkillionUnitTests.Helpers
{
    public class TestController : SkillionController
    {
        public SkillionActionResult<SkillResponse> ReturnSkillionActionResult() => 
            new SkillionActionResult<SkillResponse>(new SkillResponse());
    }
}