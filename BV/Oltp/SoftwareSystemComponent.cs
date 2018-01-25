using System;
using System.Data;
using VB.Common.ActiveRecord;

namespace VB.DomainModel.Oltp
{
    [Serializable, Table(Database = "IMT", Name = "SoftwareSystemComponent")]
    public class SoftwareSystemComponent : ActiveRecordBase<SoftwareSystemComponent>
    {
        private int? id;
        private string name;
        private string token;

        [IdAttribute, ColumnAttribute(Name = "SoftwareSystemComponentID", DbType = DbType.Int32)]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        [ColumnAttribute(Name = "Name", DbType = DbType.String)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [ColumnAttribute(Name = "Token", DbType = DbType.String)]
        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        protected override SoftwareSystemComponent This
        {
            get { return this; }
        }
    }
}
