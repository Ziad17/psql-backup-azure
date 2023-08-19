namespace Postgres.Azure.Backup
{
    public class BackupBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AutoBackupConfiguration _configuration;
        private readonly ILogger _logger;

        public BackupBackgroundService(IServiceScopeFactory scopeFactory, AutoBackupConfiguration configuration, ILogger logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
            _logger.LogInformation("automatic backup initiated every {0} hour(s)", _configuration.RepeatInHours);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<BackupService>();
                    await service.BackupAsync();
                }

                await Task.Delay(TimeSpan.FromHours(int.Parse(_configuration.RepeatInHours)), stoppingToken);
            }
        }
    }
}
