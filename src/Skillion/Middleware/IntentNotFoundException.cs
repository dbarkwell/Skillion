using System;

namespace Skillion.Middleware
{
    public class IntentNotFoundException : Exception
    {
        public IntentNotFoundException(string message) : base(message)
        {}
        
        public IntentNotFoundException() : base("Intent specified was not found.")
        {}
    }
}