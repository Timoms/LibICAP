using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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
        public static string CheckCategory(string domain)
        {
            using WebClient web = new WebClient();
            web.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1)");
            web.Headers.Add("Origin", "https://fortiguard.com");
            web.Headers.Add("Referer", "https://fortiguard.com/webfilter");
            string fortiResult = web.DownloadString("https://fortiguard.com/webfilter?q=" + domain);

            Regex regex = new Regex("Category: (.*)<");
            return regex.Match(fortiResult).Groups[1].Value;
        }
        public static string GetCurrentICAPDate()
        {
            return DateTime.Now.ToString("ddd MMM dd yyyy HH:mm:ss 'GMT'");
        }

    }
}