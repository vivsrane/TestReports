using System;
using System.Collections.Generic;
using System.ComponentModel;
using VB.Common.Core.Component;

namespace VB.Common.Core
{
    [Serializable]
    public class BusinessObjectList<T> : BindingList<T> where T : BusinessObject, ICloneable
    {
        private List<T> _deletedList;

        public List<T> DeletedList
        {
            get
            {
                if (_deletedList == null)
                {
                    _deletedList = new List<T>();
                }
                return _deletedList;
            }
        }

        private void DeleteChild(T child)
        {
            IPersisted persisted = child as IPersisted;

            if (persisted != null)
            {
                persisted.MarkDeleted();
            }

            DeletedList.Add(child);
        }

        protected override void RemoveItem(int index)
        {
            T child = this[index];

            base.RemoveItem(index);

            DeleteChild(child);
        }

        protected override void ClearItems()
        {
            while (Count > 0) RemoveItem(0);

            base.ClearItems();
        }

        protected override void SetItem(int index, T item)
        {
            T child = default(T);
            
            if (!ReferenceEquals(this[index], item))
            {
                child = this[index];
            }

            base.SetItem(index, item);

            if (child != null)
            {
                DeleteChild(child);
            }
        }

        public BusinessObjectList<T> Clone()
        {
            BusinessObjectList <T> clonedList = new BusinessObjectList<T>();
            foreach (T item in this)
            {
                clonedList.Add(item.Clone() as T);
            }
            return clonedList;
        }

        public bool IsDirty
        {
            get
            {
                // true if we have any deleted non-new items the list is dirty

                foreach (T item in DeletedList)
                {
                    IPersisted savable = item as IPersisted;

                    if (savable != null)
                    {
                        if (!savable.IsNew)
                        {
                            return true;
                        }
                    }
                }

                // true if we have any new (non ISavable) or dirty objects

                foreach (T item in this)
                {
                    IPersisted savable = item as IPersisted;

                    if (savable != null)
                    {
                        if (savable.IsDirty)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                // else

                return false;
            }
        }
    }
}
