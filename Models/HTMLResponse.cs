using System;
using System.Collections.Generic;
using System.Text;

namespace LibICAP.Models
{
    public class HTMLResponse
    {
        /// <summary>
        /// Calculates the length of a string transformed to a bytearray
        /// </summary>
        public int Length { 
            get
            {
                return Encoding.ASCII.GetBytes(Content).Length;
            }
        }
        /// <summary>
        /// Text based representation of HTML Response send to a client
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Calculates the length of a bytearray to hexadecimal representation
        /// </summary>
        public string HexLength {
            get
            {
                return Length.ToString("X");
            }
        }
    }
}
