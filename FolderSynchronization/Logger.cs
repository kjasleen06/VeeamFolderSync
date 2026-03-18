using System;
using System.IO;

namespace FolderSynchronization
{
    public class Logger
    {
        private readonly string _logFilePath;

        private Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public static Logger Create(string userLogFile)
        {
            string logFile = userLogFile;

            try
            {
                string logDir = Path.GetDirectoryName(logFile);
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                if (!File.Exists(logFile))
                    File.WriteAllText(logFile, $"Log created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n");

                Console.WriteLine($"Log file path: {logFile}");
            }
            catch
            {
                string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
                string fallbackLogFile = Path.Combine(exeFolder, "sync_log.txt");
                File.WriteAllText(fallbackLogFile, $"Log created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n");
                logFile = fallbackLogFile;
                Console.WriteLine("User-provided log file path was invalid or inaccessible.");
                Console.WriteLine($"Using fallback log file in executable folder: {logFile}");
            }

            return new Logger(logFile);
        }

        public void Debug(string message) => Log(message, ConsoleColor.White);
        public void Warning(string message) => Log(message, ConsoleColor.Yellow);

        private void Log(string message, ConsoleColor color)
        {
            string logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}";
            Console.ForegroundColor = color;
            Console.WriteLine(logMessage);
            Console.ResetColor();

            try
            {
                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                Console.WriteLine($"Failed to write to log file: {_logFilePath}");
            }
        }
    }
}