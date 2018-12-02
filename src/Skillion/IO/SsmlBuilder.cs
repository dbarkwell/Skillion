using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Skillion.IO
{
    public class SsmlBuilder
    {
        private const string StartElement = "<speak>";
        private const string EndElement = "</speak>";
        private readonly StringBuilder _builder;
        
        public SsmlBuilder()
        {
            _builder = new StringBuilder();
        }

        public SsmlBuilder Effect(SsmlEnum.AmazonEffect effect, string text)
        {
            _builder.Append(CreateElement(
                "amazon:effect", 
                "name", 
                effect.ToString().ToLower(), 
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

        public SsmlBuilder Break(uint time, bool isSeconds = true)
        {
            return Break(SsmlEnum.Strength.Medium, time, isSeconds);
        }

        public SsmlBuilder Break(SsmlEnum.Strength strength, uint time, bool isSeconds = true)
        {
            if (isSeconds && time > 10)
                throw new Exception("Maximum time in seconds is 10.");
            
            if (!isSeconds && time > 10000)
                throw new Exception("Maximum time in milliseconds is 10000.");
            
            var unitOfTime = isSeconds ? "s" : "ms";
            var attr = new Dictionary<string, string>
            {
                ["strength"] = strength.ToString().ToLower().Replace("_", "-"), 
                ["time"] = $"{time}{unitOfTime}"
            };
            
            _builder.Append(CreateElement("break", attr));
            return this;
        }
        
        // TODO Emphasis
        // TODO Lang
        
        public SsmlBuilder Paragraph(string text)
        {
            _builder.Append(CreateElement("p", text));
            return this;
        }
        
        // TODO Phoneme
        // TODO Prosody
        // TODO S
        // TODO Say-as
        // TODO Speak
        // TODO Sub
        // TODO Voice
        // TODO W
        
        public SsmlBuilder Text(string text)
        {
            _builder.Append(text);
            return this;
        }

        public string Build()
        {
            _builder.Insert(0, StartElement);
            _builder.Append(EndElement);
            return Raw();
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