using LibICAP.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibICAP
{
    public class ICAP
    {
        /// <summary>
        /// Constructs a new ICAP Server instance handling all the logic.
        /// </summary>
        /// <param name="ip">IP Address for binding</param>
        /// <param name="port">Port to bind</param>
        /// <param name="buffer">Length of the buffer used in receiving data. Should be at least 1024-20000. Default: 2048.</param>
        public ICAP(string ip, int port, int buffer)
        {           
            Thread workerThread = new Thread(() => {
                new Server(ip, port, buffer);
            });
            workerThread.Start();
        }
    }
}
