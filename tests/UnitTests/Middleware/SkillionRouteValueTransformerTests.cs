using System.Collections.Generic;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using Newtonsoft.Json;
using Skillion;
using Skillion.IO;
using Skillion.Middleware;
using Skillion.Services;
using Xunit;
using RouteData = Skillion.Services.RouteData;

namespace SkillionUnitTests.Middleware
{
    public class SkillionRouteValueTransformerTests
    {
        public class TransformAsync
        {
            public TransformAsync()
            {
                _skillRequestParser = new Mock<ISkillRequestParser>();
                _routeDataService = new Mock<IRouteDataService>();
                _httpContext = new Mock<HttpContext>();
                _httpRequest = new Mock<HttpRequest>();
            }

            private readonly Mock<ISkillRequestParser> _skillRequestParser;
            private readonly Mock<IRouteDataService> _routeDataService;
            private readonly Mock<HttpContext> _httpContext;
            private readonly Mock<HttpRequest> _httpRequest;

            [Fact]
            public async Task ContentTypeIsNotJson_ReturnNull()
            {
                _httpRequest.Setup(x => x.Path).Returns("/");
                _httpRequest.Setup(x => x.Method).Returns("GET");
                _httpRequest.Setup(x => x.ContentType).Returns("application/xml");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);

                var routeValueTransformer =
                    new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);

                Assert.Null(values);
            }

            [Fact]
            public async Task InvalidRequestBody_ReturnNull()
            {
                var skillRequest = new SkillRequest
                {
                    Session = new Session(),
                    Context = new Context(),
                    Request = new IntentRequest {Intent = new Intent {Name = "Test"}}
                };
                var routeData = new RouteData("TestController", "TestAction");

                _skillRequestParser.Setup(x => x.ParseHttpRequestAsync(_httpRequest.Object)).Throws<JsonException>();
                _routeDataService.Setup(x => x.TryGetRoute(skillRequest.Request, out routeData)).Returns(true);
                _httpRequest.Setup(x => x.Path).Returns("/");
                _httpRequest.Setup(x => x.Method).Returns("POST");
                _httpRequest.Setup(x => x.ContentType).Returns("application/json");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
                _httpContext.Setup(x => x.Items).Returns(new Dictionary<object, object>());

                var routeValueTransformer =
                    new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);

                Assert.Null(values);
            }

            [Fact]
            public async Task MethodIsNotPost_ReturnNull()
            {
                _httpRequest.Setup(x => x.Path).Returns("/");
                _httpRequest.Setup(x => x.Method).Returns("GET");
                _httpRequest.Setup(x => x.ContentType).Returns("application/json");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);

                var routeValueTransformer =
                    new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);

                Assert.Null(values);
            }

            [Fact]
            public async Task PathIsNotEmpty_ReturnNull()
            {
                _httpRequest.Setup(x => x.Path).Returns("/test/path");
                _httpRequest.Setup(x => x.Method).Returns("POST");
                _httpRequest.Setup(x => x.ContentType).Returns("application/json");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);

                var routeValueTransformer =
                    new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);

                Assert.Null(values);
            }

            [Fact]
            public async Task RouteExists_ReturnRouteValueDictionary()
            {
                var skillRequest = new SkillRequest
                {
                    Session = new Session(),
                    Context = new Context(),
                    Request = new IntentRequest {Intent = new Intent {Name = "Test"}}
                };
                var routeData = new RouteData("TestController", "TestAction");

                _skillRequestParser.Setup(x => x.ParseHttpRequestAsync(_httpRequest.Object))
                    .ReturnsAsync(skillRequest);
                _routeDataService.Setup(x => x.TryGetRoute(skillRequest.Request, out routeData)).Returns(true);
                _httpRequest.Setup(x => x.Path).Returns("/");
                _httpRequest.Setup(x => x.Method).Returns("POST");
                _httpRequest.Setup(x => x.ContentType).Returns("application/json");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
                _httpContext.Setup(x => x.Items).Returns(new Dictionary<object, object>());

                var routeValueTransformer =
                    new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);

                Assert.True(values["controller"].Equals(routeData.Controller));
                Assert.True(values["action"].Equals(routeData.Action));
            }

            [Fact]
            public async Task RouteNotFound_ReturnNull()
            {
                var skillRequest = new SkillRequest
                {
                    Session = new Session(),
                    Context = new Context(),
                    Request = new IntentRequest {Intent = new Intent {Name = "Test"}}
                };
                var routeData = new RouteData("TestController", "TestAction");

                _skillRequestParser.Setup(x => x.ParseHttpRequestAsync(_httpRequest.Object))
                    .ReturnsAsync(skillRequest);
                _routeDataService.Setup(x => x.TryGetRoute(skillRequest.Request, out routeData)).Returns(false);
                _httpRequest.Setup(x => x.Path).Returns("/");
                _httpRequest.Setup(x => x.Method).Returns("POST");
                _httpRequest.Setup(x => x.ContentType).Returns("application/json");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
                _httpContext.Setup(x => x.Items).Returns(new Dictionary<object, object>());

                var routeValueTransformer =
                    new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);

                Assert.Null(values);
            }
        }
    }
}