using System;
using System.IO;

namespace FolderSynchronization
{
    public class FolderSynchronization
    {
        private readonly Log _log;

        public FolderSynchronization(Log log)
        {
            _log = log;
        }

        public void SyncFolders(string sourceFolderPath, string replicaFolderPath)
        {
            if (!Directory.Exists(replicaFolderPath))
            {
                Directory.CreateDirectory(replicaFolderPath);
                _log.Debug($"Created directory: {replicaFolderPath}");
            }

            SyncFiles(sourceFolderPath, replicaFolderPath);
            SyncDirectories(sourceFolderPath, replicaFolderPath);
        }

        private void SyncFiles(string sourceFolderPath, string replicaFolderPath)
        {
            var sourceFilesPath = Directory.GetFiles(sourceFolderPath);
            foreach (var sourceFilePath in sourceFilesPath)
            {
                string sourceFileName = Path.GetFileName(sourceFilePath);
                string replicaFilePath = Path.Combine(replicaFolderPath, sourceFileName);

                try
                {
                    if (!File.Exists(replicaFilePath))
                    {
                        File.Copy(sourceFilePath, replicaFilePath);
                        _log.Debug($"Copied file operation: {sourceFilePath} -> {replicaFilePath}");
                    }
                    else
                    {
                        string sourceFileHash = FileHelper.GetFileHash(sourceFilePath);
                        string replicaFileHash = FileHelper.GetFileHash(replicaFilePath);

                        if (sourceFileHash != replicaFileHash)
                        {
                            File.Copy(sourceFilePath, replicaFilePath, true);
                            _log.Debug($"Updated file operation: {sourceFilePath} -> {replicaFilePath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Warning($"Error in processing file : {ex.Message}");
                }
            }

            var replicaFilesPath = Directory.GetFiles(replicaFolderPath);
            foreach (var replicaFilePath in replicaFilesPath)
            {
                string replicafileName = Path.GetFileName(replicaFilePath);
                string sourceFilePath = Path.Combine(sourceFolderPath, replicafileName);

                if (!File.Exists(sourceFilePath))
                {
                    try
                    {
                        File.Delete(replicaFilePath);
                        _log.Debug($"Deleted file operation : {replicaFilePath}");
                    }
                    catch (Exception ex)
                    {
                        _log.Warning($"Error deleting file {replicaFilePath}: {ex.Message}");
                    }
                }
            }
        }

        private void SyncDirectories(string sourceFolderPath, string replicaFolderPath)
        {
            var sourceFolderSubDirsPath = Directory.GetDirectories(sourceFolderPath);

            foreach (var sourceFolderSubDirPath in sourceFolderSubDirsPath)
            {
                string sourceFolderSubDirName = Path.GetFileName(sourceFolderSubDirPath);
                string replicaFolderSubDirPath = Path.Combine(replicaFolderPath, sourceFolderSubDirName);

                try
                {
                    if (!Directory.Exists(replicaFolderSubDirPath))
                    {
                        Directory.CreateDirectory(replicaFolderSubDirPath);
                        _log.Debug($"Created directory operation : {replicaFolderSubDirPath}");
                    }

                    SyncFolders(sourceFolderSubDirPath, replicaFolderSubDirPath);
                }
                catch (Exception ex)
                {
                    _log.Warning($"Error processing directory : {ex.Message}");
                }
            }

            var replicaFolderSubDirsPath = Directory.GetDirectories(replicaFolderPath);
            foreach (var replicaFolderSubDirPath in replicaFolderSubDirsPath)
            {
                string replicaFolderSubdirName = Path.GetFileName(replicaFolderSubDirPath);
                string sourceFolderSubDirPath = Path.Combine(sourceFolderPath, replicaFolderSubdirName);

                if (!Directory.Exists(sourceFolderSubDirPath))
                {
                    try
                    {
                        Directory.Delete(replicaFolderSubDirPath, true);
                        _log.Debug($"Deleted directory operation : {replicaFolderSubDirPath}");
                    }
                    catch (Exception ex)
                    {
                        _log.Warning($"Error deleting directory : {ex.Message}");
                    }
                }
            }
        }
    }
}