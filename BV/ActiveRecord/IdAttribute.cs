using System;

namespace VB.Common.ActiveRecord
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class IdAttribute : Attribute
    {
    }
}
