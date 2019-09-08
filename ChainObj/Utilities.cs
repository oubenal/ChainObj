using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainObj
{
    // Ascii Utf-16
    internal static class Utilities
    {
        public static string ToFixedHex(this int val)
        {
            return val.ToString("X4");
        }
    }
}
