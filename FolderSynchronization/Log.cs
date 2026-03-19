using System;
using System.IO;

namespace FolderSynchronization
{
    public class Log
    {
        private readonly string _logFilePath;

        private Log(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public static Log Create(string userLogFilePath)
        {
            string logFilePath = userLogFilePath;

            try
            {
                string logDir = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                if (!File.Exists(logFilePath))
                    File.WriteAllText(logFilePath, $"Log created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n");

                Console.WriteLine($"Log file path: {logFilePath}");
            }
            catch
            {
                string executionFolder = AppDomain.CurrentDomain.BaseDirectory;
                string backupLogFile = Path.Combine(executionFolder, "sync_log.txt");
                File.WriteAllText(backupLogFile, $"Log created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n");
                logFilePath = backupLogFile;
                Console.WriteLine("User-provided log file path was invalid or inaccessible.");
                Console.WriteLine($"Using backup log file in executable folder: {logFilePath}");
            }

            return new Log(logFilePath);
        }

        public void Debug(string message) => LogMessage(message, ConsoleColor.White);
        public void Warning(string message) => LogMessage(message, ConsoleColor.Yellow);

        private void LogMessage(string message, ConsoleColor color)
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