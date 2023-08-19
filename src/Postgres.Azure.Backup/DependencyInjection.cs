using Postgres.Azure.Backup.BlobServices;

namespace Postgres.Azure.Backup
{
    public static class DependencyInjection
    {
        public static void AddAutomaticBackup(this IServiceCollection services)
        {

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<BackupService>>();
            services.AddSingleton(typeof(ILogger), logger);

            var storageConfiguration = new StorageConfigurations
            {
                ConnectionString = Environment.GetEnvironmentVariable("AZURE_BLOBS_CONNECTION"),
                ContainerName = Environment.GetEnvironmentVariable("AZURE_BLOBS_CONTAINER")
            };

            services.AddSingleton(storageConfiguration);
            services.AddScoped<IStorageService, StorageService>();


            var autoBackupConfigurations = new AutoBackupConfiguration
            {
                BackupFilePath = Environment.GetEnvironmentVariable("BACKUP_FILE_PATH"),
                Databases = Environment.GetEnvironmentVariable("DATABASES")!.Split(","),
                Host = Environment.GetEnvironmentVariable("HOST"),
                Password = Environment.GetEnvironmentVariable("PASSWORD"),
                PgDumpPath = Environment.GetEnvironmentVariable("PG_DUMP_PATH"),
                Port = Environment.GetEnvironmentVariable("PORT"),
                RepeatInHours = Environment.GetEnvironmentVariable("BACKUP_REPEAT_IN_HOURS"),
                UserName = Environment.GetEnvironmentVariable("USER_NAME")
            };
            services.AddSingleton(autoBackupConfigurations);
            services.AddHostedService<BackupBackgroundService>();
            services.AddScoped<BackupService>();
        }
    }
}
