using System;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.ActiveRecord
{
    /// <summary>
    /// Specifies a Many to One relation between business objects.
    /// </summary>
    /// <typeparam name="TKeyType">The type for the Id used for the relationship.</typeparam>
    /// <typeparam name="TMappedBy">The type for the Many side of the relationship.</typeparam>
    /// <typeparam name="TTargetEntity">The type for the One side of the relationship.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    [Serializable]
    public class ManyToOne<TKeyType, TMappedBy, TTargetEntity> : Finder<TTargetEntity>, ISingleValuedAssociation<TTargetEntity>
        where TMappedBy : class, new()
        where TTargetEntity : class, new()
    {
        private readonly TMappedBy mappedBy;
        private readonly string memberName;
        private TTargetEntity targetEntity;
        private bool select;

        public ManyToOne(string memberName, TMappedBy mappedBy)
        {
            this.memberName = memberName;
            this.mappedBy = mappedBy;
            targetEntity = null;
            select = true;
        }

        public TKeyType Id
        {
            get { return (TKeyType) RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().Member(memberName).Get(mappedBy); }
            set { select = true; RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().Member(memberName).Set(mappedBy, value); }
        }

        public TTargetEntity GetValue()
        {
            if (select)
            {
                if (!Equals(Id, default(TKeyType))) // null foreign key => no mapping
                {
                    targetEntity = FindFirst(CreateWhereClause(GetRowDataGateway().PrimaryKey.MemberInfo.Name, Id));
                }

                select = false;
            }

            return targetEntity;
        }

        public void SetValue(TTargetEntity value)
        {
            if (value == null)
            {
                RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().Member(memberName).Set(mappedBy, null);
            }
            else
            {
                TKeyType fk = (TKeyType)GetRowDataGateway().PrimaryKey.Get(value);

                RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().Member(memberName).Set(mappedBy, fk);
            }

            targetEntity = value;
        }
    }
}
