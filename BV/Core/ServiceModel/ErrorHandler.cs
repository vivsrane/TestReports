using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using VB.Common.Core.Logging;

namespace VB.Common.Core.ServiceModel
{
    public class ErrorHandler : IErrorHandler
    {
        private Type ServiceType { get; set; }
        private readonly ILog Log = LoggerFactory.GetLogger<ErrorHandler>();

        public ErrorHandler(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public bool HandleError(Exception error)
        {
            Log.Error(
                string.Format("Unhandled exception in service type {0}", ServiceType.FullName), error);
            return false;
        }
    }
}