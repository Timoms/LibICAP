using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;

namespace LibICAP.Utilities
{
    public class Logger
    {
        private readonly LogLevel Level = LogLevel.Debug;
        private readonly StreamWriter StreamWriter;
        public Logger(LogLevel logLevel)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Level = logLevel;

            var rand = new Random().Next(100, 9999);
            string timestamp = DateTime.Now.ToString("hh-mm-ss");
            string filePath = @"C:\Users\heckel.timo\Temporary\LibICAP\LibICAP__" + timestamp + "__" + rand + ".log.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            };

            StreamWriter = File.AppendText(filePath);

        }
        public string GetLogLevel()
        {
            return Level switch
            {
                LogLevel.Default => "Default",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                LogLevel.Debug => "Debug",
                LogLevel.Verbose => "Verbose",
                LogLevel.All => "All (Caution, will produce many entries)",
                _ => "Unknown or not set",
            };
        }

        public void LogIntro()
        {
            Console.WriteLine("⯁ Anfection Core Baryonic, Unstable Beta Branch");
        }


        public Logger()
        {

        }
        public enum LogLevel { Default = 0, Warning = 1, Error = 2, Debug = 3, Verbose = 4, All = 1000 }

        /// <summary>
        /// Logs to the console
        /// </summary>
        /// <param name="logLevel">Severity of the entry</param>
        /// <param name="message">Text which will be written to log</param>
        /// <param name="markSections">If true, applies a random ID and section-marks to a specific log entry</param>
        public void Log(LogLevel logLevel, string message, bool markSections = false)
        {
            if (Level >= logLevel)
            {
                //StreamWriter.Write(message);
                //StreamWriter.FlushAsync();
                Console.ResetColor();
                var rand = new Random().Next(1000, 9999);
                string timestamp = DateTime.Now.ToString("hh:mm:ss");
                string cat = "";

                switch (logLevel)
                {
                    case LogLevel.Default:
                        cat = "NORM";
                        break;
                    case LogLevel.Warning:
                        cat = "WARN";
                        //Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        cat = "ERRM";
                        //Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Debug:
                        cat = "DEBG";
                        //Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case LogLevel.Verbose:
                        cat = "VERB";
                        Console.WriteLine();
                        //Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                }

                if (markSections) Console.WriteLine($"\r\n─── {rand} ({timestamp}) [LibICAP {cat}] ─────────────────────────────────────────────");

                if (!markSections)
                {
                    Console.WriteLine($"({timestamp}) [LibICAP {cat}]: {message}");
                }
                else
                {
                    Console.WriteLine(message);
                }

                StreamWriter.WriteLine($"({timestamp}) [LibICAP {cat}]: {message}");
                StreamWriter.Flush();

                Console.ResetColor();
                if (markSections) Console.WriteLine($"\r\n─── {rand} ({timestamp}) [LibICAP {cat}] ─────────────────────────────────────────────");
            }



        }
    }
}
