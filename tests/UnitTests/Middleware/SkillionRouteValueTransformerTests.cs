using System.Collections.Generic;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Moq;
using Skillion;
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
            private Mock<ISkillRequestParser> _skillRequestParser;
            private Mock<IRouteDataService> _routeDataService;
            private Mock<HttpContext> _httpContext;
            private Mock<HttpRequest> _httpRequest;

            public TransformAsync()
            {
                _skillRequestParser = new Mock<ISkillRequestParser>();
                _routeDataService = new Mock<IRouteDataService>();
                _httpContext = new Mock<HttpContext>();
                _httpRequest = new Mock<HttpRequest>();
            }
            
            [Fact]
            public async Task PathIsNotEmpty_ReturnNull()
            {
                _httpRequest.Setup(x => x.Path).Returns("/test/path");
                _httpRequest.Setup(x => x.Method).Returns("POST");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
                
                var routeValueTransformer = new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);
                
                Assert.Null(values);
            }
            
            [Fact]
            public async Task MethodIsNotPost_ReturnNull()
            {
                _httpRequest.Setup(x => x.Path).Returns("/");
                _httpRequest.Setup(x => x.Method).Returns("GET");
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
                
                var routeValueTransformer = new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);
                
                Assert.Null(values);
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
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
                _httpContext.Setup(x => x.Items).Returns(new Dictionary<object, object>());
                
                var routeValueTransformer = new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
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
                _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
                _httpContext.Setup(x => x.Items).Returns(new Dictionary<object, object>());
                
                var routeValueTransformer = new SkillionRouteValueTransformer(_skillRequestParser.Object, _routeDataService.Object);
                var routeValueDictionary = new RouteValueDictionary();
                var values = await routeValueTransformer.TransformAsync(_httpContext.Object, routeValueDictionary);
                
                Assert.True(values["controller"].Equals(routeData.Controller));
                Assert.True(values["action"].Equals(routeData.Action));
            }
        }
    }
}