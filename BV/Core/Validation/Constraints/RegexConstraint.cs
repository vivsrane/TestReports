using System.Text.RegularExpressions;

namespace VB.Common.Core.Validation.Constraints
{
    public class RegexConstraint : IConstraint<string>
    {
        private readonly string _resourceKey;

        private readonly string _pattern;

        public RegexConstraint(string pattern)
            : this("global.constraint.regex", pattern)
        {
        }

        public RegexConstraint(string resourceKey, string pattern)
        {
            _pattern = pattern;
            _resourceKey = resourceKey;
        }

        public string Pattern
        {
            get { return _pattern; }
        }

        public bool IsSatisfiedBy(string value)
        {
            if (value == null)
            {
                return true;
            }

            if (Regex.IsMatch(value, Pattern))
            {
                return false;
            }

            return true;
        }

        public string ResourceKey
        {
            get { return _resourceKey; }
        }

        public bool StopProcessing
        {
            get { return false; }
        }

        public static RegexConstraint GetConstraint(RegExPatterns pattern)
        {
            return new RegexConstraint(GetPattern(pattern));
        }

        public static RegexConstraint GetConstraint(RegExPatterns pattern, string resourceKey)
        {
            return new RegexConstraint(resourceKey, GetPattern(pattern));
        }

        public static string GetPattern(RegExPatterns pattern)
        {
            switch (pattern)
            {
                case RegExPatterns.Ssn:
                    return @"^\d{3}-\d{2}-\d{4}$";
                case RegExPatterns.Email:
                    return @"^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$";
                case RegExPatterns.Integer:
                    return @"^[0-9]*$";
                case RegExPatterns.StateCode:
                    return @"^(A[LKZR]|C[AOT]|DE|FL|GA|HI|I[ADLN]|K[SY]|LA|M[ADEINOST]|N[CDEHJMVY]|O[HKR]|PA|RI|S[CD]|T[NX]|UT|V[AT]|W[AIVY])$";
                default:
                    return string.Empty;
            }
        }
    }

    public enum RegExPatterns
    {
        /// <summary>
        /// US Social Security number pattern.
        /// </summary>
        Ssn,
        /// <summary>
        /// Email address pattern.
        /// </summary>
        Email,
        /// <summary>
        /// Integer pattern
        /// </summary>
        Integer,
        /// <summary>
        /// State pattern
        /// </summary>
        StateCode
    }
}

