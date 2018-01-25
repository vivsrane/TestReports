using System;

namespace VB.Common.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxLengthAttribute : Attribute
    {
        private readonly int _value;

        public MaxLengthAttribute(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }
    }
}