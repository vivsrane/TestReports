using System;
using VB.Common.Core.Extensions;

namespace VB.Common.Core
{
    public static class TypeUtility
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string Name(Type t, string postfix)
        {
            // alphanumeric, underscore, hyphens, length 1-80
            var fullName = t.FullName.Replace(".", "_");

            if (!string.IsNullOrEmpty(postfix))
                fullName += "_" + postfix;

            return fullName.Right(80);
        }

        public static string Name(string name, string postfix, char postfixSeperator)
        {
            if (name == "VB-appraisalphotos")
            {
               Log.InfoFormat("FL photos bucket : VB-appraisalphotos-{0}",postfix.ToLower());
                switch (postfix.ToLower())
                {
                    case "alpha":
                    case "dev":
                        postfix = "alpha";
                        break;
                    case "beta":
                        postfix = "staging";
                        break;
                    case "release":
                        postfix = "release";
                        break;
                    default:
                        throw new Exception("No Such enviroment");
                }
            }
            if (!string.IsNullOrEmpty(postfix))
                name += postfixSeperator + postfix;

            return name.Right(80);
        }

        public static string Name(string name, string postfix)
        {
            return Name(name, postfix, '_');
        }

    }
}
