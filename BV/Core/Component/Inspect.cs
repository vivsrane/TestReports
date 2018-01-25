using System;
using System.Linq.Expressions;
using System.Reflection;

namespace VB.Common.Core.Component
{
    public static class Inspect
    {
        public static string NameOf<T, K>(Expression<Func<T, K>> propertySelector)
        {
            if (propertySelector.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException("A property selector expression has an incorrect format");
            }

            MemberExpression memberAccess = propertySelector.Body as MemberExpression;

            if (memberAccess == null || memberAccess.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("A selected member is not a property");
            }

            return memberAccess.Member.Name;
        }
    }
}