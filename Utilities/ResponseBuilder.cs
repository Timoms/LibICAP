using LibICAP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
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

        public static string PathOk(string getPath, string host)
        {
            StringBuilder ResponseHeader = new StringBuilder();
            ResponseHeader.AppendLine($"HTTP/1.1 204 No Content");
            ResponseHeader.AppendLine($"Date: Wed, 08 Nov 2000 16:02:10 GMT");
            ResponseHeader.AppendLine("Server: Apache/1.3.12 (Unix)");
            ResponseHeader.AppendLine("Last-Modified: Thu, 02 Nov 2000 13:51:37 GMT");
            ResponseHeader.AppendLine("ETag: \"63600-1989-3a017169\"");
            //ResponseHeader.AppendLine("Via: 1.0 10.0.2.212:1344 LibICAP");
            //ResponseHeader.AppendLine("Accept: */*");
            //ResponseHeader.AppendLine("Accept-Encoding: *");
            //ResponseHeader.AppendLine("If-None-Match: \"xyzzy\", \"r2d2xxxx\"");
            //ResponseHeader.Append("\r\n");

            // res-hdr: ResponseHeader starts right after this message
            // res-body: ResponseBody starts after the header, thus ResponseHeader byte length
            StringBuilder ICAPHeader = new StringBuilder();
            ICAPHeader.AppendLine("ICAP/1.0 200 OK");
            ICAPHeader.AppendLine("Date: Mon, 10 Jan 2000  09:55:21 GMT"); // ddd MMM dd yyyy HH:mm:ss 'GMT'
            ICAPHeader.AppendLine("Server: 10.0.2.212:1344/1.0");
            ICAPHeader.AppendLine("Connection: close");
            //ICAPHeader.AppendLine("ISTag: \"W3E4R7U9-L2E4-2\"");
            ICAPHeader.AppendLine($"Encapsulated: res-hdr=0, res-body={ResponseHeader.ToString().CountBytes() + 2}");
            //ICAPHeader.Append("\r\n");

            StringBuilder ResponseBuilder = new StringBuilder();

            ResponseBuilder.Append(ICAPHeader);
            ResponseBuilder.Append("\r\n");
            ResponseBuilder.Append(ResponseHeader);
            ResponseBuilder.Append("\r\n");

            var x = ResponseBuilder.ToString();

            return ResponseBuilder.ToString();
        }

        public static string DestinationNotAllowed(string getPath, string host)
        {
            HTMLResponse response = new HTMLResponse
            {
                Content = File.ReadAllText(@".\Models\Template.html")
            };

            // Hex-Length: Chunk sizes within an encapsulated body (used to send message in chunks, splitting not needed for a static response)
            StringBuilder ResponseBody = new StringBuilder();
            ResponseBody.AppendLine($"{response.HexLength}");
            ResponseBody.AppendLine(response.Content);
            ResponseBody.AppendLine("0");
            //ResponseBody.Append("\r\n");
            //ResponseBody.Append("\r\n");

            StringBuilder ResponseHeader = new StringBuilder();
            ResponseHeader.AppendLine($"HTTP/1.1 403 Forbidden");
            ResponseHeader.AppendLine($"Date: Wed, 08 Nov 2000 16:02:10 GMT");
            ResponseHeader.AppendLine("Server: Apache/1.3.12 (Unix)");
            ResponseHeader.AppendLine("Last-Modified: Thu, 02 Nov 2000 13:51:37 GMT");
            ResponseHeader.AppendLine("ETag: \"63600-1989-3a017169\"");
            ResponseHeader.AppendLine($"Content-Length: {response.Length}");
            ResponseHeader.AppendLine("Content-Type: text/html");
            //ResponseHeader.Append("\r\n");

            // res-hdr: ResponseHeader starts right after this message
            // res-body: ResponseBody starts after the header, thus ResponseHeader byte length
            StringBuilder ICAPHeader = new StringBuilder();
            ICAPHeader.AppendLine("ICAP/1.0 200 OK");
            ICAPHeader.AppendLine("Date: Mon, 10 Jan 2000  09:55:21 GMT"); // ddd MMM dd yyyy HH:mm:ss 'GMT'
            ICAPHeader.AppendLine("Server: ICAP-Server-Software/1.0");
            ICAPHeader.AppendLine("Connection: close");
            ICAPHeader.AppendLine("ISTag: \"W3E4R7U9-L2E4-2\"");
            ICAPHeader.AppendLine($"Encapsulated: res-hdr=0, res-body={ResponseHeader.ToString().CountBytes() + 2}");
            //ICAPHeader.Append("\r\n");

            StringBuilder ResponseBuilder = new StringBuilder();

            ResponseBuilder.Append(ICAPHeader);
            ResponseBuilder.Append("\r\n");
            ResponseBuilder.Append(ResponseHeader);
            ResponseBuilder.Append("\r\n");
            ResponseBuilder.Append(ResponseBody);
            ResponseBuilder.Append("\r\n");
            ResponseBuilder.Append("\r\n");

            return ResponseBuilder.ToString();
        }
    }
}
