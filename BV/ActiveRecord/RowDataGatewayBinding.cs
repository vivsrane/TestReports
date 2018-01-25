using System.Data;
using VB.Common.Core.Utilities;

namespace VB.Common.ActiveRecord
{
    public class RowDataGatewayBinding
    {
        ColumnAttribute column;
        DynamicMember member;
        object value;

        public ColumnAttribute Column
        {
            get { return column; }
        }

        public DynamicMember Member
        {
            get { return member; }
        }

        public object Value
        {
            get { return value; }
        }

        public RowDataGatewayBinding(ColumnAttribute column, DynamicMember member, object value)
        {
            this.column = column;
            this.member = member;
            this.value = value;
        }

        public RowDataGatewayBinding(string column, object value, DbType type, bool nullable)
        {
            this.column = new ColumnAttribute();
            this.column.Name = column;
            this.column.DbType = type;
            this.column.Nullable = nullable;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            RowDataGatewayBinding other = obj as RowDataGatewayBinding;
            if (other == null)
                return false;
            bool lEquals = true;
            lEquals &= NullHelper.Equals(column, other.column);
            lEquals &= NullHelper.Equals(value, other.value);
            return lEquals;
        }

        public override int GetHashCode()
        {
            int hashCode = 1;
            hashCode = NullHelper.GetHashCode(column, hashCode);
            hashCode = NullHelper.GetHashCode(value, hashCode);
            return hashCode;
        }
    }
}
