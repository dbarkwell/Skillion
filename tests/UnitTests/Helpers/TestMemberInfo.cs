using System;
using System.Reflection;
using Skillion.Attributes;

namespace SkillionUnitTests.Helpers
{
    public class TestMemberInfo : MemberInfo
    {
        private readonly SkillionRequestAttribute[] _customAttributes;
        
        public TestMemberInfo(Type type, string name, SkillionRequestAttribute[] customAttributes)
        {
            ReflectedType = type;
            Name = name;
            _customAttributes = customAttributes;
        }
        
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GetCustomAttributes();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return GetCustomAttributes();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override Type? DeclaringType { get; }
        
        public override MemberTypes MemberType { get; }
        
        public override string Name { get; }
        
        public override Type? ReflectedType { get; }

        private object[] GetCustomAttributes()
        {
            if (_customAttributes == null || _customAttributes.Length == 0)
                return Array.Empty<object>();
                        
            return _customAttributes;
        }
    }
}