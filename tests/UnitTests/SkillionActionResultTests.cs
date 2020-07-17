using System;
using System.Reflection;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Skillion;
using SkillionUnitTests.Helpers;
using Xunit;

namespace SkillionUnitTests
{
    public class SkillionActionResultTests
    {
        [Fact]
        public void SkillResponseActionResult_SkillResponseValue()
        {
            var skillResponse = new SkillResponse();
            var actionResult = new SkillionActionResult<SkillResponse>(skillResponse);
            
            Assert.Equal(skillResponse, actionResult.Value);
            Assert.IsType<SkillResponse>(actionResult.Value);
        }
        
        [Fact]
        public void SkillionResponseActionResult_IsJsonResult()
        {
            var mapper = new TestSkillionActionResultTypeMapper();
            var controller = new TestController();
            var objectMethodExecutor = GetExecutor(nameof(TestController.ReturnSkillionActionResult));
            var actionMethodExecutor = TestSkillionActionMethodExecutor.GetExecutor(objectMethodExecutor);
            
            var valueTask = actionMethodExecutor.Execute(mapper, objectMethodExecutor, controller, Array.Empty<object>());
            var result = valueTask.Result as JsonResult;
            var resultSerializerSettings = result?.SerializerSettings as JsonSerializerSettings;
            
            Assert.True(valueTask.IsCompleted);
            Assert.IsType<JsonResult>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("application/json; charset=utf-8", result.ContentType);
            Assert.Equal(typeof(CamelCasePropertyNamesContractResolver), resultSerializerSettings?.ContractResolver.GetType());
            Assert.Equal(NullValueHandling.Ignore, resultSerializerSettings?.NullValueHandling);
        }
        
        private static TestSkillionObjectMethodExecutor GetExecutor(string methodName)
        {
            var type = typeof(TestController);
            var methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            Assert.NotNull(methodInfo);
            return TestSkillionObjectMethodExecutor.Create(methodInfo, type.GetTypeInfo());
        }
    }
}