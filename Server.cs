using LibICAP.Models;
using LibICAP.Utilities;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static LibICAP.Utilities.Logger;

namespace LibICAP
{
    public class Server
    {
        /// <summary>
        /// Logging instance.
        /// </summary>
        private readonly Logger Out = new Logger();
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
        }

        private TcpListener TCPServer { get; set; }

        private void InitializeServer()
        {
            
            
            using (WebClient webClient = new WebClient())
            {
                Out.Log(LogLevel.Default, $"Downloading blocklist: Advertising");
                string x = webClient.DownloadString("https://justdomains.github.io/blocklists/lists/adguarddns-justdomains.txt");
                Out.Log(LogLevel.Default, $"Downloading blocklist: Privacy");
                string y = webClient.DownloadString("https://justdomains.github.io/blocklists/lists/easyprivacy-justdomains.txt");

                Parser.AdBlockList = x.Split("\n").ToList();
                Out.Log(LogLevel.Default, $"Loaded blocklist: Advertising (removes most adverts from international webpages, including unwanted frames, images and objects)");

                Parser.PrivacyList = y.Split("\n").ToList();
                Out.Log(LogLevel.Default, $"Loaded blocklist: Privacy (completely removes all forms of tracking from the internet)");
            }



            try
            {
                TCPServer = new TcpListener(ServerIPAdress, Port);
                TCPServer.Start();
                Out.Log(LogLevel.Debug, $"Starting server using {ServerIPAdress}:{Port}");
                Out.Log(LogLevel.Debug, "Waiting for connection");
                while (true) // Enter the listening loop. Locks the current instance.
                {
                    TcpClient client = TCPServer.AcceptTcpClient();
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException)
            {
                Out.Log(LogLevel.Error, "SocketException occured");
            }
            finally
            {
                // Stop listening for new clients.
                Out.Log(LogLevel.Debug, "Stopped listening for clients");
                TCPServer.Stop();
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            Out.Log(LogLevel.Debug, $"Client {client.Client.RemoteEndPoint} connected");
            Data = null; // Resets data after each received stream
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();
            // Wait to receive all the data sent by the client.
            if (stream.CanRead)
            {
                StringBuilder c_completeMessage = new StringBuilder();
                int numberOfBytesRead = 0;
                // Incoming message may be larger than the buffer size. 
                do
                {
                    numberOfBytesRead = stream.Read(Buffer, 0, Buffer.Length);
                    c_completeMessage.AppendFormat("{0}", Encoding.ASCII.GetString(Buffer, 0, numberOfBytesRead));
                }

                while (stream.DataAvailable);

                Out.Log(LogLevel.Debug, c_completeMessage.ToString(), true);
                TCPRequest req = Parser.ParseData(c_completeMessage.ToString());
                if (req != null)
                {
                    stream.Write(req.Response, 0, req.Response.Length);
                    //stream.Flush(); -- might need to be enabled!
                    Out.Log(LogLevel.Default, $"Sent {req.Type} answer");
                    Out.Log(LogLevel.Debug, $"Sent \r\n{Encoding.UTF8.GetString(req.Response)}", true);
                }
                else
                {
                    Out.Log(LogLevel.Warning, "Req was null, investigate if websites not loading correctly");
                }
            }
            else
            {
                Out.Log(LogLevel.Error, "Cannot read from this NetworkStream.");
            }
            // Shutdown and end connection
            Out.Log(LogLevel.Debug, "Closing connection");

            client.GetStream().Close();
            client.Close();
            client.Dispose();
        }
    }
}
