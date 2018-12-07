using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Skillion.IO;

namespace Skillion
{
    public class SkillionActionResult : IConvertToActionResult
    {
        public SkillionActionResult(IntentResponse result)
        {
            Value = result;
        }
        
        public IntentResponse Value { get; }
        
        public static implicit operator SkillionActionResult(IntentResponse value)
        {
            return new SkillionActionResult(value);
        }

        IActionResult IConvertToActionResult.Convert()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            // TODO total 24 kilobytes
            var json = JsonConvert.SerializeObject(Value, settings);
            return new ContentResult { Content = json, ContentType = "application/json", StatusCode = 200 };
        }
    }
}