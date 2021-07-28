using LibICAP.Utilities;

using System.Net;

namespace LibICAP
{
    public class Program
    {
        static void Main()
        {
            // LibICAP Baryonic
            _ = new Server("0.0.0.0", 8080, 245000);
        }
    }
}
