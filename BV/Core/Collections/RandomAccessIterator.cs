using System;
using System.Collections.Generic;

namespace VB.Common.Core.Collections
{
    public class RandomAccessIterator<T> : IEnumerator<T> {

        private RandomAccess<T> _list;

        private int _cursor = -1;

        private readonly int _expectedModificationCount;

        public RandomAccessIterator(RandomAccess<T> list)
        {
            _list = list;

            _expectedModificationCount = list.ModificationCount;
        }

        public bool MoveNext
        {
            get
            {
                CheckForComodification();

                if (++_cursor != _list.Count)
                {
                    return true;
                }

                return false;
            }
        }

        public T Current
        {
            get
            {
                try
                {
                    return _list[_cursor];
                }
                catch (ArgumentOutOfRangeException)
                {
                    CheckForComodification();

                    throw;
                }
            }
        }

        private void CheckForComodification()
        {
            if (_expectedModificationCount != _list.ModificationCount)
            {
                throw new Exception("Concurrent Modification");
            }
        }

        #region IEnumerator<T> Members

        T IEnumerator<T>.Current
        {
            get { return Current; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _list = null;
            }
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            return MoveNext;
        }

        void System.Collections.IEnumerator.Reset()
        {
            _cursor = -1;
        }

        #endregion
    }
}

//end namespace Model