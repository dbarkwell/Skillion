using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Skillion.IO
{
    public class StandardRequest
    {
        [JsonRequired]
        public string Version { get; set; }
        
        [JsonRequired]
        public Session Session { get; set; }
        
        [JsonRequired]
        public Request Request { get; set; }
    }

    public class Request
    {
        public string Type { get; set; }
        
        public string RequestId { get; set; }
        
        public DateTime Timestamp { get; set; }

        public string Locale { get; set; }
        
        public Intent Intent { get; set; }
        
        public Error Error { get; set; }
    }

    public class Error
    {
        public string Type { get; set; } 
        
        public string Message { get; set; }
    }
    
    public class Intent
    {
        public string Name { get; set; }
        
        public string ConfirmationStatus { get; set; }
    }
    
    public class Session
    {
        public bool New { get; set; }
        
        public string SessionId { get; set; }
        
        public IDictionary<string, object> Attributes { get; set; }
    }
}