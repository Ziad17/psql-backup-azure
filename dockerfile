FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY src/Postgres.Azure.Backup/*.csproj ./src/Postgres.Azure.Backup/

RUN dotnet restore ./src/Postgres.Azure.Backup/Postgres.Azure.Backup.csproj

COPY . .

RUN dotnet build ./src/Postgres.Azure.Backup/Postgres.Azure.Backup.csproj -C Release

RUN dotnet publish ./src/Postgres.Azure.Backup/Postgres.Azure.Backup.csproj --configuration Release --no-restore --output ./publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Postgres.Azure.Backup.dll"]