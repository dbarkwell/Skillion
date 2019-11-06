using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Skillion
{
    public class SkillionActionResult<T> : IConvertToActionResult
    {
        public SkillionActionResult(T result)
        {
            Value = result;
        }

        public T Value { get; }

        IActionResult IConvertToActionResult.Convert()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            // TODO total 24 kilobytes
            var json = JsonConvert.SerializeObject(Value, settings);
            return new ContentResult
                {Content = json, ContentType = "application/json; charset=utf-8", StatusCode = 200};
        }

        public static implicit operator SkillionActionResult<T>(T value)
        {
            return new SkillionActionResult<T>(value);
        }
    }
}