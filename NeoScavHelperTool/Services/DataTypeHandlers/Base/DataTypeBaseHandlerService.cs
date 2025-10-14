using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Resolvers;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Helpers;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandlers.Base
{
    public abstract class DataTypeBaseHandlerService<T, C> : IDataTypeHandlerService<C>
        where T : DataTypeBaseHandlerService<T, C>
    {
        public abstract string Table { get; }
        protected abstract string CreateTableSql { get; }

        protected readonly ILogger<T> _logger;
        protected readonly DatabaseService _dbService;

        private readonly ModsMetadataService _modsMetadataService;

        protected DataTypeBaseHandlerService(
            ILogger<T> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
        {
            _logger = logger;
            _dbService = dbService;
            _modsMetadataService = modsMetadataService;
        }

        // returns:
        // - false: if table already existed
        // - true: if table was created
        public bool CreateTableIfNotExists(ModInfo mod)
        {
            string modTableName = $"{mod.Name}_{Table}";
            bool tableAlreadyExists = _dbService.Connection.GetTableInfo(modTableName).Any();
            if (tableAlreadyExists)
            {
                _logger.LogTrace("\"{table} already exists, will override values\"", modTableName);
            }
            else
            {
                _dbService.Connection.ExecuteScalar<int>(
                    string.Format(CreateTableSql, modTableName)
                );
                _logger.LogTrace("\"{table} created\"", modTableName);
                _modsMetadataService.AddTableToMod(mod, Table);
            }

            return !tableAlreadyExists;
        }

        public abstract void LoadIntoDb(string gamePath, ModInfo mod, DataTypeAttribute attribute);
        public abstract void ReadItemIntoDb(C context, ModInfo mod, bool willOverride);
    }
}
