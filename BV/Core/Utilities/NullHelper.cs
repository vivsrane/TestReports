using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Core.Utilities
{
    public static class NullHelper
    {
        public static bool IsList(Type type)
        {
            bool isList = type.Equals(typeof(IList));
            if (type.IsGenericType)
                isList |= type.GetGenericTypeDefinition().Equals(typeof(IList<>));
            return isList;
        }

        public static bool Equals<TType>(TType value1, TType value2) where TType : class
        {
            if (value1 == null)
            {
                return (value2 == null);
            }
            else
            {
                if (value2 == null)
                {
                    return false;
                }
                else
                {
                    Type parameterType = typeof(TType);

                    bool isList = IsList(parameterType);

                    Type[] interfaces = parameterType.GetInterfaces();

                    for (int i = 0; i < interfaces.Length && !isList; i++)
                    {
                        isList = IsList(interfaces[i]);
                    }

                    if (isList)
                    {
                        IEnumerator e1 = ((IList)value1).GetEnumerator();
                        IEnumerator e2 = ((IList)value2).GetEnumerator();

                        while (e1.MoveNext() && e2.MoveNext())
                        {
                            object o1 = e1.Current;
                            object o2 = e2.Current;

                            if (!(o1 == null ? o2 == null : o1.Equals(o2)))
                                return false;
                        }

                        return !(e1.MoveNext() || e2.MoveNext());
                    }
                    else
                    {
                        return value1.Equals(value2);
                    }
                }
            }
        }

        public static int GetHashCode(object value)
        {
            int hashCode = 1;

            if (value != null)
            {
                IList list = value as IList;

                if (list != null)
                {
                    foreach (object item in list)
                    {
                        hashCode = GetHashCode(item, hashCode);
                    }
                }
                else
                {
                    hashCode = value.GetHashCode();
                }
            }

            return hashCode;
        }


        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "hashCode*31", Justification = "Hash code calculations can safely overflow")]
        public static int GetHashCode(object value, int hashCode)
        {
            return hashCode * 31 + GetHashCode(value);
        }
    }
}