using System;
using System.Reflection;

namespace VB.Common.ActiveRecord
{
    public class DynamicMember
    {
        private readonly MemberInfo memberInfo;
        private readonly DynamicMethodGetHandler getter;
        private readonly DynamicMethodSetHandler setter;

        public DynamicMember(Type type, PropertyInfo property)
            : this(property, DynamicMethodCompiler.CreateGetHandler(type, property), DynamicMethodCompiler.CreateSetHandler(type, property))
        {
            // no op
        }

        public DynamicMember(Type type, FieldInfo field)
            : this(field, DynamicMethodCompiler.CreateGetHandler(type, field), DynamicMethodCompiler.CreateSetHandler(type, field))
        {
            // no op
        }

        public DynamicMember(MemberInfo memberInfo, DynamicMethodGetHandler getter, DynamicMethodSetHandler setter)
        {
            this.memberInfo = memberInfo;
            this.getter = getter;
            this.setter = setter;
        }

        public MemberInfo MemberInfo
        {
            get { return memberInfo; }
        }

        public object Get(object item)
        {
            return getter(item);
        }

        public void Set(object item, object value)
        {
            setter(item, value);
        }
    }
}
