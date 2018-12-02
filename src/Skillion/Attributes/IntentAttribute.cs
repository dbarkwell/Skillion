using System;

namespace Skillion.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class IntentAttribute : Attribute
    {
        public IntentAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; set; }
    }
}