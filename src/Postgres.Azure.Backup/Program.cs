using Postgres.Azure.Backup;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddStorage(builder.Configuration);
builder.Services.AddAutomaticBackup(builder.Configuration);

var app = builder.Build();

app.MapPost("/backup", async (BackupService service, ILogger logger) =>
{
    logger.LogInformation("began manual backup of database");

    await service.BackupAsync();
    return "Done";
});

app.Run();


