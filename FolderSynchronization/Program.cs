using System;
using System.Threading.Tasks;

namespace FolderSynchronization
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Validate and parse arguments
            if (!ArgumentsValidator.TryParse(args,
                out string sourcePath,
                out string replicaPath,
                out int intervalSeconds,
                out string logFilePath))
            {
                return; // exit gracefully if arguments are invalid
            }

            //Initialize logger
            Logger logger = Logger.Create(logFilePath);

            //Check if source folder exists
            if (!System.IO.Directory.Exists(sourcePath))
            {
                logger.Warning($"Source folder does not exist: {sourcePath}");
                return;
            }

            logger.Debug("Folder synchronization started...");

            //Initialize the synchronizer
            var synchronizer = new FolderSynchronizer(logger);

            //Synchronization loop
            while (true)
            {
                try
                {
                    synchronizer.SyncFolders(sourcePath, replicaPath);
                }
                catch (Exception ex)
                {
                    logger.Warning($"Error during sync: {ex.Message}");
                }

                // Wait for the next interval
                await Task.Delay(intervalSeconds * 1000);
            }
        }
    }
}