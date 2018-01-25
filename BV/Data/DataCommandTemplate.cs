using System.Collections.Generic;
using System.Data;

namespace VB.Common.Data
{
    internal class DataCommandTemplate : IDataCommandTemplate
    {
        private string commandText;
        private CommandType commandType;
        private readonly IList<IDataParameterTemplate> parameters;
        private IDataMap dataMap;

        public DataCommandTemplate()
            : this(CommandType.Text, null, new List<IDataParameterTemplate>())
        {
        }

        public DataCommandTemplate(CommandType commandType, string commandText)
            : this(commandType, commandText, new List<IDataParameterTemplate>())
        {
        }

        public DataCommandTemplate(CommandType commandType, string commandText, IList<IDataParameterTemplate> parameters)
        {
            this.commandType = commandType;
            this.commandText = commandText;
            this.parameters  = parameters;
        }

        public string CommandText
        {
            get { return commandText; }
            set { commandText = value; }
        }

        public CommandType CommandType
        {
            get { return commandType; }
            set { commandType = value; }
        }

        public IList<IDataParameterTemplate> Parameters
        {
            get { return parameters; }
        }

        public IDataMap DataMap
        {
            get { return dataMap; }
            set { dataMap = value;  }
        }
    }
}
