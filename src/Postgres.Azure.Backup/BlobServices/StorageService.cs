using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace Postgres.Azure.Backup.BlobServices
{
    public class StorageService : IStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly ILogger<StorageService> _logger;

        public StorageService(IOptions<StorageConfigurations> storageConfigurations, ILogger<StorageService> logger)
        {
            _logger = logger;
            var configurations = storageConfigurations.Value;
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

        public async Task<string> UploadAsync(IFormFile file, string destination = null, CancellationToken cancellationToken = default)
        {
            if (file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString("N") + extension;
                try
                {
                    if (destination is { Length: > 0 })
                    {
                        fileName = $"{destination}/{fileName}";
                    }

                    _logger.LogInformation($"Start uploading a {file.Length / 1024}Kb to storage as {fileName}");

                    var blobClient = _blobContainerClient.GetBlobClient(fileName);
                    await using var stream = file.OpenReadStream();
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

        public async Task<bool> DeleteAsync(string fileUrl, CancellationToken cancellationToken)
        {
            var uri = new Uri(fileUrl);
            var fileLocation = string.Join(string.Empty, uri.Segments[2..]);

            var blobClient = _blobContainerClient.GetBlobClient(fileLocation);
            var response = await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, new BlobRequestConditions(),
                cancellationToken);

            _logger.LogInformation($"file {fileUrl} has been deleted from storage");

            return response.IsError;
        }
    }
}
