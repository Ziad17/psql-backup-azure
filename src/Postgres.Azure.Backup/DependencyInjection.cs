using Postgres.Azure.Backup.BlobServices;

namespace Postgres.Azure.Backup
{
    public static class DependencyInjection
    {
        public static void AddStorage(this IServiceCollection services, IConfiguration configurations)
        {
            services.Configure<StorageConfigurations>(configurations.GetSection("AzureBlobStorage"));
            services.AddScoped<IStorageService, StorageService>();
        }

        public static void AddAutomaticBackup(this IServiceCollection services, IConfiguration configurations)
        {
            var autoBackupConfigurations = new AutoBackupConfiguration();
            configurations.Bind("Backup", autoBackupConfigurations);

            services.AddHostedService<BackupBackgroundService>();
            services.AddSingleton(autoBackupConfigurations);
            services.AddScoped<BackupService>();
        }
    }
}
