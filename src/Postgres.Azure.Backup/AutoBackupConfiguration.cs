namespace Postgres.Azure.Backup
{
    public class AutoBackupConfiguration
    {
        /// <summary>
        /// Gets or sets the repeat in hours.
        /// </summary>
        /// <value>
        /// The repeat in hours. for example every 24 Hours
        /// </value>
        public string RepeatInHours { get; set; }

        /// <summary>
        /// Gets or sets the host of the database server e.g. (127.0.0.1)
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port of the db server e.g. (default port 5432)
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public string Port { get; set; }


        /// <summary>
        /// Gets or sets the name of the db user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }


        /// <summary>
        /// Gets or sets the db user password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }


        /// <summary>
        /// Gets or sets the backup file path on the machine locally.
        /// </summary>
        /// <value>
        /// The backup file path.
        /// </value>
        public string BackupFilePath { get; set; }


        /// <summary>
        /// Gets or sets the databases.
        /// </summary>
        /// <value>
        /// The databases.
        /// </value>
        public string[] Databases { get; set; }


        /// <summary>
        /// Gets or sets the absolute path of the pg_dump tool on linux e.g. (/var/www/psql/15/bin/pg_dump).
        /// </summary>
        /// <value>
        /// The pg dump path.
        /// </value>
        public string PgDumpPath { get; set; }
    }
}
