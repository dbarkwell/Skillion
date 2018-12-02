using System.Collections.Generic;

namespace Skillion.IO
{
    public abstract class StandardResponse
    {
        public string Version => "1.0";
        
        public Response Response { get; set; }
    }

    public class Response
    {
        public OutputSpeech OutputSpeech { get; set; }
        
        public Card Card { get; set; }
        
        public Reprompt Reprompt { get; set; }

        public IEnumerable<Directives> Directives { get; set; }
        
        public bool? ShouldEndSession { get; set; } = true;
    }
    
    // TODO total size 
    public class Card
    {
        public string Type { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
    }

    public class Reprompt
    {
        public OutputSpeech OutputSpeech { get; set; }
    }
    
    // TODO total char 8000char
    public class OutputSpeech
    {
        public string Type { get; set; }
            
        public string Text { get; set; }
    }

    public class Directives
    {
        public string Type { get; set; }
        
        public string PlayBehavior { get; set; }

        public AudioItem AudioItem { get; set; }
    }

    public class AudioItem
    {
        public Stream Stream { get; set; }
    }

    public class Stream
    {
        public string Token { get; set; }
        
        public string Url { get; set; }
        
        public long OffsetInMilliseconds { get; set; }
     }
}