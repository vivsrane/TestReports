using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.XmlDiffPatch;

namespace VB.Common.Core
{
    public abstract class PropertyChangeSupport : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public virtual event PropertyChangingEventHandler PropertyChanging;

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void Assignment(PropertyChange change, string propertyName)
        {
            OnPropertyChanging(propertyName);
            change();
            OnPropertyChanged(propertyName);
        }

        protected bool AreSame(XmlDocument a, XmlDocument b)
        {
            if (a == null && b != null) return false;

            if (a != null && b == null) return false;

            if (ReferenceEquals(a, b)) return true;

            XmlDiff diff = new XmlDiff(XmlDiffOptions.IgnoreChildOrder |
                                       XmlDiffOptions.IgnoreComments |
                                       XmlDiffOptions.IgnoreWhitespace |
                                       XmlDiffOptions.IgnoreXmlDecl);

            bool identical;

            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb))
            {
                using (XmlWriter xw = XmlWriter.Create(sw))
                {
                   identical = diff.Compare(new XmlNodeReader(a), new XmlNodeReader(b), xw);
                }
            }

            return identical;
        }
    }
}