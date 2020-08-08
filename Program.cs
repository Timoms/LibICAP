using LibICAP.Utilities;

namespace LibICAP
{
    class Program
    {
        public static Logger Out = new Logger();
        static void Main()
        {
            // LibICAP Baryonic
            _ = new Server("10.0.2.212", 1344, 2048);
        }
    }
}
