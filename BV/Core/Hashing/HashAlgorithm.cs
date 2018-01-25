namespace VB.Common.Core.Hashing
{
    /// <summary>
    /// Hash algorithm. Mostly taken from Microsoft's XmlDiffPatch.
    /// </summary>
    public class HashAlgorithm
    {        
        /// <summary>
        /// Hash value - a signed 64 bit integer.
        /// </summary>
        public long Value
        {
            get { return _hash; }
        }
        private long _hash;

        /// <summary>
        /// Add a bool to the hash.
        /// </summary>
        /// <param name="b">Bool.</param>
        public void AddBool(bool b)
        {
            int i = b ? 1 : 0;
            _hash += (_hash << 11) + i;
        }

        /// <summary>
        /// Add a nullable bool to the hash.
        /// </summary>
        /// <param name="b">Nullable bool.</param>
        public void AddBool(bool? b)
        {
            if (b.HasValue)
            {
                int i = b.Value ? 1 : 0;
                _hash += (_hash << 11) + i;
            }
        }

        /// <summary>
        /// Add a 32 bit integer to the hash.
        /// </summary>
        /// <param name="i">Integer.</param>
        public void AddInt(int i)
        {
            _hash += (_hash << 11) + i;
        }

        /// <summary>
        /// Add a nullable 32 bit integer to the hash.
        /// </summary>
        /// <param name="i">Nullable integer.</param>
        public void AddInt(int? i)
        {
            if (i.HasValue)
            {
                _hash += (_hash << 11) + i.Value;
            }
        }

        /// <summary>
        /// Add a 64 bit integer to the hash.
        /// </summary>
        /// <param name="l">Long.</param>
        public void AddLong(long l)
        {
            _hash += (_hash << 11) + l;
        }

        /// <summary>
        /// Add a string to the hash (if the string is not null).
        /// </summary>
        /// <param name="s">String.</param>
        public void AddString(string s)
        {
            if (s != null)
            {
                _hash += (_hash << 13) + s.Length;

                for (int i = 0; i < s.Length; i++)
                {
                    _hash += (_hash << 17) + s[i];
                }
            }
        }
    }
}
