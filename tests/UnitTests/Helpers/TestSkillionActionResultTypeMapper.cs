using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SkillionUnitTests.Helpers
{
    // https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.Core/src/Infrastructure/ActionResultTypeMapper.cs
    internal class TestSkillionActionResultTypeMapper : IActionResultTypeMapper
    {
        public Type GetResultDataType(Type returnType)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException(nameof(returnType));
            }

            if (returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
            {
                return returnType.GetGenericArguments()[0];
            }

            return returnType;
        }

        public IActionResult Convert(object value, Type returnType)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException(nameof(returnType));
            }

            if (value is IConvertToActionResult converter)
            {
                return converter.Convert();
            }

            return new ObjectResult(value)
            {
                DeclaredType = returnType,
            };
        }
    }
}