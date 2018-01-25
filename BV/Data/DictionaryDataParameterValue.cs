using System;
using System.Collections.Generic;
using VB.Common.Core.Utilities;

namespace VB.Common.Data
{
    public class DictionaryDataParameterValue
    {
        private readonly IDictionary<string, object> values;

        public DictionaryDataParameterValue() : this(new Dictionary<string, object>())
        {
        }

        public DictionaryDataParameterValue(IDictionary<string,object> values)
        {
            this.values = values;
        }

        public IDictionary<string, object> GetValues()
        {
          return  this.values;
        }
        public object DataParameterValue(IDataParameterTemplate template)
        {
            if (values.ContainsKey(template.Name))
                return ConversionHelper.Convert(values[template.Name], ConversionHelper.ToType(template.DbType), DBNull.Value);
            return DBNull.Value;
        }

        public void Add(string parameterName, object parameterValue)
        {
            values.Add(parameterName, parameterValue);
        }

        public object this[string key]
        {
            get { return values[key]; }
            set { values[key] = value; }
        }
    }
}
