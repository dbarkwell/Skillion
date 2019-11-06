using System;
using System.IO;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using Skillion;
using Skillion.IO;
using Xunit;

namespace SkillionUnitTests
{
    public class SkillRequestParserTests
    {
        public class ParseHttpRequestAsync
        {
            [Theory]
            [InlineData(null)]
            [InlineData(0)]
            [InlineData(-100)]
            public async Task RequestContentLengthIsInvalid_ThrowsInvalidDataException(long? length)
            {
                var httpRequest = new Mock<HttpRequest>();
                httpRequest.Setup(x => x.ContentLength).Returns(length);
                var exception = await Assert.ThrowsAsync<InvalidDataException>(() => GetSkillRequestParser().ParseHttpRequestAsync(httpRequest.Object).AsTask());
                Assert.Equal("Request body is missing.", exception.Message);
            }

            [Fact]
            public async Task RequestContentLengthIsLarge_ThrowsInvalidDataException()
            {
                var httpRequest = new Mock<HttpRequest>();
                httpRequest.Setup(x => x.ContentLength).Returns(1024 * 1024);
                var exception = await Assert.ThrowsAsync<InvalidDataException>(() => GetSkillRequestParser().ParseHttpRequestAsync(httpRequest.Object).AsTask());
                Assert.Equal("Request body is too large.", exception.Message);
            }
            
            [Fact]
            public async Task AlwaysValidateSkillRequestAndSkillIdEmpty_ThrowsException()
            {
                var httpRequest = new Mock<HttpRequest>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true});
                
                var exception = await Assert.ThrowsAsync<Exception>(() => GetSkillRequestParser(options: options).ParseHttpRequestAsync(httpRequest.Object).AsTask());
                Assert.Equal("Unable to process request. Always validate skill id is true and skill id is missing", exception.Message);
            }
            
            [Fact]
            public async Task IsDevelopmentAndNotAlwaysValidateSkillRequest_ReturnsSkillRequest()
            {
                var httpRequest = new Mock<HttpRequest>();
                var webHostEnv = new Mock<IWebHostEnvironment>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                var skillRequest = new SkillRequest { Session = new Session(), Context = new Context(), Request = new IntentRequest {Type = "IntentRequest"}, Version = "1.0"};
                var skillRequestJson = JsonConvert.SerializeObject(skillRequest);
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                httpRequest.Setup(x => x.Body).Returns(GetHttpRequestBody(skillRequestJson));
                webHostEnv.Setup(x => x.EnvironmentName).Returns("Development");
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = false});

                var skill = await GetSkillRequestParser(webHostEnv, options).ParseHttpRequestAsync(httpRequest.Object);
                
                Assert.Equal(skillRequestJson, JsonConvert.SerializeObject(skill));
            }
            
            [Fact]
            public async Task ApplicationIdAndSkillIdNotEqual_ThrowsException()
            {
                var httpRequest = new Mock<HttpRequest>();
                var webHostEnv = new Mock<IWebHostEnvironment>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                var skillRequest = new SkillRequest
                {
                    Session = new Session(), 
                    Context = new Context
                    {
                        System = new AlexaSystem { Application = new Application { ApplicationId = "ABC123"}}
                    }, 
                    Request = new IntentRequest {Type = "IntentRequest"}, Version = "1.0"
                };
                var skillRequestJson = JsonConvert.SerializeObject(skillRequest);
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                httpRequest.Setup(x => x.Body).Returns(GetHttpRequestBody(skillRequestJson));
                webHostEnv.Setup(x => x.EnvironmentName).Returns("Production");
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true, SkillId = "XYZ123"});

                var exception = await Assert.ThrowsAsync<Exception>(() => GetSkillRequestParser(webHostEnv, options).ParseHttpRequestAsync(httpRequest.Object).AsTask());
                Assert.Equal("Invalid application id.", exception.Message);
            }
            
            [Fact]
            public async Task InvalidRequestMissingSignatureCertChainUrlHeader_ThrowsException()
            {
                var httpRequest = new Mock<HttpRequest>();
                var webHostEnv = new Mock<IWebHostEnvironment>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                var skillRequest = new SkillRequest
                {
                    Session = new Session(), 
                    Context = new Context
                    {
                        System = new AlexaSystem { Application = new Application { ApplicationId = "ABC123"}}
                    }, 
                    Request = new IntentRequest {Type = "IntentRequest"}, Version = "1.0"
                };
                var skillRequestJson = JsonConvert.SerializeObject(skillRequest);
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                httpRequest.Setup(x => x.Body).Returns(GetHttpRequestBody(skillRequestJson));
                StringValues signatureCertChainUrl;
                httpRequest.Setup(x => x.Headers.TryGetValue("SignatureCertChainUrl", out signatureCertChainUrl)).Returns(false).Verifiable();
                webHostEnv.Setup(x => x.EnvironmentName).Returns("Production");
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true, SkillId = "ABC123"});

                var exception = await Assert.ThrowsAsync<Exception>(() => GetSkillRequestParser(webHostEnv, options).ParseHttpRequestAsync(httpRequest.Object).AsTask());

                httpRequest.Verify(x => x.Headers.TryGetValue("SignatureCertChainUrl", out signatureCertChainUrl));
                
                Assert.Equal("Invalid request. Check timestamp or signature.", exception.Message);
            }
            
            [Fact]
            public async Task InvalidRequestMissingSignatureHeader_ThrowsException()
            {
                var httpRequest = new Mock<HttpRequest>();
                var webHostEnv = new Mock<IWebHostEnvironment>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                var skillRequest = new SkillRequest
                {
                    Session = new Session(), 
                    Context = new Context
                    {
                        System = new AlexaSystem { Application = new Application {ApplicationId = "ABC123"}}
                    }, 
                    Request = new IntentRequest {Type = "IntentRequest"}, Version = "1.0"
                };
                var skillRequestJson = JsonConvert.SerializeObject(skillRequest);
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                httpRequest.Setup(x => x.Body).Returns(GetHttpRequestBody(skillRequestJson));
                StringValues signatureCertChainUrl = "http://example.com";
                httpRequest.Setup(x => x.Headers.TryGetValue("SignatureCertChainUrl", out signatureCertChainUrl)).Returns(true);
                StringValues signature;
                httpRequest.Setup(x => x.Headers.TryGetValue("Signature", out signature)).Returns(false).Verifiable();
                webHostEnv.Setup(x => x.EnvironmentName).Returns("Production");
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true, SkillId = "ABC123"});

                var exception = await Assert.ThrowsAsync<Exception>(() => GetSkillRequestParser(webHostEnv, options).ParseHttpRequestAsync(httpRequest.Object).AsTask());
                
                httpRequest.Verify(x => x.Headers.TryGetValue("Signature", out signature));
                
                Assert.Equal("Invalid request. Check timestamp or signature.", exception.Message);
            }
            
            [Fact]
            public async Task InvalidRequestTimestamp_ThrowsException()
            {
                var httpRequest = new Mock<HttpRequest>();
                var webHostEnv = new Mock<IWebHostEnvironment>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                var requestValidator = new Mock<ISkillRequestValidator>();
                var skillRequest = new SkillRequest
                {
                    Session = new Session(), 
                    Context = new Context
                    {
                        System = new AlexaSystem { Application = new Application {ApplicationId = "ABC123"}}
                    }, 
                    Request = new IntentRequest {Type = "IntentRequest"}, Version = "1.0"
                };
                var skillRequestJson = JsonConvert.SerializeObject(skillRequest);
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                httpRequest.Setup(x => x.Body).Returns(GetHttpRequestBody(skillRequestJson));
                httpRequest.Setup(x => x.Headers).Returns(new HeaderDictionary {{"SignatureCertChainUrl", "http://example.com"},{"Signature", "abc"}});
                webHostEnv.Setup(x => x.EnvironmentName).Returns("Production");
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true, SkillId = "ABC123"});
                requestValidator.Setup(x => x.IsTimestampValid(It.IsAny<SkillRequest>())).Returns(false).Verifiable();
                requestValidator.Setup(x => x.IsRequestValidAsync(It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>())).ReturnsAsync(true);
                
                var exception = await Assert.ThrowsAsync<Exception>(() => GetSkillRequestParser(webHostEnv, options, requestValidator).ParseHttpRequestAsync(httpRequest.Object).AsTask());
                
                requestValidator.Verify(x => x.IsTimestampValid(It.IsAny<SkillRequest>()));
                Assert.Equal("Invalid request. Check timestamp or signature.", exception.Message);
            }
            
            [Fact]
            public async Task InvalidRequest_ThrowsException()
            {
                var httpRequest = new Mock<HttpRequest>();
                var webHostEnv = new Mock<IWebHostEnvironment>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                var requestValidator = new Mock<ISkillRequestValidator>();
                var skillRequest = new SkillRequest
                {
                    Session = new Session(), 
                    Context = new Context
                    {
                        System = new AlexaSystem { Application = new Application {ApplicationId = "ABC123"}}
                    }, 
                    Request = new IntentRequest {Type = "IntentRequest"}, Version = "1.0"
                };
                var skillRequestJson = JsonConvert.SerializeObject(skillRequest);
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                httpRequest.Setup(x => x.Body).Returns(GetHttpRequestBody(skillRequestJson));
                httpRequest.Setup(x => x.Headers).Returns(new HeaderDictionary {{"SignatureCertChainUrl", "http://example.com"},{"Signature", "abc"}});
                webHostEnv.Setup(x => x.EnvironmentName).Returns("Production");
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true, SkillId = "ABC123"});
                requestValidator.Setup(x => x.IsTimestampValid(It.IsAny<SkillRequest>())).Returns(true);
                requestValidator.Setup(x => x.IsRequestValidAsync(It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>())).ReturnsAsync(false).Verifiable();
                
                var exception = await Assert.ThrowsAsync<Exception>(() => GetSkillRequestParser(webHostEnv, options, requestValidator).ParseHttpRequestAsync(httpRequest.Object).AsTask());

                requestValidator.Verify(x =>
                    x.IsRequestValidAsync(It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()));
                    
                Assert.Equal("Invalid request. Check timestamp or signature.", exception.Message);
            }
            
            [Fact]
            public async Task ValidRequest_ReturnsSkillRequest()
            {
                var httpRequest = new Mock<HttpRequest>();
                var webHostEnv = new Mock<IWebHostEnvironment>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                var requestValidator = new Mock<ISkillRequestValidator>();
                var skillRequest = new SkillRequest
                {
                    Session = new Session(), 
                    Context = new Context
                    {
                        System = new AlexaSystem { Application = new Application {ApplicationId = "ABC123"}}
                    }, 
                    Request = new IntentRequest {Type = "IntentRequest"}, Version = "1.0"
                };
                var skillRequestJson = JsonConvert.SerializeObject(skillRequest);
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                httpRequest.Setup(x => x.Body).Returns(GetHttpRequestBody(skillRequestJson));
                httpRequest.Setup(x => x.Headers).Returns(new HeaderDictionary {{"SignatureCertChainUrl", "http://example.com"},{"Signature", "abc"}});
                webHostEnv.Setup(x => x.EnvironmentName).Returns("Production");
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true, SkillId = "ABC123"});
                requestValidator.Setup(x => x.IsTimestampValid(It.IsAny<SkillRequest>())).Returns(true);
                requestValidator.Setup(x => x.IsRequestValidAsync(It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>())).ReturnsAsync(true);
                
                var skill = await GetSkillRequestParser(webHostEnv, options, requestValidator).ParseHttpRequestAsync(httpRequest.Object);
                
                Assert.Equal(skillRequestJson, JsonConvert.SerializeObject(skill));
            }
            
            private static SkillRequestParser GetSkillRequestParser(
                Mock<IWebHostEnvironment> webHostEnv = null, 
                Mock<IOptions<SkillionConfiguration>> options = null,
                Mock<ISkillRequestValidator> requestValidator = null)
            {
                if (webHostEnv == null)
                    webHostEnv = new Mock<IWebHostEnvironment>();
                
                if (options == null)
                    options = new Mock<IOptions<SkillionConfiguration>>();

                if (requestValidator == null)
                    requestValidator = new Mock<ISkillRequestValidator>();
                
                return new SkillRequestParser(webHostEnv.Object, options.Object, requestValidator.Object);
            }

            private static Stream GetHttpRequestBody(string skillRequest)
            {
                var memStream = new MemoryStream();
                var streamWriter = new StreamWriter(memStream);
                
                streamWriter.Write(skillRequest);
                streamWriter.Flush();
                
                memStream.Position = 0;
                return memStream;
            }
        }
    }
}