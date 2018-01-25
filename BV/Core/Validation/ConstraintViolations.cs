using System.Collections;
using System.Collections.Generic;

namespace VB.Common.Core.Validation
{
    public class ConstraintViolations : IConstraintViolations
    {
        private readonly List<IConstraintViolation> _items = new List<IConstraintViolation>();

        public IEnumerator<IConstraintViolation> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IConstraintViolation item)
        {
            if (!_items.Exists(x => Equals(x, item)))
            {
                _items.Add(item);
            }
        }

        public void Remove(IConstraintViolation item)
        {
            _items.Remove(item);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public string Message
        {
            get { return string.Format("Encountered {0} constraint violations", Count); }
        }
    }
}