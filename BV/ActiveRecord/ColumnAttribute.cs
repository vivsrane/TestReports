using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using VB.Common.Core.Utilities;

namespace VB.Common.ActiveRecord
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        private string name;
        private DbType dbType;
        private bool nullable;
        private string selectCast;
        private string updateCast;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db", Justification = "Framework Design Guideline: Consider giving a property the same name as its type.")]
        public DbType DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        public bool Nullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        public string SelectCast
        {
            get { return selectCast; }
            set { selectCast = value; }
        }

        public string UpdateCast
        {
            get { return updateCast; }
            set { updateCast = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            ColumnAttribute other = obj as ColumnAttribute;
            if (other == null)
                return false;
            bool lEquals = true;
            lEquals &= NullHelper.Equals(name, other.name);
            lEquals &= dbType.Equals(other.dbType);
            lEquals &= Nullable.Equals(other.nullable);
            lEquals &= NullHelper.Equals(selectCast, other.selectCast);
            lEquals &= NullHelper.Equals(updateCast, other.updateCast);
            return lEquals;
        }

        public override int GetHashCode()
        {
            int hashCode = 1;
            hashCode = NullHelper.GetHashCode(name, hashCode);
            hashCode = NullHelper.GetHashCode(dbType, hashCode);
            hashCode = NullHelper.GetHashCode(nullable, hashCode);
            hashCode = NullHelper.GetHashCode(selectCast, hashCode);
            hashCode = NullHelper.GetHashCode(updateCast, hashCode);
            return hashCode;
        }

        public override string ToString()
        {
            return "[name=" + name + ", type=" + dbType + "]";
        }
    }
}
