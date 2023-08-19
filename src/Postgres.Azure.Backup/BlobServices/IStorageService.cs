namespace Postgres.Azure.Backup.BlobServices
{
    public interface IStorageService
    {
        Task<string> UploadAsync(byte[] file, string fileName, string destination = null,
            CancellationToken cancellationToken = default);
    }
}
