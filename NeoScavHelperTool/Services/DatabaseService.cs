using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Models;
using SQLite;

namespace NeoScavHelperTool.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IOptions<AppOptions> _options;
        private SQLiteConnection _connection = null;
        public SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    Connect();
                }
                return _connection;
            }
            private set => _connection = value;
        }

        public DatabaseService(
            ILogger<DatabaseService> logger,
            IConfiguration configuration,
            IOptions<AppOptions> options
        )
        {
            _logger = logger;
            _configuration = configuration;
            _options = options;
        }

        private void Connect()
        {
            string connectionStringValue = _configuration.GetConnectionString(
                _options.Value.SQLite
            );
            if ("file".Equals(_options.Value.SQLite))
            {
                string fullPathToFile = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    connectionStringValue
                );
                if (File.Exists(fullPathToFile))
                {
                    File.Delete(fullPathToFile);
                    _logger.LogTrace("Deleted old db file");
                }
            }

            SQLiteConnectionString connectionString = new SQLiteConnectionString(
                connectionStringValue
            );
            _logger.LogDebug(
                "Connecting to DB: \"{connectionString}\"",
                connectionString.DatabasePath
            );
            _connection = new SQLiteConnection(connectionString);
            // Create all the tables that are not dynamically generated
            _connection.CreateTables(CreateFlags.None, typeof(ModMetadata));
            _logger.LogDebug("Successfully connected to DB");
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
