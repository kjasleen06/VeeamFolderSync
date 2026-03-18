using System;
using System.IO;

namespace FolderSynchronization
{
    public class FolderSynchronizer
    {
        private readonly Logger _logger;

        public FolderSynchronizer(Logger logger)
        {
            _logger = logger;
        }

        public void SyncFolders(string source, string replica)
        {
            if (!Directory.Exists(replica))
            {
                Directory.CreateDirectory(replica);
                _logger.Debug($"Created directory: {replica}");
            }

            SyncFiles(source, replica);
            SyncDirectories(source, replica);
        }

        private void SyncFiles(string source, string replica)
        {
            var sourceFiles = Directory.GetFiles(source);
            foreach (var file in sourceFiles)
            {
                string fileName = Path.GetFileName(file);
                string replicaFile = Path.Combine(replica, fileName);

                try
                {
                    if (!File.Exists(replicaFile))
                    {
                        File.Copy(file, replicaFile);
                        _logger.Debug($"Copied: {file} -> {replicaFile}");
                    }
                    else
                    {
                        string sourceHash = FileHelper.GetFileHash(file);
                        string replicaHash = FileHelper.GetFileHash(replicaFile);

                        if (sourceHash != replicaHash)
                        {
                            File.Copy(file, replicaFile, true);
                            _logger.Debug($"Updated: {file}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning($"Error processing file {file}: {ex.Message}");
                }
            }

            var replicaFiles = Directory.GetFiles(replica);
            foreach (var file in replicaFiles)
            {
                string fileName = Path.GetFileName(file);
                string sourceFile = Path.Combine(source, fileName);

                if (!File.Exists(sourceFile))
                {
                    try
                    {
                        File.Delete(file);
                        _logger.Debug($"Deleted: {file}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"Error deleting file {file}: {ex.Message}");
                    }
                }
            }
        }

        private void SyncDirectories(string source, string replica)
        {
            var sourceDirs = Directory.GetDirectories(source);

            foreach (var dir in sourceDirs)
            {
                string dirName = Path.GetFileName(dir);
                string replicaDir = Path.Combine(replica, dirName);

                try
                {
                    if (!Directory.Exists(replicaDir))
                    {
                        Directory.CreateDirectory(replicaDir);
                        _logger.Debug($"Created directory: {replicaDir}");
                    }

                    SyncFolders(dir, replicaDir);
                }
                catch (Exception ex)
                {
                    _logger.Warning($"Error processing directory {dir}: {ex.Message}");
                }
            }

            var replicaDirs = Directory.GetDirectories(replica);
            foreach (var dir in replicaDirs)
            {
                string dirName = Path.GetFileName(dir);
                string sourceDir = Path.Combine(source, dirName);

                if (!Directory.Exists(sourceDir))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        _logger.Debug($"Deleted directory: {dir}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"Error deleting directory {dir}: {ex.Message}");
                    }
                }
            }
        }
    }
}