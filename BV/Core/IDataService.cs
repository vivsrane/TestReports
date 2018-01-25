using System;
using System.Collections;

namespace VB.Common.Core
{
    public interface IDataService
    {
        void Initialize(DataServiceParameterCallback parameterCallback);

        IEnumerable GetData(DataServiceArguments arguments);

        IAsyncResult BeginGetData(DataServiceArguments arguments, AsyncCallback callback, object state);

        IEnumerable EndGetData(IAsyncResult result);

        bool CanPage { get; }

        bool CanRetrieveTotalRowCount { get; }

        bool CanSort { get; }
    }
}
