using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibICAP.Utilities
{
    static class Tools
    {
        public static string EqualElement(this string input)
        {
            return input.Substring(input.IndexOf("=") + 2);
        }        
        public static string ColonElement(this string input)
        {
            return input.Substring(input.IndexOf(":") + 2);
        }
        public static string LastWord(this string input, int times = int.MaxValue)
        {
            return input.Split(new char[] { ' ' }, times).Last();
        }
        public static string FirstWord(this string input, int times = int.MaxValue)
        {
            return input.Split(new char[] { ' ' }, times).First();
        }

        public static byte[] GetBytes(this string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }
        public static int CountBytes(this string input)
        {
            return Encoding.ASCII.GetBytes(input).Length;
        }
    }
}