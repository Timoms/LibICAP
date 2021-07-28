using LibICAP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace LibICAP.Utilities
{
    class ResponseBuilder
    {
        public static string OPTIONSAnswer()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ICAP/1.0 200 OK");
            sb.AppendLine($"Date: {Tools.GetCurrentICAPDate()}");
            sb.AppendLine("Methods: RESPMOD, REQMOD");
            sb.AppendLine("Service: LibICAP 1.0");
            sb.AppendLine("ISTag: \"W3E4R7U9 - L2E4 - 2\"");
            sb.AppendLine("Encapsulated: null-body=0");
            sb.AppendLine("Max-Connections: 10");
            sb.AppendLine("Options-TTL: 7200");
            sb.AppendLine("Allow: 204");
            sb.AppendLine("Preview: 8196");
            sb.AppendLine("Transfer-Complete: asp, bat, exe, com");
            sb.AppendLine("Transfer-Ignore: html");
            sb.AppendLine("Transfer-Preview: *");
            sb.AppendLine("\r\n");
            return sb.ToString();
        }

        public enum Resp { Unmodified = 204, NotAllowed = 405, MovedPermanently = 301, Example = 0}
        
        public static string Return(Resp resp, string originReq, string filter)
        {
            if (resp == Resp.Unmodified)
            {
                // res-hdr: ResponseHeader starts right after this message
                // res-body: ResponseBody starts after the header, thus ResponseHeader byte length
                // req-hdr=0 -> if reqmod so no reqheader redirected
                // null-body
                StringBuilder ICAPHeader = new StringBuilder();
                ICAPHeader.AppendLine("ICAP/1.0 200 OK");
                ICAPHeader.AppendLine("Server: LibICAP/0.1.0");
                ICAPHeader.AppendLine($"Date: {Tools.GetCurrentICAPDate()}");
                ICAPHeader.AppendLine("Connection: close");
                ICAPHeader.AppendLine("ISTag: W3E4R7U9-L2E4-2"); // ddd MMM dd yyyy HH:mm:ss 'GMT'
                ICAPHeader.AppendLine($"Encapsulated: req-hdr=0, null-body={originReq.ToString().CountBytes() + 2}"); 

                StringBuilder ResponseBuilder = new StringBuilder();

                ResponseBuilder.Append(ICAPHeader);
                ResponseBuilder.Append("\r\n");
                ResponseBuilder.Append(originReq + "\r\n");
                ResponseBuilder.Append("\r\n");

                return ResponseBuilder.ToString();
            } 
            else if (resp == Resp.NotAllowed) 
            {
                var template = File.ReadAllText(@"./Models/Template.html"); //TODO: remove hardcoded path. Expl: used to modify template on the fly
                template = template.Replace("{AAP_POLICY}", filter);

                HTMLResponse response = new HTMLResponse
                {
                    Content = template
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
                ResponseHeader.AppendLine($"Date: {Tools.GetCurrentICAPDate()}");
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
                ICAPHeader.AppendLine($"Date: {Tools.GetCurrentICAPDate()}");
                ICAPHeader.AppendLine("Server: LibICAP/1.0");
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
            else if (resp == Resp.MovedPermanently)
            {
                StringBuilder ResponseHeader = new StringBuilder();
                ResponseHeader.AppendLine($"HTTP/1.1 301 Moved Permanently");
                ResponseHeader.AppendLine($"Location: http://www.example.org/index.asp");
                //ResponseHeader.Append("\r\n");


                StringBuilder ICAPHeader = new StringBuilder();
                ICAPHeader.AppendLine("ICAP/1.0 200 OK");
                ICAPHeader.AppendLine($"Date: {Tools.GetCurrentICAPDate()}");
                ICAPHeader.AppendLine("Server: LibICAP/1.0");
                ICAPHeader.AppendLine("Connection: close");
                ICAPHeader.AppendLine("ISTag: \"W3E4R7U9-L2E4-2\"");
                ICAPHeader.AppendLine($"Encapsulated: res-hdr=0, res-body={ResponseHeader.ToString().CountBytes() + 2}");

                StringBuilder ResponseBuilder = new StringBuilder();
                ResponseBuilder.Append(ICAPHeader);
                ResponseBuilder.Append("\r\n");
                ResponseBuilder.Append(ResponseHeader);
                ResponseBuilder.Append("\r\n");
                ResponseBuilder.Append("\r\n");
            }
            else if (resp == Resp.Example)
            {
                #region example
                //HTMLResponse response = new HTMLResponse
                //{
                //    Content = ""
                //};

                //StringBuilder ResponseBody = new StringBuilder();
                //ResponseBody.AppendLine("HTTP/1.1 200 OK");
                //ResponseBody.AppendLine("Date: Mon, 10 Jan 2000  09:55:21 GMT");
                //ResponseBody.AppendLine("Via: 1.0 icap.example.org (ICAP Example RespMod Service 1.1)");
                //ResponseBody.AppendLine("Server: Apache/1.3.6 (Unix)");
                //ResponseBody.AppendLine("ETag: \"63840-1ab7-378d415b\"");
                //ResponseBody.AppendLine("Content-Type: text/html");
                //ResponseBody.AppendLine($"Content-Length: {response.Length}");


                //StringBuilder ResponseFooter = new StringBuilder();
                //ResponseFooter.AppendLine($"{response.HexLength}");
                //ResponseFooter.AppendLine(response.Content);
                //ResponseFooter.AppendLine("0");

                //StringBuilder ResponseHeader = new StringBuilder();
                //ResponseHeader.AppendLine("ICAP/1.0 200 OK");
                //ResponseHeader.AppendLine($"Date: {Tools.GetCurrentICAPDate()}");
                //ResponseHeader.AppendLine("Server: LibICAP/1.0");
                //ResponseHeader.AppendLine("Connection: close");
                //ResponseHeader.AppendLine("ISTag: \"W3E4R7U9-L2E4-2\"");
                //ResponseHeader.AppendLine($"Encapsulated: res-hdr=0, res-body={ResponseBody.ToString().CountBytes() + 2}");


                //StringBuilder ResponseBuilder = new StringBuilder();

                //ResponseBuilder.Append(ResponseHeader);
                //ResponseBuilder.Append("\r\n");
                //ResponseBuilder.Append(ResponseBody);
                //ResponseBuilder.Append("\r\n");
                //ResponseBuilder.Append(ResponseFooter);
                //ResponseBuilder.Append("\r\n");
                //ResponseBuilder.Append("\r\n");

                //return ResponseBuilder.ToString();

                #endregion

                string[] split = originReq.Split("\r\n\r\n"); // 0 = HTTP/1.1, 1 = <html>...
                string content = split[2];

                HTMLResponse response = new HTMLResponse
                {
                    Content = content
                };

                // 0, 1
                //StringBuilder ResponseBody = new StringBuilder(); 
                //ResponseBody.AppendLine("HTTP/1.1 301 Moved Permanently");
                //ResponseBody.AppendLine("Location: http://example.com/");
                //ResponseBody.AppendLine("Via: 1.0 icap.example.org (ICAP Example RespMod Service 1.1)");
                //ResponseBody.AppendLine("Server: Apache/1.3.6 (Unix)");
                //ResponseBody.AppendLine("ETag: \"63840-1ab7-378d415b\"");
                //ResponseBody.AppendLine("Content-Type: text/html");
                //ResponseBody.AppendLine($"Content-Length: {response.Length}");

                // 0,2 
                StringBuilder ResponseFooter = new StringBuilder();
                ResponseFooter.AppendLine($"{response.HexLength}");
                ResponseFooter.AppendLine(response.Content);
                ResponseFooter.AppendLine("0");

                StringBuilder ResponseHeader = new StringBuilder();
                ResponseHeader.AppendLine("ICAP/1.0 200 OK");
                ResponseHeader.AppendLine($"Date: {Tools.GetCurrentICAPDate()}");
                ResponseHeader.AppendLine("Server: LibICAP/1.0");
                ResponseHeader.AppendLine("Connection: close");
                ResponseHeader.AppendLine("ISTag: \"W3E4R7U9-L2E4-2\"");
                ResponseHeader.AppendLine($"Encapsulated: res-hdr=0, res-body={split[0].CountBytes() + 2}");
                // http://google.de
                
                StringBuilder ResponseBuilder = new StringBuilder();

                ResponseBuilder.Append(ResponseHeader);
                ResponseBuilder.Append("\r\n");
                ResponseBuilder.Append(split[0]);
                ResponseBuilder.Append("\r\n");
                ResponseBuilder.Append(ResponseFooter);
                ResponseBuilder.Append("\r\n");
                ResponseBuilder.Append("\r\n");

                string m  = ResponseBuilder.ToString();

                return ResponseBuilder.ToString();

            }
            return "";

        }
    }
}
