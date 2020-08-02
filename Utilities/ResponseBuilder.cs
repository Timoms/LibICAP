using System;
using System.Collections.Generic;
using System.Text;

namespace LibICAP.Utilities
{
    class ResponseBuilder
    {
        public static string OPTIONSAnswer()
        {
            #region ExampleAnswer
            /*
               ICAP/1.0 200 OK
               Date: Mon, 10 Jan 2000  09:55:21 GMT
               Methods: RESPMOD
               Service: FOO Tech Server 1.0
               ISTag: "W3E4R7U9-L2E4-2"
               Encapsulated: null-body=0
               Max-Connections: 1000
               Options-TTL: 7200
               Allow: 204
               Preview: 2048
               Transfer-Complete: asp, bat, exe, com
               Transfer-Ignore: html
               Transfer-Preview: *
             */
            #endregion
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ICAP/1.0 200 OK");
            sb.AppendLine("Date: Sat, 18 July 2020  09:55:21 GMT");
            sb.AppendLine("Methods: RESPMOD");
            sb.AppendLine("Service: FOO Tech Server 1.0");
            sb.AppendLine("ISTag: \"W3E4R7U9 - L2E4 - 2\"");
            sb.AppendLine("Encapsulated: null-body=0");
            sb.AppendLine("Max-Connections: 1000");
            sb.AppendLine("Options-TTL: 7200");
            sb.AppendLine("Allow: 204");
            sb.AppendLine("Preview: 2048");
            sb.AppendLine("Transfer-Complete: asp, bat, exe, com");
            sb.AppendLine("Transfer-Ignore: html");
            sb.AppendLine("Transfer-Preview: *");
            sb.AppendLine("\r\n");
            return sb.ToString();
        }
        public static string NotAllowedAnwer(string getPath, string host)
        {
            #region ExampleAnswer
            /* 
               ICAP/1.0 200 OK
               Date: Mon, 10 Jan 2000  09:55:21 GMT
               Server: ICAP-Server-Software/1.0
               Connection: close
               ISTag: "W3E4R7U9-L2E4-2"
               Encapsulated: res-hdr=0, res-body=213

               HTTP/1.1 403 Forbidden
               Date: Wed, 08 Nov 2000 16:02:10 GMT
               Server: Apache/1.3.12 (Unix)
               Last-Modified: Thu, 02 Nov 2000 13:51:37 GMT
               ETag: "63600-1989-3a017169"
               Content-Length: 58
               Content-Type: text/html

               3a
               Sorry, you are not allowed to access that naughty content.
               0
             */
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("ICAP/1.0 200 OK");
            //sb.AppendLine("Date: Sat, 18 July 2020  09:55:21 GMT");
            //sb.AppendLine("Server: ICAP-Server-Software/1.0");
            //sb.AppendLine("Connection: close");
            //sb.AppendLine("ISTag: \"W3E4R7U9 - L2E4 - 2\"");
            //sb.AppendLine("Encapsulated: res-hdr=0, res-body=213");
            //sb.Append("\r\n");
            //sb.AppendLine("HTTP/1.1 403 Forbidden");
            //sb.AppendLine("Date: Wed, 08 Nov 2000 16:02:10 GMT");
            //sb.AppendLine("Server: Apache/1.3.12 (Unix)");
            //sb.AppendLine("Last-Modified: Thu, 02 Nov 2000 13:51:37 GMT");
            //sb.AppendLine("ETag: \"63600 - 1989 - 3a017169\"");
            //sb.AppendLine("Content-Length: 58");
            //sb.AppendLine("Content-Type: text/html");
            //sb.Append("\r\n");
            //sb.AppendLine("3a");
            //sb.AppendLine("Sorry, you are not allowed to access that naughty content.");
            //sb.AppendLine("0");
            //sb.Append("\r\n");
            //return sb.ToString();

            /*
               ICAP/1.0 200 OK
               Date: Mon, 10 Jan 2000  09:55:21 GMT
               Server: ICAP-Server-Software/1.0
               Connection: close
               ISTag: "W3E4R7U9-L2E4-2"
               Encapsulated: req-hdr=0, null-body=231

               GET /modified-path HTTP/1.1
               Host: www.origin-server.com
               Via: 1.0 icap-server.net (ICAP Example ReqMod Service 1.1)
               Accept: text/html, text/plain, image/gif
               Accept-Encoding: gzip, compress
               If-None-Match: "xyzzy", "r2d2xxxx"
             
             */
            #endregion
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ICAP/1.0 200 OK");
            sb.AppendLine("Date: Sat, 18 July 2020  09:55:21 GMT");
            sb.AppendLine("Server: ICAP-Server-Software/1.0");
            sb.AppendLine("Connection: close");
            sb.AppendLine("ISTag: \"W3E4R7U9 - L2E4 - 2\"");
            sb.AppendLine("Encapsulated: res-hdr=0, res-body=231");
            sb.Append("\r\n");
            sb.AppendLine($"GET {getPath} HTTP/1.1");
            sb.AppendLine($"Host: {host}");
            sb.AppendLine("Via: 1.0 icap-server.net (ICAP Example ReqMod Service 1.1)");
            sb.AppendLine("Accept: text/html, text/plain, image/gif");
            sb.AppendLine("Accept-Encoding: gzip, compress");
            sb.AppendLine("If-None-Match: \"xyzzy\", \"r2d2xxxx\"");
            sb.Append("\r\n");
            return sb.ToString();
        }

        public static string NaughtyContent(string getPath, string host)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ICAP/1.0 200 OK");
            sb.AppendLine("Date: Mon, 10 Jan 2000  09:55:21 GMT");
            sb.AppendLine("Server: ICAP-Server-Software/1.0");
            sb.AppendLine("Connection: close");
            sb.AppendLine("ISTag: \"W3E4R7U9-L2E4-2\"");
            sb.AppendLine("Encapsulated: res-hdr=0, res-body=213");
            sb.Append("\r\n");
            sb.AppendLine($"HTTP/1.1 403 Forbidden");
            sb.AppendLine($"Date: Wed, 08 Nov 2000 16:02:10 GMT");
            sb.AppendLine("Server: Apache/1.3.12 (Unix)");
            sb.AppendLine("Last-Modified: Thu, 02 Nov 2000 13:51:37 GMT");
            sb.AppendLine("ETag: \"63600-1989-3a017169\"");
            sb.AppendLine("Content-Length: 58");
            sb.AppendLine("Content-Type: text/html");
            sb.Append("\r\n");
            sb.AppendLine("3a");
            sb.AppendLine("Sorry, you are not allowed to access that naughty content.");
            sb.AppendLine("0");
            return sb.ToString();
        }
    }
}
