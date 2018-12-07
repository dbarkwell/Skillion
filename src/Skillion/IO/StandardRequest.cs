using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skillion.IO
{
    public class IntentRequest
    {
        [JsonRequired]
        public string Version { get; set; }
        [JsonRequired]
        public Session Session { get; set; }
        [JsonRequired]
        public Request Request { get; set; }
    }

    public class Session
    {
        public bool New { get; set; }
        
        public string SessionId { get; set; }
        
        public IDictionary<string, object> Attributes { get; set; }
    }
    
    public class Request
    {
        public string Type { get; set; }
        
        public string RequestId { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public string Locale { get; set; }
        
        public Intent Intent { get; set; }
    }

    public class Intent
    {
        public string Name { get; set; }
        
        public string ConfirmationStatus { get; set; }
    }
}