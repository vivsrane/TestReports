using System;
using System.ComponentModel;
using VB.Common.Core.Component;
using VB.Common.Core.Validation;

namespace VB.Common.Core
{
    [Serializable]
    public abstract class BusinessObject : INotifyPropertyChanging, INotifyPropertyChanged, IPropertyChanged, IPersisted, IConstrained, ICloneable
    {
        protected BusinessObject()
        {
            IsDirty = true;
            IsNew = true;

            PropertyChanged += MarkDirty;
            PropertyChanged += CheckConstraints;
        }

        protected BusinessObject(BusinessObject copyBusinessObject)
        {
            
        }

        #region Bound

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanging(string propertyName, object oldValue, object newValue)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new ExtendedPropertyChangingEventArgs(propertyName, oldValue, newValue));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            var e = new ExtendedPropertyChangedEventArgs(propertyName, oldValue, newValue);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        protected void Let<TValue>(
            string propertyName,
            ref TValue oldValue,
            TValue newValue)
        {
            if (!Equals(oldValue, newValue) || Equals(newValue, default(TValue)))
            {
                OnPropertyChanging(propertyName, oldValue, newValue);

                oldValue = newValue;

                OnPropertyChanged(propertyName, oldValue, newValue);
            }
        }

        void IPropertyChanged.OnUnknownPropertyChanged()
        {
            OnPropertyChanged(string.Empty);
        }

        #endregion

        #region Constrained

        protected virtual void MarkDirty(object sender, PropertyChangedEventArgs e)
        {
            MarkDirty();
        }

        protected virtual void CheckConstraints(object sender, PropertyChangedEventArgs e)
        {
            var a = e as ExtendedPropertyChangedEventArgs;

            if (a == null)
            {
                GetValidationRules().CheckRules(e.PropertyName);
            }
            else
            {
                GetValidationRules().CheckRules(a.PropertyName, a.OldValue, a.NewValue);
            }
        }

        protected abstract IValidationRules GetValidationRules();

        public IConstraintViolations ConstraintViolations
        {
            get { return GetValidationRules().ConstraintViolations; }
        }

        public virtual bool IsValid
        {
            get { return GetValidationRules().IsValid; }
        }

        #endregion

        public abstract object Clone();
        object ICloneable.Clone(){return Clone();}

        #region Status Checking

        /// <summary>
        /// Is this entity new?
        /// </summary>
        protected bool IsNew { get; set; }
        bool IPersisted.IsNew { get { return IsNew; } }

        /// <summary>
        /// Has the entity been marked for deletion?
        /// </summary>
        protected bool IsDeleted { get; set; }
        bool IPersisted.IsDeleted { get { return IsDeleted; } }

        /// <summary>
        /// Is the entity dirty?
        /// </summary>
        protected virtual bool IsDirty { get; set; }
        bool IPersisted.IsDirty { get { return IsDirty; } }
        #endregion

        #region Status Marking

        /// <summary>
        /// Mark this entity as new.
        /// </summary>
        protected internal virtual void MarkNew()
        {
            IsNew = true;
            
            IsDeleted = false;

            MarkDirty();
        }

        void IPersisted.MarkNew()
        {
            MarkNew();
        }

        /// <summary>
        /// Mark this entity as old.
        /// </summary>
        protected internal virtual void MarkOld()
        {
            IsNew = false;

            MarkClean();
        }

        void IPersisted.MarkOld()
        {
            MarkOld();
        }

        /// <summary>
        /// Mark this entity as being up for deletion.
        /// </summary>
        protected virtual void MarkDeleted()
        {
            IsDeleted = true;

            MarkDirty();
        }

        void IPersisted.MarkDeleted()
        {
            MarkDeleted();
        }

        /// <summary>
        /// Mark this entity as dirty.
        /// </summary>
        protected virtual void MarkDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Mark this entity as clean.
        /// </summary>
        protected virtual void MarkClean()
        {
            IsDirty = false;
        }

        #endregion
    }
}
