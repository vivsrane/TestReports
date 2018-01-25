using System;
using System.Data;
using VB.Common.ActiveRecord;

namespace VB.DomainModel.Oltp
{
    [Serializable, Table(Database = "", Name = "Period_D")]
    public class ReportingPeriod : ActiveRecordBase<ReportingPeriod>
    {
        #region Properties
        private int id;
        private string name;

        [Id, Column(Name = "PeriodId", DbType = DbType.Int32)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Column(Name = "Description", DbType = DbType.String)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        protected override ReportingPeriod This
        {
            get { return this; }
        }
    }
}
