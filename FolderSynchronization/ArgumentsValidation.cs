using System;

namespace FolderSynchronization
{
    public static class ArgumentsValidation
    {
        public static bool TryParse(
            string[] args,
            out string sourceFolderPath,
            out string replicaFolderPath,
            out int intervalInSeconds,
            out string logFilePath)
        {
            sourceFolderPath = replicaFolderPath = logFilePath = string.Empty;
            intervalInSeconds = 0;

            if (args.Length != 4)
            {
                ShowErrorAndUsage("Incorrect number of arguments.");
                return false;
            }

            sourceFolderPath = args[0];
            replicaFolderPath = args[1];
            logFilePath = args[3];

            if (!TryParseInterval(args[2], out intervalInSeconds))
                return false;

            return true;
        }
        private static bool TryParseInterval(string input, out int intervalInSeconds)
        {
            intervalInSeconds = 0;
            if (!int.TryParse(input, out intervalInSeconds) || intervalInSeconds <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid interval value '{input}'. Must be a positive integer.");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return false;
            }
            return true;
        }
        private static void ShowErrorAndUsage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{errorMessage}");
            Console.ResetColor();
            Console.WriteLine("Usage: FolderSynchronization.exe <sourceFolderPath> <replicaFolderPath> <synchronizationIntervalInSeconds> <logFilePath>");
            Console.WriteLine(@"Example: FolderSynchronization.exe C:\Source C:\Replica 5 C:\Logs\sync_log.txt");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}