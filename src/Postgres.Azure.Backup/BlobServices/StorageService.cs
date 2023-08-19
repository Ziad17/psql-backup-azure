using Azure.Storage.Blobs;

namespace Postgres.Azure.Backup.BlobServices
{
    public class StorageService : IStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ILogger _logger;

        public StorageService(StorageConfigurations storageConfigurations, ILogger logger)
        {
            _logger = logger;
            var configurations = storageConfigurations;
            _blobContainerClient = new BlobContainerClient(configurations.ConnectionString, configurations.ContainerName);
        }

        public async Task<string> UploadAsync(byte[] file, string fileName, string destination = null, CancellationToken cancellationToken = default)
        {
            if (file.Length > 0)
            {
                try
                {
                    if (destination is { Length: > 0 })
                    {
                        fileName = $"{destination}/{fileName}";
                    }

                    _logger.LogInformation($"Start uploading a {file.Length / 1024}Kb to storage as {fileName}");

                    var blobClient = _blobContainerClient.GetBlobClient(fileName);
                    await using var stream = new MemoryStream(file);
                    await blobClient.UploadAsync(stream, cancellationToken);
                    stream.Close();
                    _logger.LogInformation("Closing stream.");
                    return blobClient.Uri.AbsoluteUri;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "blob file uploading error");
                    throw new Exception(e.Message);
                }
            }

            return string.Empty;
        }
    }
}
