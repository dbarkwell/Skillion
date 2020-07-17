using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Skillion;
using Skillion.Attributes;
using SkillionUnitTests.Helpers;
using Xunit;

namespace SkillionUnitTests
{
    public class RouteMapperTests
    {
        [Fact]
        public void GivenControllerAction_WhenHasAttribute_ReturnRoutes()
        {
            const string intentName = "TestIntent";
            const string controllerName = "Test";
            const string methodName = "TestAction";
            
            var controllerNamespace = $"RouteMapperTests.Controllers.{controllerName}Controller";

            var type = new Mock<Type>();
            type.Setup(t => t.FullName).Returns(controllerNamespace);
            var testMemberInfo = new TestMemberInfo(type.Object, methodName, new SkillionRequestAttribute[] {new IntentRequestAttribute(intentName)});

            var routes = RouteMapper.MapRoutes(new List<MemberInfo> {testMemberInfo});
            
            Assert.True(routes.ContainsKey(intentName));
            Assert.Equal(controllerName, routes[intentName].Controller);
        }
        
        [Fact]
        public void GivenControllerAction_WhenDoesNotHaveAttribute_ReturnEmptyRoutes()
        {
            const string intentName = "TestIntent";
            const string controllerName = "Test";
            const string methodName = "TestAction";
            
            var controllerNamespace = $"RouteMapperTests.Controllers.{controllerName}Controller";

            var type = new Mock<Type>();
            type.Setup(t => t.FullName).Returns(controllerNamespace);
            var testMemberInfo = new TestMemberInfo(type.Object, methodName, null);

            var routes = RouteMapper.MapRoutes(new List<MemberInfo> {testMemberInfo});
            
            Assert.Empty(routes);
        }
        
        [Fact]
        public void GivenControllerAction_WhenHasAttributeAndAsyncSuffix_ReturnRoutes()
        {
            const string intentName = "TestIntent";
            const string controllerName = "Test";
            const string methodName = "TestActionAsync";
            const string truncatedMethodName = "TestAction";
            
            var controllerNamespace = $"RouteMapperTests.Controllers.{controllerName}Controller";

            var type = new Mock<Type>();
            type.Setup(t => t.FullName).Returns(controllerNamespace);
            var testMemberInfo = new TestMemberInfo(type.Object, methodName, new SkillionRequestAttribute[] {new IntentRequestAttribute(intentName)});

            var routes = RouteMapper.MapRoutes(new List<MemberInfo> {testMemberInfo});
            
            Assert.True(routes.ContainsKey(intentName));
            Assert.Equal(controllerName, routes[intentName].Controller);
            Assert.Equal(truncatedMethodName, routes[intentName].Action);
        }
        
        [Fact]
        public void GivenControllerAction_WhenHasAttributeAndDoubleAsyncSuffix_ReturnRoutes()
        {
            const string intentName = "TestIntent";
            const string controllerName = "Test";
            const string methodName = "TestAsyncAsync";
            const string truncatedMethodName = "TestAsync";
            
            var controllerNamespace = $"RouteMapperTests.Controllers.{controllerName}Controller";

            var type = new Mock<Type>();
            type.Setup(t => t.FullName).Returns(controllerNamespace);
            var testMemberInfo = new TestMemberInfo(type.Object, methodName, new SkillionRequestAttribute[] {new IntentRequestAttribute(intentName)});

            var routes = RouteMapper.MapRoutes(new List<MemberInfo> {testMemberInfo});
            
            Assert.True(routes.ContainsKey(intentName));
            Assert.Equal(controllerName, routes[intentName].Controller);
            Assert.Equal(truncatedMethodName, routes[intentName].Action);
        }
    }
}