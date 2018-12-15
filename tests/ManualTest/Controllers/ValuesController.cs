using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Skillion;
using Skillion.Attributes;
using Skillion.IO;

namespace ManualTest.Controllers
{
    public class ValuesController : SkillionController
    {
        [Intent("HelloIntent")]
        public SkillionActionResult<IntentResponse> Hello()
        {
            var response = new IntentResponse();
            var numberOfItems = StandardRequest.Intent.Slots != null ? StandardRequest.Intent.Slots["NumberOfItems"].Value : "0";
            
            return response.PlainTextSpeech($"Here are your {numberOfItems} items").SimpleCard("Items", $"Here are your {numberOfItems} items");
        }
        
        [Intent("SessionIntent")]
        public SkillionActionResult<IntentResponse> Hello2([FromBody] IDictionary<string, object> session)
        {
            var response = new IntentResponse
            {
                SessionAttributes = new Dictionary<string, object> {{"name", session["name"]}}
            };
            return response
                .PlainTextSpeech($"Hello {session["name"]}, how are you?")
                .SimpleCard("Greetings", $"Hello {session["name"]}, how are you?");
        }
        
        [Intent("DialogIntent")]
        public SkillionActionResult<DialogDirectiveResponse> Dialog()
        {
            var response = new DialogDirectiveResponse();
            var intent = StandardRequest.Intent;
            intent.Slots["fromCity"].Value = "Toronto";
            response.UpdateIntent(intent);
            
            return response;
        }
        
        [Launch]
        public SkillionActionResult<IntentResponse> Welcome()
        {
            var response = new IntentResponse();
            return response.PlainTextSpeech("Welcome to my skill. Ask some questions.").SimpleCard("Welcome", "Welcome to my skill. Ask some questions.");
        }
        
        [SessionEnded]
        public void End()
        {
            var reason = StandardRequest.Reason;
        }
    }
}