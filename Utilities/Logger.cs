using System;
using System.Collections.Generic;
using System.Text;

namespace LibICAP.Utilities
{
    public class Logger
    {
        public Logger()
        {

        }
        public enum LogLevel { Default = 0, Warning = 1, Error = 2, Debug = 3, Answer = 4 }
        public void Log(LogLevel logLevel, string message, bool markSections = false)
        {
            Console.ResetColor();
            var rand = new Random().Next(1000, 9999);
            string timestamp = DateTime.Now.ToString("hh:mm:ss");

            if (markSections) Console.WriteLine($"\r\n─── {rand} [LibICAP {timestamp}] ─────────────────────────────────────────────");
            
            switch (logLevel)
            {
                case LogLevel.Default:
                    break;
                case LogLevel.Warning:
                    //Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    //Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Debug:
                    //Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogLevel.Answer:
                    Console.WriteLine();
                    //Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
            }
            
            if (!markSections)
            {
                Console.WriteLine($"[LibICAP {timestamp}]: {message}");
            }
            else
            {
                Console.WriteLine(message);
            }


            Console.ResetColor();
            if (markSections) Console.WriteLine($"\r\n─── {rand} [LibICAP {timestamp}] ─────────────────────────────────────────────");
        }
    }
}
