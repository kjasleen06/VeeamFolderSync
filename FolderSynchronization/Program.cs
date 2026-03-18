using System;
using System.IO;

namespace FolderSynchronization
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1️⃣ Validate and parse arguments
            if (!ArgumentsValidator.TryParse(args, out string sourcePath, out string replicaPath, out int intervalSeconds, out string logFilePath))
                return;

            // 2️⃣ Initialize logger
            Logger logger = Logger.Create(logFilePath);

            // 3️⃣ Validate source folder
            if (!Directory.Exists(sourcePath))
            {
                logger.Warning($"Source folder does not exist: {sourcePath}");
                return;
            }

            logger.Debug("Folder synchronization started...");

            // 4️⃣ Flow only: you can now call FolderSynchronizer here
            // Example: var synchronizer = new FolderSynchronizer(logger);
            //          synchronizer.SyncFolders(sourcePath, replicaPath);
        }
    }

    public static class ArgumentsValidator
    {
        public static bool TryParse(string[] args, out string sourcePath, out string replicaPath, out int intervalSeconds, out string logFilePath)
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

        private static bool TryParseInterval(string input, out int intervalSeconds)
        {
            intervalSeconds = 0;

            if (!int.TryParse(input, out intervalSeconds) || intervalSeconds <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"⚠ Invalid interval '{input}'. Must be a positive integer.");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return false;
            }

            return true;
        }

        private static void ShowUsage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠ {errorMessage}");
            Console.ResetColor();
            Console.WriteLine("Usage: FolderSynchronization.exe <sourceFolder> <replicaFolder> <intervalSeconds> <logFilePath>");
            Console.WriteLine(@"Example: FolderSynchronization.exe C:\Source C:\Replica 5 C:\Logs\sync_log.txt");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}