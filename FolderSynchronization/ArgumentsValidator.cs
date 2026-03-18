using System;

namespace FolderSynchronization
{
    public static class ArgumentsValidator
    {
        /// <summary>
        /// Parses and validates the command line arguments.
        /// </summary>
        public static bool TryParse(
            string[] args,
            out string sourcePath,
            out string replicaPath,
            out int intervalSeconds,
            out string logFilePath)
        {
            sourcePath = replicaPath = logFilePath = string.Empty;
            intervalSeconds = 0;

            if (args.Length != 4)
            {
                ShowUsage("Incorrect number of arguments.");
                return false;
            }

            sourcePath = args[0];
            replicaPath = args[1];
            logFilePath = args[3];

            if (!TryParseInterval(args[2], out intervalSeconds))
                return false;

            return true;
        }

        /// <summary>
        /// Converts interval string to int and validates it.
        /// </summary>
        private static bool TryParseInterval(string input, out int intervalSeconds)
        {
            intervalSeconds = 0;
            if (!int.TryParse(input, out intervalSeconds) || intervalSeconds <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid interval '{input}'. Must be a positive integer.");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Prints usage instructions.
        /// </summary>
        private static void ShowUsage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{errorMessage}");
            Console.ResetColor();
            Console.WriteLine("Usage: FolderSynchronization.exe <sourceFolder> <replicaFolder> <intervalInSeconds> <logFilePath>");
            Console.WriteLine(@"Example: FolderSynchronization.exe C:\Source C:\Replica 5 C:\Logs\sync_log.txt");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}