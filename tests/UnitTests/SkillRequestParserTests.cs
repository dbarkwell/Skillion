using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Skillion;
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
            public void RequestContentLengthIsInvalid_ShouldThrowInvalidDataException(long? length)
            {
                var httpRequest = new Mock<HttpRequest>();
                httpRequest.Setup(x => x.ContentLength).Returns(length);
                Assert.ThrowsAsync<InvalidDataException>(() => GetSkillRequestParser().ParseHttpRequestAsync(httpRequest.Object).AsTask());
            }

            [Fact]
            public void RequestContentLengthIsLarge_ShouldThrowInvalidDataException()
            {
                var httpRequest = new Mock<HttpRequest>();
                httpRequest.Setup(x => x.ContentLength).Returns(1024 * 1024 * 2);
                Assert.ThrowsAsync<InvalidDataException>(() => GetSkillRequestParser().ParseHttpRequestAsync(httpRequest.Object).AsTask());
            }
            
            [Fact]
            public void AlwaysValidateSkillRequest_SkillIdEmpty_ShouldThrowException()
            {
                var httpRequest = new Mock<HttpRequest>();
                var options = new Mock<IOptions<SkillionConfiguration>>();
                
                httpRequest.Setup(x => x.ContentLength).Returns(1024);
                options.Setup(x => x.Value).Returns(new SkillionConfiguration {AlwaysValidateSkillRequest = true});
                
                Assert.ThrowsAsync<Exception>(() => GetSkillRequestParser(options).ParseHttpRequestAsync(httpRequest.Object).AsTask());
            }
        
            private static SkillRequestParser GetSkillRequestParser(Mock<IOptions<SkillionConfiguration>> options = null)
            {
                var webHostEnv = new Mock<IWebHostEnvironment>();
                if (options == null)
                    options = new Mock<IOptions<SkillionConfiguration>>();
                
                return new SkillRequestParser(webHostEnv.Object, options.Object);
            }
        }
    }
}