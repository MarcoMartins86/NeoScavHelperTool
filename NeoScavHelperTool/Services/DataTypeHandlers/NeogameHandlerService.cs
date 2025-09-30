using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Services.DataTypeHandlers.Base;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class NeogameHandlerService : DataTypeBaseHandlerService<NeogameHandlerService>
    {
        public override string Table => throw new NotImplementedException();

        protected override string CreateTableSql => throw new NotImplementedException();

        public NeogameHandlerService(
            ILogger<NeogameHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        public override void LoadModIntoDb(
            string gamePath,
            ModInfo mod,
            DataTypeAttribute attribute
        )
        {
            // Create the XmlDocument from file
            // XSD valitations will run at loading time
            string file = GetFileFullPath(gamePath, mod, attribute);
            XmlDocument doc = CreateXmlDocument(file);

            _logger.LogTrace("\"{mod}\" \"{file}\" validated successfully", mod.Name, file);

            // Fetch the tables nodes from the XML file
            XmlNodeList tables = GetTableNodes(doc);

            // Transverse all table nodes that will eventually call the ReadItemIntoDb()
            TrasverseTableNodes(tables, mod);

            _logger.LogDebug("\"{mod}\" \"{file}\" was successfuly loaded", mod.Name, file);
        }

        protected void TrasverseTableNodes(XmlNodeList tables, ModInfo mod)
        {
            _dbService.Connection.BeginTransaction();
            TrasverseTableNodes(
                tables,
                (tableElement, tableName) =>
                {
                    if (!DataTypeHelper.TryGetHandlerFromTable(tableName, out var type))
                    {
                        throw new Exception(
                            $"Unexpected error, unknown tableName: \"{tableName}\""
                        );
                    }

                    if (Ioc.Default.GetService(type) is IDataTypeHandlerService handler)
                    {
                        // Create the DB table if needed
                        bool willOverride = !handler.CreateTableIfNotExists(mod);
                        handler.ReadItemIntoDb(tableElement, tableName, mod, willOverride);
                    }
                    else
                    {
                        throw new Exception(
                            "Unexpected error, check the Handler attribute assignement in DataType enum"
                        );
                    }
                }
            );
            _dbService.Connection.Commit();
        }

        public override void ReadItemIntoDb(
            XmlElement table,
            string tableName,
            ModInfo mod,
            bool willOverride
        )
        {
            throw new NotImplementedException();
        }
    }
}
