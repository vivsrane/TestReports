
using System;

namespace VB.Common.ActiveRecord
{
    [Serializable]
    public abstract class ActiveRecordBase<T> where T : class, new()
    {
        protected abstract T This
        {
            get; // ugly but necessary for the call to the row data gateway
        }

        public void Save()
        {
            RowDataGatewayRegistry<T>.GetRowDataGateway().Save(This);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this == obj)
                return true;
            if (!GetType().Equals(obj.GetType()))
                return false;
            RowDataGateway<T> gateway = RowDataGatewayRegistry<T>.GetRowDataGateway();
            object pk1 = gateway.PrimaryKey.Get(this);
            object pk2 = gateway.PrimaryKey.Get(obj);
            bool equals;
            if (pk1 == null)
            {
                if (pk2 == null)
                {
                    equals = base.Equals(obj);
                }
                else
                {
                    equals = false;
                }
            }
            else
            {
                if (pk2 == null)
                {
                    equals = false;
                }
                else
                {
                    equals = pk1.Equals(pk2);
                }
            }
            return equals;
        }

        public override int GetHashCode()
        {
            return RowDataGatewayRegistry<T>.GetRowDataGateway().HashCode(This);
        }

        public override string ToString()
        {
            return RowDataGatewayRegistry<T>.GetRowDataGateway().ToString(This);
        }
    }
}
