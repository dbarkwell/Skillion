using System;
using System.Collections.Generic;
using System.Linq;

namespace Skillion.IO
{
    public class IntentResponse : StandardResponse<Response>
    {
        public IntentResponse()
        {
            Response = new Response();
        }
        
        public override Response Response { get;}
        
        public IntentResponse PlainTextSpeech(string text)
        {
            var outSpeech = new OutputSpeech
            {
                Type = "PlainText",
                Text = text
            };

            Response.OutputSpeech = outSpeech;
            return this;
        }
        
        public IntentResponse SsmlSpeech(SsmlBuilder builder)
        {
            var outSpeech = new OutputSpeech
            {
                Type = "SSML",
                Text = builder.Build()
            };

            Response.OutputSpeech = outSpeech;
            return this;
        }
        
        public IntentResponse SimpleCard(string title, string content)
        {
            var card = new Card
            {
                Type = "Simple",
                Title = title,
                Content = content
            };

            Response.Card = card;
            return this;
        }
        
        public IntentResponse Directives(IEnumerable<Stream> streams, bool? shouldEndSession = null)
        {
            var directives = streams.Select(s => CreateDirective(s.Token, s.Url.ToString(), s.OffsetInMilliseconds));
            Response.Directives = directives;
            Response.ShouldEndSession = shouldEndSession;
            return this;
        }
        
        public IntentResponse Directive(string token, Uri url, long offsetInMilliseconds, bool? shouldEndSession = null)
        {
            Response.Directives = new[] { CreateDirective(token, url.ToString(), offsetInMilliseconds) };
            Response.ShouldEndSession = shouldEndSession;
            return this;
        }

        private static StandardDirective CreateDirective(string token, string url, long offsetInMilliseconds)
        {
            var stream = new Stream
            {
                Token = token,
                Url = url,
                OffsetInMilliseconds = offsetInMilliseconds
            };

            var audioItem = new AudioItem
            {
                Stream = stream
            };
            
            var directive = new StandardDirective()
            {
                Type = "AudioPlayer.Play",
                PlayBehavior = "REPLACE_ALL",
                AudioItem = audioItem
            };

            return directive;
        }
    }
}