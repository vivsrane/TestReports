using System.Collections.Generic;
using System.Data;

namespace VB.Common.Data
{
    public interface IDataCommandTemplate
    {
        string CommandText
        {
            get;
            set;
        }

        CommandType CommandType
        {
            get;
            set;
        }

        IList<IDataParameterTemplate> Parameters
        {
            get;
        }

        IDataMap DataMap
        {
            get;
            set;
        }
    }
}
