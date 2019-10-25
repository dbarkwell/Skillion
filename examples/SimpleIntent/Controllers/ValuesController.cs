using Alexa.NET;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Mvc;
using Skillion;
using Skillion.Attributes;

namespace SimpleIntent.Controllers
{
    public class ValuesController : ControllerBase
    {
        [IntentRequest("HelloIntent")]
        public SkillionActionResult<SkillResponse> HelloIntent()
        {
            var greeting = "Hello, how are you?";
            return ResponseBuilder.TellWithCard(greeting, "Greetings", greeting);
        }
    }
}