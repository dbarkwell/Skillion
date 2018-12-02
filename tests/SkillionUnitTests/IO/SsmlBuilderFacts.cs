using System;
using Skillion.IO;

using Xunit;

namespace SkillionUnitTests.IO
{
    public class SsmlBuilderFacts
    {
        public class EffectFacts
        {
            [Fact]
            public void WhenAddEffect_ReturnsElement()
            {
                var effectElement = "<amazon:effect name='whispered'>I am not a real human.</amazon:effect>";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Effect(SsmlEnum.AmazonEffect.Whispered, "I am not a real human.");
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(effectElement, build);
            }
        }
        
        public class AudioFacts
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
        
        public class BreakFacts
        {
            [Fact]
            public void WhenAddBreak_ReturnsElementInSeconds()
            {
                var breakElement = "<break strength='strong' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlEnum.Strength.Strong, 3, true);
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
            }
            
            [Fact]
            public void WhenAddBreak_WithDefaultTimeUnit_ReturnsElementInSeconds()
            {
                var breakElement = "<break strength='strong' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlEnum.Strength.Strong, 3);
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
                
            }
            
            [Fact]
            public void WhenAddBreak_WithOnlyTime_ReturnsElementMedium()
            {
                var breakElement = "<break strength='medium' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(3);
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
                
            }
            
            [Fact]
            public void WhenAddBreak_WithXStrong_ReturnsElementXStrong()
            {
                var breakElement = "<break strength='x-strong' time='3s' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlEnum.Strength.X_Strong, 3);
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
                
            }
            
            [Fact]
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
            }
            
            [Fact]
            public void WhenAddBreakWithMilliseconds_ReturnsBreakElementInMilliSeconds()
            {
                var breakElement = "<break strength='strong' time='3ms' />";
                var ssmlBuilder = new SsmlBuilder();
                ssmlBuilder.Break(SsmlEnum.Strength.Strong, 3, false);
                var build = ssmlBuilder.Raw();
                
                Assert.Equal(breakElement, build);
            }
        }

        public class ParagraphFacts
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
    }
}