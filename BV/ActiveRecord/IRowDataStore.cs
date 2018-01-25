using System.Collections.Generic;

namespace VB.Common.ActiveRecord
{
    public interface IRowDataStore<T>
    {
        bool ContainsKey(object key);

        IList<T> this[object key] { get; set; }
    }
}
