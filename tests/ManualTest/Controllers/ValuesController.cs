﻿using System;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Skillion;
using Skillion.Attributes;

namespace ManualTest.Controllers
{
    public class ValuesController : SkillionController
    {
        [IntentRequest("HelloIntent")]
        public Task<SkillionActionResult<SkillResponse>> HelloAsyncAsync()
        {
            if (!TryCastRequest<IntentRequest>(out var intent))
                return Task.FromResult<SkillionActionResult<SkillResponse>>(ResponseBuilder.TellWithCard("Something went wrong", "Error", "Something went wrong"));

            var numberOfItems = intent.Intent.Slots != null
                ? ((IntentRequest) RequestContext).Intent.Slots["NumberOfItems"].Value
                : "0";

            var text = $"Here are your {numberOfItems} items";
            return Task.FromResult<SkillionActionResult<SkillResponse>>(ResponseBuilder.TellWithCard(text, "Your items", text));
        }

        [IntentRequest("SessionIntent")]
        public SkillionActionResult<SkillResponse> Hello2()
        {
            var greeting = $"Hello {SessionContext.Attributes["name"]}, how are you?";
            return ResponseBuilder.TellWithCard(greeting, "Greetings", greeting);
        }

        [IntentRequest("DialogIntent")]
        public SkillionActionResult<SkillResponse> Dialog()
        {
            var intentRequest = RequestContext as IntentRequest;
            intentRequest?.Intent.Slots.Add("fromCity", new Slot {Name = "fromCity", Value = "Toronto"});
            return ResponseBuilder.DialogDelegate(intentRequest?.Intent);
        }

        [IntentRequest("AskIntent")]
        public SkillionActionResult<SkillResponse> Ask()
        {
            return ResponseBuilder.Ask("Hello",
                new Reprompt {OutputSpeech = new PlainTextOutputSpeech {Text = "Is it me you're looking for?"}});
        }

        [LaunchRequest]
        public SkillionActionResult<SkillResponse> Welcome()
        {
            const string response = "Welcome to my skill. Ask some questions.";
            return ResponseBuilder.TellWithCard(response, "Welcome", response);
        }

        [SessionEndedRequest]
        public void End()
        {
            Console.WriteLine(RequestContext.RequestId);
        }
    }
}