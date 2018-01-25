using System;

namespace VB.Common.Core.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ConstraintAttribute : Attribute
    {
    }
}