FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY src/Postgres.Azure.Backup/*.csproj ./src/Postgres.Azure.Backup/

RUN dotnet restore ./src/Postgres.Azure.Backup/Postgres.Azure.Backup.csproj

COPY . .

RUN dotnet build ./src/Postgres.Azure.Backup/Postgres.Azure.Backup.csproj --configuration Release

RUN dotnet publish ./src/Postgres.Azure.Backup/Postgres.Azure.Backup.csproj --configuration Release --no-restore --output ./publish

FROM postgres:latest AS pg-tools

RUN apt-get update && \
    apt-get install -y postgresql-client && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .
COPY --from=pg-tools /usr/bin/pg_dump /usr/bin/pg_dump
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Postgres.Azure.Backup.dll"]