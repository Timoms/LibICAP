using LibICAP.Models;
using LibICAP.Utilities;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            try
            {
                TCPServer = new TcpListener(ServerIPAdress, Port);
                TCPServer.Start();
                Out.Log(LogLevel.Debug, "Starting server");

                while (true) // Enter the listening loop. Locks the current instance.
                {
                    Out.Log(LogLevel.Debug, "Waiting for connection");

                    // Perform a blocking call to accept requests. Could also use server.AcceptSocket() here.
                    TcpClient client = TCPServer.AcceptTcpClient();
                    
                    Out.Log(LogLevel.Debug, $"Client {client.Client.RemoteEndPoint} connected");

                    Data = null; // Resets data after each received stream
                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();
                    // Loop to receive all the data sent by the client.
                    
                    int i;
                    
                    while ((i = stream.Read(Buffer, 0, Buffer.Length)) != 0)
                    {
                        if (!stream.DataAvailable) Out.Log(LogLevel.Warning, "Stream saturated and no data available.", false);
                        // Translate data bytes to a ASCII string.
                        Data = Encoding.ASCII.GetString(Buffer, 0, BufferLength);
                        Out.Log(LogLevel.Answer, $"Received: {Data}", true);

                        // Handle answering the call using parser's response byte array
                        TCPRequest req = Parser.ParseData(Data);
                        if (req != null && req.Response != null)
                        {
                            stream.Write(req.Response, 0, req.Response.Length);
                            Out.Log(LogLevel.Default, $"Sent {req.Type} answer");
                            Out.Log(LogLevel.Debug, $"Sent \r\n{Encoding.UTF8.GetString(req.Response)}", true);
                            //client.Close();
                        }
                        else
                        {
                            Out.Log(LogLevel.Warning, "Waiting for second message after REQ");
                        }
                    }

                    // Shutdown and end connection
                    Out.Log(LogLevel.Debug, "Closing connection");
                    client.Close();
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


    }
}
