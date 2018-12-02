using Microsoft.AspNetCore.Mvc;
using Skillion;
using Skillion.Attributes;
using Skillion.IO;

namespace ManualTest.Controllers
{
    public class ValuesController : ControllerBase
    {
        [Intent("HelloIntent")]
        public IntentActionResult Hello()
        {
            var response = new IntentResponse();
            return response.PlainTextSpeech("Hello, how are you.").SimpleCard("Greetings", "Hello, how are you.");
        }
    }
}