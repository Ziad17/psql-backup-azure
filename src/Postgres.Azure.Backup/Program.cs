using Postgres.Azure.Backup;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAutomaticBackup();

var app = builder.Build();

app.MapPost("/backup", async (BackupService service, ILogger logger) =>
{
    logger.LogInformation("began manual backup of database");

    await service.BackupAsync();
    return "Done";
});

app.Run();


