using System;
using System.Collections.Generic;
using System.Reflection;
using VB.Common.Core.Utilities;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class ReportParameterValuesDynamic : AbstractReportParameterValues
    {
        private readonly List<MethodArgumentEntry> _methodArguments = new List<MethodArgumentEntry>();
        private readonly Type _type;
        private readonly string _methodName;
        private readonly IDictionary<string, string> _parameterLabelValuePair;

        public ReportParameterValuesDynamic(Type type, string methodName, 
            IDictionary<string, string> parameterLabelValuePair)
        {
            _type = type;
            _methodName = methodName;
            _parameterLabelValuePair = parameterLabelValuePair;
        }
        
        public override IList<IReportParameterValue> GetReportParameterValues(ResolveParameterValue resolver)
        {
            List<Type> argumentTypes = new List<Type>();
            List<object> arguments = new List<object>();

            foreach (MethodArgumentEntry argument in _methodArguments)
            {
                // also gets moved into get
                Type targetType = argument.type;

                object value = null;

                argumentTypes.Add(targetType);

                switch (argument.source)
                {
                    case MethodArgumentSource.Callback:
                        value = resolver(argument.name);
                        break;
                    case MethodArgumentSource.Constant:
                        value = argument.value;
                        break;
                }

                arguments.Add(ConversionHelper.Convert(value, targetType));
            }

            object classInstance = _type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            MethodInfo methodInfo = _type.GetMethod(
                    _methodName,
                    BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance,
                    null,
                    argumentTypes.ToArray(),
                    new ParameterModifier[0]);

            object result = methodInfo.Invoke(classInstance, arguments.ToArray());

            object[] items = null;

            if (result.GetType().IsArray)
            {
                items = (object[])result;
            }
            else
            {
                foreach (Type t in result.GetType().GetInterfaces())
                {
                    if (t.Equals(typeof(System.Collections.ICollection)))
                    {
                        System.Collections.ICollection c = (System.Collections.ICollection)result;
                        items = new object[c.Count];
                        c.CopyTo(items, 0);
                    }
                    if (t.Equals(typeof(ICollection<object>)))
                    {
                        ICollection<object> c = (ICollection<object>)result;
                        items = new object[c.Count];
                        c.CopyTo(items, 0);
                    }
                }

                if (items == null)
                {
                    items = new object[] { result };
                }
            }

            foreach (object item in items)
            {
                ReportParameterValue reportParameterValue = new ReportParameterValue();
                reportParameterValue.Label = GetPropertyValue(item, _parameterLabelValuePair["Label"]).ToString();
                reportParameterValue.Value = GetPropertyValue(item, _parameterLabelValuePair["Value"]).ToString();
                reportParameterValues.Add(reportParameterValue);
            }
            return reportParameterValues;
        }

        public void AddMethodArgumentEntry(string name, Type type, object value, string source)
        {
            MethodArgumentEntry entry = new MethodArgumentEntry();
            entry.name = name;
            entry.type = type;
            entry.value = value;
            entry.source = (MethodArgumentSource)Enum.Parse(typeof(MethodArgumentSource), source);

            _methodArguments.Add(entry);
        }

        private static object GetPropertyValue(object obj, string propertyName)
        {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name == propertyName)
                {
                    return propertyInfo.GetValue(obj, null);
                }
            }
            return null;
        }

        struct MethodArgumentEntry
        {   
            public string name;
            public Type type;
            public object value;
            public MethodArgumentSource source;
        }

        enum MethodArgumentSource
        {
            Callback,
            Constant
        }
    }
}
