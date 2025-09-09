using System;
using System.Collections.Generic;
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
        protected SQLiteConnection Connection
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
            SQLiteConnectionString connectionString = new SQLiteConnectionString(
                _configuration.GetConnectionString(_options.Value.SQLite)
            );
            _logger.LogDebug(
                "Connecting to DB: \"{connectionString}\"",
                connectionString.DatabasePath
            );
            _connection = new SQLiteConnection(connectionString);
            _logger.LogDebug("Successfully connected to DB");
        }

        public T Query<T>()
        {
            var cenas = Connection.QueryScalars<int>("Select 1");
            return default(T);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
