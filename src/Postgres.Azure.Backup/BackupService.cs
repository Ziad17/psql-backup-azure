using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Postgres.Azure.Backup.BlobServices;

namespace Postgres.Azure.Backup
{
    public class BackupService
    {
        private readonly ILogger _logger;
        private readonly AutoBackupConfiguration _configuration;
        private readonly IStorageService _storageService;

        public BackupService(ILogger logger, AutoBackupConfiguration configuration, IStorageService storageService)
        {
            _logger = logger;
            _configuration = configuration;
            _storageService = storageService;
        }

        public async Task BackupAsync()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _logger.LogError("Os not supported for db backups, exiting....");

            try
            {
                if (!File.Exists(_configuration.PgDumpPath))
                    _logger.LogError($"Could not find the pg_dump executor in the specified path {_configuration.PgDumpPath}");

                foreach (var database in _configuration.Databases)
                {
                    var fileName = database + "_backup_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH:mm") + ".sql";

                    Directory.CreateDirectory(_configuration.BackupFilePath);
                    var originalFile = File.Create(_configuration.BackupFilePath + "/" + fileName);
                    originalFile.Close();

                    string pgDumpArgs = $"--host={_configuration.Host} --port={_configuration.Port} --username={_configuration.UserName} --file={_configuration.BackupFilePath + "/" + fileName} {database}";

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = _configuration.PgDumpPath,
                        Arguments = pgDumpArgs,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    };

                    Environment.SetEnvironmentVariable("PGPASSWORD", _configuration.Password);

                    using (Process process = new Process { StartInfo = startInfo })
                    {
                        process.Start();
                        await process.WaitForExitAsync();

                        if (process.ExitCode != 0)
                            _logger.LogError($"Error occurred during backup. Exit code: {process.ExitCode} -  Error details: {await process.StandardError.ReadToEndAsync()}");
                    }

                    var file = await File.ReadAllBytesAsync(_configuration.BackupFilePath + "/" + fileName);

                    var compressedFile = CompressByteArray(file, fileName);

                    var url = await _storageService.UploadAsync(compressedFile, fileName + ".zip", "backups/database");

                    Environment.SetEnvironmentVariable("PGPASSWORD", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during backup.");
            }
        }

        private static byte[] CompressByteArray(byte[] byteArray, string fileName)
        {
            using MemoryStream output = new MemoryStream();

            using (ZipArchive archive = new ZipArchive(output, ZipArchiveMode.Create, true))
            {
                // Create a new entry in the zip archive
                ZipArchiveEntry entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

                // Write the byte array data to the zip entry
                using (Stream entryStream = entry.Open())
                {
                    entryStream.Write(byteArray, 0, byteArray.Length);
                }
            }

            return output.ToArray();
        }
    }
}
