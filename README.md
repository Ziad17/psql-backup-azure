# psql-backup-azure

A utility tool that takes automated backups of postgres databases using "pg_dump" tool and store them in an azure blob container

### Getting started

change the environment variables in the docker image files with your own settings, then spin up the image.

```powershell
docker-compose up -d
```

### Additional Http Request

you can trigger the backup manually rather than waiting for the duration by calling the POST endpoint "/backup" which will trigger the backup process.

### Final output

backup snapshots are stored in the container as a zip file with the DB name and the timestamp.

