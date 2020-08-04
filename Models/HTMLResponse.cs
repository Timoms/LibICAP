using System;
using System.Collections.Generic;
using System.Text;

namespace LibICAP.Models
{
    public class HTMLResponse
    {
        public int Length { 
            get
            {
                return Encoding.ASCII.GetBytes(Content).Length;
            }
        }
        public string Content { get; set; }
        public string HexLength {
            get
            {
                return Length.ToString("X");
            }
        }
    }
}
