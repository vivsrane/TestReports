using System;

namespace VB.Common.Core.Component
{
    public class ConcurrentModificationException : Exception
    {
        public ConcurrentModificationException(string message) : base(message) { }
    }
}