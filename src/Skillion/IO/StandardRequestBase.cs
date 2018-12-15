using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Skillion.IO
{
    public class StandardRequestBase
    {
        [JsonRequired]
        public string Version { get; set; }
        
        [JsonRequired]
        public Session Session { get; set; }
        
        [JsonRequired]
        public Context Context { get; set; }
        
        [JsonRequired]
        public StandardRequest Request { get; set; }
    }

    public class StandardRequest
    {
        public string Type { get; set; }
        
        public string RequestId { get; set; }
        
        public DateTime Timestamp { get; set; }

        public string DialogState { get; set; }
        
        public string Reason { get; set; }
        
        public string Locale { get; set; }
        
        public Intent Intent { get; set; }
        
        public Error Error { get; set; }
    }

    public class Context
    {
        public System System { get; set; }
        
        //public AudioPlayer AudioPlayer { get; set; }
    }

    public class System
    {
        public string ApiAccessToken { get; set; }
        
        public string ApiEndpoint { get; set; }
        
        public Application Application { get; set; }
        
       // public Device Device { get; set; }
        
        public User User { get; set; }
    }

    public class Application
    {
        public string ApplicationId { get; set; }
    }

    //public class Device
    //{
    //        
    //}

    public class User
    {
        public string UserId { get; set; }
        
        public string AccessToken { get; set; }
        
        public string Permissions { get; set; }
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
        
        public IDictionary<string, Slot> Slots { get; set; }
    }

    public class Slot
    {
        public string Name { get; set; }
        
        public string Value { get; set; }
        
        public string ConfirmationStatus { get; set; }
        
        public string Source { get; set; }
    }
    
    public class Session
    {
        public bool New { get; set; }
        
        public string SessionId { get; set; }
        
        public IDictionary<string, object> Attributes { get; set; }
    }
}