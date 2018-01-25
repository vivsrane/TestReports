using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using VB.Common.ActiveRecord;

namespace VB.DomainModel.Oltp
{
    [Serializable, Table(Database = "", Name = "SoftwareSystemComponentState")]
    public class SoftwareSystemComponentState : ActiveRecordBase<SoftwareSystemComponentState>
    {
        #region Fields
        #pragma warning disable 0169, 0649

        [Column(Name = "SoftwareSystemComponentID", DbType = DbType.Int32)]
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Foreign key used by ActiveRecord library")]
        private int? softwareSystemComponentId;

        [Column(Name = "AuthorizedMemberID", DbType = DbType.Int32)]
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Foreign key used by ActiveRecord library")]
        private int? authorizedMemberId;

        [Column(Name = "DealerGroupID", DbType = DbType.Int32)]
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Foreign key used by ActiveRecord library")]
        private int? dealerGroupId;

        [Column(Name = "DealerID", DbType = DbType.Int32, Nullable = true)]
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Foreign key used by ActiveRecord library")]
        private int? dealerId;

        [Column(Name = "MemberID", DbType = DbType.Int32, Nullable = true)]
        private int? memberId;

        #pragma warning restore 0169, 0649
        #endregion

        #region Properties
        private int? id;

        [Id, Column(Name = "SoftwareSystemComponentStateID", DbType = DbType.Int32)]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }
        #endregion

        #region Associations
        private readonly ManyToOne<int?, SoftwareSystemComponentState, SoftwareSystemComponent> softwareSystemComponent;
      

        public ISingleValuedAssociation<SoftwareSystemComponent> SoftwareSystemComponent
        {
            get { return softwareSystemComponent; }
        }

       
        #endregion

        public SoftwareSystemComponentState()
        {
            softwareSystemComponent = new ManyToOne<int?, SoftwareSystemComponentState, SoftwareSystemComponent>("softwareSystemComponentId", this);
           
        }

        protected override SoftwareSystemComponentState This
        {
            get { return this; }
        }
        
    }
}
