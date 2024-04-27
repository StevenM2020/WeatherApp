using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WeatherApp
{

    internal static  class Logger
    {
        private static bool blnLogging = true;
        private static bool blnDebugging = true; // controls console logging and file logging path
        private static int maxFileLines = 15; // only goes to 15 for testing purposes

        public static async void Log(string message)
        {
            ConsoleLog(message);
            if (!blnLogging)
                return;
            string logFileDebugPath = await SecureStorage.GetAsync("LogFileDebugPath");
            if (logFileDebugPath == null)
            {
                ConsoleLog("logFileDebugPath not found");
                return;
            }
            string logPath = blnDebugging ? Path.Combine(logFileDebugPath, "WeatherApp.log") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WeatherApp.log");
            if (!File.Exists(logPath))
            {
                File.Create(logPath).Close();
            }
            using (var streamWriter = new StreamWriter(logPath, true))
            {
                streamWriter.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
            var lines = File.ReadAllLines(logPath);
            if (lines.Length > maxFileLines)
            {
                File.WriteAllLines(logPath, lines.Skip(lines.Length - maxFileLines).Take(maxFileLines));
            }
        }

        public static void ConsoleLog(string message)
        {
            if (!blnDebugging)
                return;
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }

    }
}