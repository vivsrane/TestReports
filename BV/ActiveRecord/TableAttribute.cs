using System;

namespace VB.Common.ActiveRecord
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TableAttribute : Attribute
    {
        private string database;
        private string name;

        public string Database
        {
            get { return database; }
            set { database = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        override public string ToString()
        {
            return "[database=" + database + ",name=" + name + "]";
        }
    }
}
