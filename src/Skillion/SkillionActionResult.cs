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

            return new JsonResult(Value, settings)
            {
                StatusCode = 200,
                ContentType = "application/json; charset=utf-8"
            };
        }

        public static implicit operator SkillionActionResult<T>(T value)
        {
            return new SkillionActionResult<T>(value);
        }
    }
}