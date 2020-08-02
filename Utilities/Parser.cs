using LibICAP.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace LibICAP.Utilities
{
    class Parser
    {
        /// <summary>
        /// Checks for fist line and decides on the next parser
        /// </summary>
        private static Logger _Logger = new Logger();
        private static TCPRequest RequestBuffer { get; set; }
        public static TCPRequest ParseData(string data)
        {
            string[] chunks = data.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries); // split on three main chunks -> ICAP - REQUEST - HEADEr

            //--string[] ICAPRequest = chunks[0].Split("\r\n", StringSplitOptions.RemoveEmptyEntries); // contains client informations
            //--string[] DestinationRequest = chunks[1].Split("\r\n", StringSplitOptions.RemoveEmptyEntries); // contains request or response information
            //--string[] HTMLRequest = chunks[2].Split("\r\n", StringSplitOptions.RemoveEmptyEntries); // not needed because no necessary information

            if (chunks[0].Contains("REQMOD"))
            {
                return R_REQMOD(chunks[1]);
            } 
            else
            {
                _Logger.Log(Logger.LogLevel.Error, "Request is not a REQMOD", false);
            }

            return null;

            /*
            REQMOD icap://10.0.2.212:1344/ ICAP/1.0                                         (REQMOD|RESPMOD|OPTIONS) icap:\/\/((?:[0-9]{1,3}\.){3}[0-9]{1,3}):(\d*)\/ ICAP\/(\d.\d)
            Host: 10.0.2.212:1344                                                           Host: (.*):(\d*)
            X-Client-IP: 10.0.20.101                                                        X-Client-IP: (.*)
            X-Server-IP: 172.217.23.131                                                     X-Server-IP: (.*)
            X-Authenticated-User: TG9jYWw6Ly9hbm9ueW1vdXM=                                  X-Authenticated-User: (.*)
            X-Authenticated-Groups: TG9jYWw6Ly9sb2NhbGhvc3Qvbm8gYXV0aGVudGljYXRpb24=        X-Authenticated-Groups: (.*)
            User-Agent: FortiOS                                                             User-Agent: (.*)
            Encapsulated: req-hdr=0, null-body=240                                          Encapsulated: req-hdr=(\d*), (null-body|req-body)=(\d*)
            */

            /*
            GET /msdownload/update/v3/static/trustedr/en/disallowedcertstl.cab?62e1794ce62ea2ce HTTP/1.1    (GET) (.*) HTTP\/(\d.\d)
            Cache-Control: no-cache                                                                         Cache-Control: (.*)
            Pragma: no-cache                                                                                Pragma: (.*)
            Accept: *                                                                                       Accept: (.*)
            User - Agent: Microsoft - CryptoAPI / 10.0                                                      User-Agent: (.*)
            Host: ctldl.windowsupdate.com                                                                   Host: (.*)

            RANDOMLONGSTRING/STRINGLONGRANDOM HTTP/1.1
            Host: client.teamviewer.com
            User - Agent: Mozilla / 4.0(compatible; MSIE 6.0; DynGate)
            Accept: *             
             */         
        }
        /// <summary>
        /// Parses OPTIONS request and builds the reponse, no handling needed
        /// </summary>
        private static TCPRequest R_OPTIONS(TCPRequest x)
        {
            x.Response = Encoding.UTF8.GetBytes(ResponseBuilder.OPTIONSAnswer());
            return x;
        }
        /// <summary>
        /// I would like to get "xxx.xxx". Can i visit that page? Please modify my request as you allow.
        /// </summary>
        private static TCPRequest R_REQMOD(string x)
        {
            string type = new Regex(@"(GET) (.*) HTTP\/(\d.\d)").Match(x).Groups[1].Value;
            string url = new Regex(@"(GET) (.*) HTTP\/(\d.\d)").Match(x).Groups[2].Value;
            string version = new Regex(@"(GET) (.*) HTTP\/(\d.\d)").Match(x).Groups[3].Value;
            string host = new Regex(@"Host: (.*)").Match(x).Groups[1].Value;


            return new TCPRequest
            {
                Response = Encoding.ASCII.GetBytes(ResponseBuilder.NaughtyContent(url, host)),
                Host = host,
                Type = type,
                URL = url,
                Version = version
            };
        } 
        private static TCPRequest R_RESPMOD(TCPRequest x)
        {

           // var encaps = x[3].ColonElement(); // contains GET/POST Request

           return x;
        }
    }
}
