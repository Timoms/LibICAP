using System;
using System.Collections.Generic;
using System.Text;

namespace LibICAP.Models
{
    public class TCPRequest
    {
        public string Type { get; set; }
        public string URL { get; set; }
        public string Version { get; set; }
        public string Host { get; set; }
        public string UserAgent { get; set; }
        public string Encapsulated { get; set; }
        public List<string> Header { get; set; }
        public string EncapsulatedContent { get; set; }
        public byte[] Response { get; set; }
    }
}
