using System;
using System.Data;

namespace VB.Common.Data
{
    [Serializable]
    public class DataParameterTemplate : IDataParameterTemplate
    {
        private string name;
        private DbType type;
        private bool nullable;

        public DataParameterTemplate() : this(null, default(DbType), false)
        {
        }

        public DataParameterTemplate(string name, DbType type, bool nullable)
        {
            this.name = name;
            this.type = type;
            this.nullable = nullable;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DbType DbType
        {
            get { return type; }
            set { type = value; }
        }

        public bool IsNullable
        {
            get { return nullable; }
            set { nullable = value; }
        }
    }
}
