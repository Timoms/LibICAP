using System;
using System.Collections.Generic;
using System.Text;

namespace LibICAP.Utilities
{
    public static class Header
    {
        public static readonly byte[] GZIP = { 31, 139 };
        public static readonly byte[] Brotli = { 0xCE, 0xB2, 0xCF, 0x81 };
        public static byte[] HTML { 
            get 
            {
                return Encoding.UTF8.GetBytes("div");
            } 
        }
    }
}
