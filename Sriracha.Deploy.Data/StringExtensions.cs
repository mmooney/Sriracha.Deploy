using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    [Obsolete("Move to MMDB.Shared")]
    public static class StringExtensions
    {
        public static bool Contains(this string x, string value, StringComparison comparer)
        {
            return (x.IndexOf(value, comparer) >= -1);
        }
    }
}
