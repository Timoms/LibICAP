using LibICAP.Models;
using LibICAP.Utilities;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static LibICAP.Utilities.Logger;

namespace LibICAP
{
    public class Server
    {
        public static volatile int Test = 10;
        /// <summary>
        /// Logging instance.
        /// </summary>
        private readonly Logger Out = new Logger(LogLevel.All);
        public Server _Server;
        private IPAddress ServerIPAdress { get; set; }
        /// <summary>
        /// ICAP Port. Default: 1344
        /// </summary>
        private int Port { get; set; }
        /// <summary>
        /// Buffer for reading data. Default: 1024
        /// </summary>
        private byte[] Buffer { get; set; }
        /// <summary>
        /// Length of the buffer
        /// </summary>
        private int BufferLength { get; set; }
        /// <summary>
        /// Holds the received data
        /// </summary>
        private string Data { get; set; }
        /// <summary>
        /// Initializes a reusable TCPServer class and listens for connections.
        /// </summary>
        /// <param name="ip">IP Adress Binding</param>
        /// <param name="port">Port for the server. Default 1344</param>
        /// <param name="buffer">Buffer length of the tcp stream. Default 1024</param>
        public Server(string ip, int port, int buffer)
        {
            Port = port;
            Buffer = new byte[buffer];
            ServerIPAdress = IPAddress.Parse(ip);
            BufferLength = buffer;
            InitializeServer();
            _Server = this;
        }

        public static string ReturnMeAStaticString()
        {
            return "Hello from LibICAP!";
        }
        public static string DownloadBlockList(List<string> blocklist)
        {
            using (WebClient webClient = new WebClient())
            {
                blocklist.ForEach(bl =>
                {
                    string x = webClient.DownloadString("https://justdomains.github.io/blocklists/lists/adguarddns-justdomains.txt");
                    Parser.AdBlockList = x.Split("\n").ToList();
                });
            }

            return "OK";
        }

        private TcpListener TCPServer { get; set; }

        /// <summary>
        /// Downloads Blocklists, initializes threaded servers and accepts clients.
        /// </summary>
        private void InitializeServer()
        {

            using (WebClient webClient = new WebClient())
            {
                Out.LogIntro();
                Out.Log(LogLevel.Default, $"Downloading blocklist: Advertising");
                string x = webClient.DownloadString("https://justdomains.github.io/blocklists/lists/adguarddns-justdomains.txt");
                Out.Log(LogLevel.Default, $"Downloading blocklist: Privacy");
                string y = webClient.DownloadString("https://justdomains.github.io/blocklists/lists/easyprivacy-justdomains.txt");

                Parser.AdBlockList = x.Split("\n").ToList();
                Out.Log(LogLevel.Default, $"Loaded blocklist: Advertising (removes most adverts)");
                Parser.PrivacyList = y.Split("\n").ToList();
                Out.Log(LogLevel.Default, $"Loaded blocklist: Privacy (removes tracking)");
            }

            try
            {
                TCPServer = new TcpListener(ServerIPAdress, Port);
                TCPServer.Start();
                Out.Log(LogLevel.Default, $"Starting server using {ServerIPAdress}:{Port}");
                Out.Log(LogLevel.Verbose, "Using a buffer length of " + BufferLength + " bytes");
                Out.Log(LogLevel.Default, "Logger of server is set to verbosity: " + Out.GetLogLevel());
                Out.Log(LogLevel.Default, "Server up, waiting for connection");
                while (true) // Enter the listening loop. Locks the current instance.
                {
                    TcpClient client = TCPServer.AcceptTcpClient();
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException ex)
            {
                Out.Log(LogLevel.Error, $"SocketException occured: {ex.Message}");
            }
            finally
            {
                // Stop listening for new clients.
                Out.Log(LogLevel.Default, "Stopped listening for clients");
                TCPServer.Stop();
            }
        }

        /// <summary>
        /// Creates a TcpPackage and leaves it handling the rest.
        /// </summary>
        /// <param name="obj"></param>
        private void HandleClient(object obj)
        {
            // Hand over the client to the package, job of server is done here.
            TcpPackage package = new TcpPackage((TcpClient)obj);
            

            //Data = null; // Resets data after each received stream
            //StringBuilder c_completeMessage = new StringBuilder();
            //string ICAPHeader = "";
            //TCPRequest req = null;


            //// Wait to receive all the data sent by the client.
            //if (stream.CanRead)
            //{
            //    Out.Log(LogLevel.Debug, "Can read stream");
            //    byte[] res = ReadAllPerf(stream, 1024);

            //    if (res.SearchBool(Header.GZIP))
            //    {
            //        Out.Log(LogLevel.Warning, "Handling client.");
            //        Out.Log(LogLevel.Debug, "Stream data read, checking for gzip now");

            //        int breakPoint = res.Search(Header.GZIP); // if -1 no gzip found.
            //        Out.Log(LogLevel.Debug, "Found breakpoint at " + breakPoint);
            //        ICAPHeader = res.SubArray(0, breakPoint).GetUTF8TextFromByteArray();

            //        if (breakPoint != -1)
            //        {
            //            Out.Log(LogLevel.Verbose, $"GZIP_MAGIC found.");

            //            byte[] res2 = res.SubArray(breakPoint, res.Length - breakPoint - 7); // (7 for offset linebreaks, eol, etc)
            //            res2.WriteToFile(@"C:\Users\heckel.timo\Temporary\LibICAP\Buffer_ReadFully_cropped.gz");
            //            try
            //            {
            //                string decom = res2.DecompressGzipStream().GetUTF8TextFromByteArray();
            //                c_completeMessage.Append(decom);
            //                bool containHTML = decom.Contains("html");
            //                Console.WriteLine("Contains HTML Code: " + containHTML);
            //            }
            //            catch (Exception ex)
            //            {
            //                Out.Log(LogLevel.Error, ex.Message);
            //            }
            //        }

            //        req = Parser.ParseData(ICAPHeader + "\r\n\r\n" + c_completeMessage.ToString());
            //    }
            //    else
            //    {
            //        string x = res.GetUTF8TextFromByteArray();
            //        req = Parser.ParseData(x);
            //        Out.Log(LogLevel.Verbose, x, true);
            //    }

            //    // Out.Log(LogLevel.Verbose, c_completeMessage.ToString(), true);
                



            //    if (req != null)
            //    {
            //        stream.Write(req.Response, 0, req.Response.Length);
            //        //stream.Flush(); -- might need to be enabled!
            //        Out.Log(LogLevel.Verbose, $"Sent {req.Type} answer");
            //        Out.Log(LogLevel.Verbose, $"Sent \r\n{Encoding.UTF8.GetString(req.Response)}", true);
            //    }
            //    else
            //    {
            //        Out.Log(LogLevel.Warning, "Req was null, investigate if websites not loading correctly");
            //    }
            //}
            //else
            //{
            //    Out.Log(LogLevel.Error, "Cannot read from this NetworkStream.");
            //}
            //// Shutdown and end connection
            //Out.Log(LogLevel.Debug, "Closing connection");

            //client.GetStream().Close();
            //client.Close();
            //client.Dispose();
        }
    }
}
