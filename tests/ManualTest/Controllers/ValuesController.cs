using Microsoft.AspNetCore.Mvc;
using Skillion;
using Skillion.Attributes;
using Skillion.IO;

namespace ManualTest.Controllers
{
    public class ValuesController : ControllerBase
    {
        [Intent("HelloIntent")]
        public SkillionActionResult Hello()
        {
            var response = new IntentResponse();
            return response.PlainTextSpeech("Hello, how are you?").SimpleCard("Greetings", "Hello, how are you?");
        }
        
        [Launch]
        public SkillionActionResult Welcome()
        {
            var response = new IntentResponse();
            return response.PlainTextSpeech("Welcome to my skill. Ask some questions.").SimpleCard("Welcome", "Welcome to my skill. Ask some questions.");
        }
    }
}