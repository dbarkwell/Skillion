using System;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Skillion;
using Skillion.Attributes;

namespace ManualTest.Controllers
{
    public class ValuesController : SkillionController
    {
        [FallbackIntentRequest]
        public SkillionActionResult<SkillResponse> Fallback()
        {
            return Hello();
        }
        
        [IntentRequest("HelloIntent")]
        public SkillionActionResult<SkillResponse> Hello()
        {
            var numberOfItems = ((IntentRequest)RequestContext).Intent.Slots != null ? 
                ((IntentRequest)RequestContext).Intent.Slots["NumberOfItems"].Value : 
                "0";

            var text = $"Here are your {numberOfItems} items";
            return ResponseBuilder.TellWithCard(text, "Your items", text);
        }
/*
        [Intent("SessionIntent")]
        public SkillionActionResult<Response> Hello2([FromBody] IDictionary<string, object> session)
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
            var response = new DialogDelegateResponse(StandardRequest.Intent);
            response.UpdateSlot("fromCity", "Toronto");
            
            return response;
        }
        
        [Launch]
        public SkillionActionResult<IntentResponse> Welcome()
        {
            var response = new IntentResponse();
            return response.PlainTextSpeech("Welcome to my skill. Ask some questions.").SimpleCard("Welcome", "Welcome to my skill. Ask some questions.");
        }
        */

        [SessionEndedRequest]
        public void End()
        {
            Console.WriteLine(RequestContext.RequestId);
        }
    }
}