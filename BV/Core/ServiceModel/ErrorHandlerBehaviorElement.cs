using System;
using System.ServiceModel.Configuration;

namespace VB.Common.Core.ServiceModel
{
    public class ErrorHandlerBehaviorElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new ErrorHandlerBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof (ErrorHandlerBehavior); }
        }
    }
}