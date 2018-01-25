using System;
using System.Runtime.Serialization;
using System.Xml;

namespace VB.Common.Core.Xml
{
    [Serializable]
    public class SerializableXmlDocument : XmlDocument, ISerializable 
    {
        public SerializableXmlDocument(){}

        protected SerializableXmlDocument(SerializationInfo info, StreamingContext context)
        {
            InnerXml = info.GetString("XML");
            // Consider using XmlDocument.Load() instead
        }

        public override sealed string InnerXml
        {
            get { return base.InnerXml; }
            set { base.InnerXml = value; }
        }

        #region Implementation of ISerializable

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("XML", InnerXml);
            // consider using XmlDocument.Save() instead
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if( info == null )
            {
                throw new ArgumentNullException("info");
            }
            GetObjectData(info, context);
        }

        #endregion
    }
}
