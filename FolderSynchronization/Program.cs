using System;
using System.Threading.Tasks;

namespace FolderSynchronization
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Validate command line arguments
            if (!ArgumentsValidation.TryParse(args,
                out string sourceFolderPath,
                out string replicFolderaPath,
                out int syncIntervalInSeconds,
                out string logFilePath))
            {
                return;
            }

            // Initialize log class
            Log log = Log.Create(logFilePath);
            log.Debug("Folder synchronization started...");

            // Initialize the FolderSynchronization class
            var folderSynchronization = new FolderSynchronization(log);

            while (true)
            {
                try
                {
                    folderSynchronization.SyncFolders(sourceFolderPath, replicFolderaPath);
                }
                catch (Exception ex)
                {
                    log.Warning($"Error during folder synchronization: {ex.Message}");
                }

                // Wait for the next synchronization interval
                await Task.Delay(syncIntervalInSeconds * 1000);
            }
        }
    }
}