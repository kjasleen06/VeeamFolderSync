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

            // If the user-provided log file does not exist, use backup mechanism to create a log file in the executable folder
            if (!File.Exists(logFilePath))
            {
                string executionFolder = AppDomain.CurrentDomain.BaseDirectory;
                string backupLogFile = Path.Combine(executionFolder, "sync_log.txt");

                Console.WriteLine("User-provided log file does not exist or is inaccessible.");
                Console.WriteLine($"Using backup log file in executable folder: {backupLogFile}");

                logFilePath = backupLogFile;

                // Ensure the backup file exists
                if (!File.Exists(logFilePath))
                    File.WriteAllText(logFilePath, $"Log created at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n");
            }

            Console.WriteLine($"Log file path: {logFilePath}");
            return new Log(logFilePath);
        }

        public void Debug(string message) => LogMessage(message, ConsoleColor.White);
        public void Warning(string message) => LogMessage(message, ConsoleColor.Yellow);

        private void LogMessage(string message, ConsoleColor color)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
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