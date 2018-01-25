using System;
using System.Data;
using VB.Common.ActiveRecord;

namespace VB.DomainModel.Oltp
{
    [Serializable, Table(Database = "IMT", Name = "Role")]
    public class Role : ActiveRecordBase<Role>
    {
        private int id;
        private string roleType;
        private string roleCode;
        private string name;

        [IdAttribute, ColumnAttribute(Name = "RoleID", DbType = DbType.Int32)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [ColumnAttribute(Name = "Type", DbType = DbType.String)]
        public string RoleType
        {
            get { return roleType; }
            set { roleType = value; }
        }

        [ColumnAttribute(Name = "Code", DbType = DbType.String)]
        public string RoleCode
        {
            get { return roleCode; }
            set { roleCode = value; }
        }

        [ColumnAttribute(Name = "Name", DbType = DbType.String)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        protected override Role This
        {
            get { return this; }
        }
    }
}
