using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexa.NET.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Skillion.Middleware;

namespace Skillion.Ssml
{
    public class SsmlBuilder
    {
        private const string StartElement = "<speak>";
        private const string EndElement = "</speak>";
        private readonly StringBuilder _builder;
        private readonly ILogger _logger;
        
        public SsmlBuilder(ILogger<SsmlBuilder> logger = null)
        {
            _builder = new StringBuilder();
            _logger = logger;
        }

        public SsmlBuilder Effect(string text)
        {
            _builder.Append(CreateElement(
                "amazon:effect", 
                "name", 
                "whispered", 
                text));
            
            return this;
        }

        public SsmlBuilder Audio(Uri url)
        {
            _builder.Append(CreateElement(
                "audio",
                "src", 
                url.ToString()));

            return this;
        }

        public SsmlBuilder Break(TimeSpan time)
        {
            return Break(SsmlAttributes.Strength.Medium, time);
        }

        public SsmlBuilder Break(string strength, TimeSpan time)
        {
            if (!SsmlAttributes.Strength.TryValidate(strength, out var validStrength))
            {
                _logger?.LogWarning($"{strength} is not a valid strength. Defaulting to {validStrength}");
            }

            if (time.Seconds > 10 || time.TotalMilliseconds > 10000)
            {
                _logger?.LogWarning($"Maximum time for a break is 10s or 10000ms. Defaulting to 10s.");
                time = new TimeSpan(0, 0, 10);
            }

            var timeValue = time.Milliseconds == 0
                ? $"{time.Seconds.ToString()}s"
                : $"{time.Milliseconds.ToString()}ms";
            
            var attr = new Dictionary<string, string>
            {
                ["strength"] = validStrength, 
                ["time"] = timeValue
            };
            
            _builder.Append(CreateElement("break", attr));
            return this;
        }

        public SsmlBuilder Emphasis(string text, string level)
        {
            if (!SsmlAttributes.Level.TryValidate(level, out var validLevel))
            {
                _logger?.LogWarning($"{level} is not a valid level. Defaulting to {validLevel}.");
            }
            
            _builder.Append(CreateElement("emphasis", "level", validLevel, text));
            return this;
        }
        
        // TODO Lang
        
        public SsmlBuilder Paragraph(string text)
        {
            _builder.Append(CreateElement("p", text));
            return this;
        }

        public SsmlBuilder Phoneme(string text, string alphabet, string ph)
        {
            var attributes = new Dictionary<string, string>()
            {
                {"alphabet", alphabet},
                {"ph", ph}
            };

            _builder.Append(CreateElement("phoneme", attributes, text));
            return this;
        }
        
        // TODO Prosody
        // TODO Say-as
        // TODO Sub
        // TODO Voice
        // TODO W

        public SsmlBuilder S(string text)
        {
            _builder.Append(CreateElement("s", text));
            return this;
        }
        
        public SsmlBuilder Text(string text)
        {
            _builder.Append(text);
            return this;
        }

        public SsmlOutputSpeech Build()
        {
            _builder.Insert(0, StartElement);
            _builder.Append(EndElement);
            return new SsmlOutputSpeech(Raw());
        }

        internal string Raw()
        {
            return _builder.ToString();      
        }
        
        private static string CreateElement(string element, string text)
        {
            return $"<{element}>{text}</{element}>";
        }
        
        private static string CreateElement(string element, string attributeName, string attributeValue, string text = "")
        {
            return CreateElement(element, new Dictionary<string, string> {{attributeName, attributeValue}}, text);
        }
        
        private static string CreateElement(string element, IDictionary<string, string> attributes, string text = "")
        {
            var attr = string.Join(" ", attributes.Select(a => $"{a.Key}='{a.Value}'"));
            return string.IsNullOrWhiteSpace(text) ? 
                $"<{element} {attr} />" :
                $"<{element} {attr}>{text}</{element}>";
        }
    }
}