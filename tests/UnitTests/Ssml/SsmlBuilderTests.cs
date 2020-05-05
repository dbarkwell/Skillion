using System;
using Skillion.Ssml;

using Xunit;

namespace SkillionUnitTests.Ssml
{
    public class SsmlBuilderTests
    {
        public class Effect
        {
            [Fact]
            public void WhenAddEffect_ReturnsElement()
            {
                var effectElement = "<amazon:effect name='whispered'>I am not a real human.</amazon:effect>";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Effect("I am not a real human.");
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(effectElement, build);
            }
        }
        
        public class Audio
        {
            [Fact]
            public void WhenAddAudio_ReturnsElement()
            {
                var audioElement = "<audio src='soundbank://soundlibrary/transportation/amzn_sfx_car_accelerate_01' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Audio(new Uri("soundbank://soundlibrary/transportation/amzn_sfx_car_accelerate_01"));
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(audioElement, build);
            }
        }
        
        public class Break
        {
            [Fact]
            public void WhenAddBreak_ReturnsElementInSeconds()
            {
                var breakElement = "<break strength='strong' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlAttributes.Strength.Strong, new TimeSpan(0, 0, 3));
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
            }
            
            [Fact]
            public void WhenAddBreak_WithDefaultTimeUnit_ReturnsElementInSeconds()
            {
                var breakElement = "<break strength='strong' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlAttributes.Strength.Strong, new TimeSpan(0, 0, 3));
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
                
            }
            
            [Fact]
            public void WhenAddBreak_WithOnlyTime_ReturnsElementMedium()
            {
                var breakElement = "<break strength='medium' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(new TimeSpan(0, 0, 3));
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
                
            }
            
            [Fact]
            public void WhenAddBreak_WithXStrong_ReturnsElementXStrong()
            {
                var breakElement = "<break strength='x-strong' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlAttributes.Strength.XStrong, new TimeSpan(0, 0, 3));
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
                
            }
            
            /*[Fact]
            public void WhenAddBreak_WithTimeInSecondsGreaterThan10_ThrowsException()
            {
                var ssmlBuilder = new SsmlBuilder();
                Assert.Throws<Exception>(() => ssmlBuilder.Break(15)); 
            }
            
            [Fact]
            public void WhenAddBreak_WithTimeInMilliSecondsGreaterThan10000_ThrowsException()
            {
                var ssmlBuilder = new SsmlBuilder();
                Assert.Throws<Exception>(() => ssmlBuilder.Break(15000, false)); 
            }*/
            
            [Fact]
            public void WhenAddBreakWithMilliseconds_ReturnsBreakElementInMilliSeconds()
            {
                var breakElement = "<break strength='strong' time='3ms' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlAttributes.Strength.Strong, new TimeSpan(0, 0, 0, 0, 3));
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
            }
        }

        public class Paragraph
        {
            [Fact]
            public void WhenAddParagraph_ReturnsElement()
            {
                var paragraphElement = "<p>This is the second paragraph.</p>";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Paragraph("This is the second paragraph.");
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(paragraphElement, build);
            }
        }

        public class Emphasis
        {
            [Fact]
            public void WhenValidLevel_ReturnElement()
            {
                var emphasisElement = "<emphasis level='moderate'>This is a test.</emphasis>";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Emphasis("This is a test.", SsmlAttributes.Level.Moderate);
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(emphasisElement, build);
            }
            
            [Fact]
            public void WhenInValidLevel_ReturnElementWithModerateLevel()
            {
                var emphasisElement = "<emphasis level='moderate'>This is a test.</emphasis>";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Emphasis("This is a test.", "abc123");
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(emphasisElement, build);
            }
        }
    }
}