using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LibICAP.Models;
using LibICAP.Utilities;
using static LibICAP.Models.TypeDefinition;
using static LibICAP.Utilities.Logger;

namespace LibICAP.Models
{
    public class TcpPackage
    {
        public TcpPackage(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            Logger.Log(LogLevel.Verbose, $"Client {TcpClient.Client.RemoteEndPoint} connected");
            ReadStream(1024);
        }
        private readonly Logger Logger = new Logger(LogLevel.All);
        public TcpClient TcpClient { get; set; }
        public NetworkStream NetworkStream { get { return TcpClient.GetStream(); } }
        public T_ICAP Type = T_ICAP.Unset;

        public List<byte[]> ChunkParts = new List<byte[]>();
        public byte[] FullContent = new byte[] { };

        public byte[] ICAPHeader { 
            get 
            {
                return FullContent.SeparateAfter(new byte[] { 0x0D, 0x0A, 0x0D, 0x0A })[0];
            } 
        }
        public byte[] HTTPHeader
        {
            get
            {
                return FullContent.SeparateAfter(new byte[] { 0x0D, 0x0A, 0x0D, 0x0A })[1];
            }
        }
        public byte[] HTTPBody;
        public bool StreamReadable { get { return NetworkStream.CanRead; } }
        public bool DataAvailable { get { return NetworkStream.DataAvailable; } }
        public bool Compressed { get { return ContainsGZip || ContainsGZip; } }
        public bool ContainsGZip { get { return StreamByteContent.SearchBool(Header.GZIP); }}
        public bool ContainsBrotli { get { return StreamByteContent.SearchBool(Header.Brotli); }}

        public string StreamTextContent;
        public byte[] StreamByteContent;

        /// <summary>
        /// Checks the presence of <<0; ieof\r\n\r\n>> or <<0\r\n\r\n>>, if found the client will not be sending any more chunk data.
        /// </summary>
        public bool ReceivedAll { get; set; }

        public void RequestChunk()
        {
            NetworkStream.Write(Encoding.UTF8.GetBytes("ICAP/1.0 100 Continue"));
            ReadStream(1024); /// read the received chunks again and concat with the already received ones
        }

        public void OK()
        {
            NetworkStream.Write(Encoding.UTF8.GetBytes("ICAP/1.0 204 No Content"));
        }

        /// <summary>
        /// Read operation of a stream
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="buffer">Initial buffer length</param>
        /// <returns>Byte array with all data from a single stream request</returns>
        public void ReadStream(int buffer)
        {
            if (StreamReadable)
            {
                byte[] data = new byte[buffer];
                using MemoryStream ms = new MemoryStream();
                do
                {
                    NetworkStream.Read(data, 0, data.Length);
                    ms.Write(data);
                    data = new byte[buffer];
                }
                while (NetworkStream.DataAvailable);
                ChunkParts.Add(ms.ToArray()); /// Header will alwas be in the first list!
                FullContent = Extensions.ConcatArrays(FullContent, ms.ToArray());
                //FullContent = ms.ToArray().CombineWith(FullContent);
                //File.WriteAllBytes(@"C:\Users\heckel.timo\Temporary\LibICAP\new\textbased.txt", x);
                //File.WriteAllText(@"C:\Users\heckel.timo\Temporary\LibICAP\new\bytebased.dat", y);
                var x = NetworkStream;
                Thread.Sleep(100);
                //OK();
                if (NetworkStream.DataAvailable == true)
                {
                    RequestChunk();
                }
                else
                {
                    ReceivedAll = true;
                    Parse();
                }
            }
        }

        private void Parse()
        {
            //int L_IcapHeader = FullContent.SeparateAfter(new byte[] { 0x0D, 0x0A, 0x0D, 0x0A })[0].Length + 4;
            //int L_HTTPHeader = FullContent.SeparateAfter(new byte[] { 0x0D, 0x0A, 0x0D, 0x0A })[1].Length + 4;

            //int L_Pre = L_IcapHeader + L_HTTPHeader;
            //HTTPBody = FullContent.Skip(L_Pre).ToArray();



            //List<byte[]> croppedChunks = new List<byte[]>();
            if (ChunkParts.Count == 1)
            {
                int index = ChunkParts[0].Search(new byte[] { 0x1F, 0x8B });
                if (index == -1) /// No GZIP Header present!
                {

                } 
                else
                {
                    ChunkParts[0] = ChunkParts[0].Skip(index).ToArray();
                    ChunkParts[0] = ChunkParts[0].TrimTailingZeros();
                    byte[] uc_complete_message = ChunkParts[0].DecompressGzipStream();
                    var x = uc_complete_message.GetUTF8TextFromByteArray();
                    ResponseBuilder.Return(ResponseBuilder.Resp.Unmodified, x, "");
                }
            }


            if (ChunkParts.Count > 100)
            {
                int index = ChunkParts[0].Search(new byte[] { 0x1F, 0x8B});
                int firstNewline = ChunkParts[0].Search(new byte[] { 0x0D, 0x0A});
                
                //ChunkParts[0] = ChunkParts[0].Skip(index).ToArray();
                //ChunkParts[0] = ChunkParts[0].TrimTailingZeros();
                //ChunkParts[0] = ChunkParts[0].SkipLast(firstNewline).ToArray();

                //index = ChunkParts[0].Search(new byte[] { 0x0D, 0x0A });
                //ChunkParts[1] = ChunkParts[1].Skip(index).ToArray();

                //// Last one ended with 0D 0A 30 0D 0A 0D 0A

                int i = 0;
                ChunkParts.ForEach(e =>
                {
                    File.WriteAllBytes(@"C:\Users\heckel.timo\Temporary\LibICAP\new\bytechunk." + i + ".dat", e);
                    i++;
                });
            }

           


            //File.WriteAllBytes(@"C:\Users\heckel.timo\Temporary\LibICAP\new\allbytebased.dat", FullContent);
            //File.WriteAllBytes(@"C:\Users\heckel.timo\Temporary\LibICAP\new\httpbytebased.dat", HTTPBody);
            //File.WriteAllBytes(@"C:\Users\heckel.timo\Temporary\LibICAP\new\httpbytebased.gz", HTTPBody);
            _ = ";";
        }

        public void Reply()
        {
            Parser.ParseData("" + "\r\n\r\n" + "");
        }

        public bool Terminate()
        {
            try
            {
                NetworkStream.Close();
                TcpClient.Close();
                TcpClient.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
