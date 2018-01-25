using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class Acl<T> where T : class
    {
        readonly AclOrder   _order;
        readonly AceList<T> _allow;
        readonly AceList<T> _deny;

        public Acl() : this(AclOrder.Deny_Allow) { }

        public Acl(AclOrder order)
        {
            _order = order;
            _allow = new AceList<T>();
            _deny  = new AceList<T>();
        }

        public void Install(AceType type, IAce<T> entry)
        {
            switch (type)
            {
                case AceType.Allow:
                    _allow.Add(entry);
                    break;
                case AceType.Deny:
                    _deny.Add(entry);
                    break;
            }
        }

        public bool Allow(T value)
        {
            bool b = _order.Equals(AclOrder.Deny_Allow) ? true : false;

            switch (_order)
            {
                case AclOrder.Deny_Allow:
                    if (_allow.Match(value))
                    {
                        b = true;
                    }
                    else if (_deny.Match(value))
                    {
                        b = false;
                    }
                    else
                    {
                        b = true;
                    }
                    break;
                case AclOrder.Allow_Deny:
                    if (_deny.Match(value))
                    {
                        b = false;
                    }
                    else if (_allow.Match(value))
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    break;
            }

            return b;
        }

        class AceList<X> where X : class
        {
            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "FxCop does not detect Add as modification?")]
            private readonly IList<IAce<X>> _list = new List<IAce<X>>();

            public void Add(IAce<X> entry)
            {
                _list.Add(entry);
            }

            public bool Match(X value)
            {
                foreach (IAce<X> entry in _list)
                {
                    if (entry.Match(value))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    internal interface IAce<T> where T : class
    {
        bool Match(T value);
    }

    internal class StringAce : IAce<string>
    {
        private readonly string _entry;
        private readonly bool _matchAll;

        public StringAce(string str)
        {
            _entry = str;
            _matchAll = "*".Equals(str);
        }

        public bool Match(string value)
        {
            return (_matchAll || _entry.Equals(value));
        }
    }

    internal enum AceType
    {
        Deny  = 0,
        Allow = 1
    }

    internal enum AclOrder
    {
        Deny_Allow = 0,
        Allow_Deny = 1
    }
}
