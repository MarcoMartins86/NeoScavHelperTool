using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers.Base
{
    public abstract class NewModBaseHandlerService<T, I> : DataTypeBaseHandlerService<T>
        where T : DataTypeBaseHandlerService<T>
        where I : DataTypeModelBase, new()
    {
        protected NewModBaseHandlerService(ILogger<T> logger, DatabaseService dbService)
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

            // Create the DB table if needed
            bool willOverride = !CreateTableIfNotExists(mod);

            // Fetch the tables nodes from the XML file
            XmlNodeList tables = GetTableNodes(doc);

            // Transverse all table nodes that will eventually call the ReadItemIntoDb()
            TrasverseTableNodes(tables, mod, willOverride);

            _logger.LogDebug("\"{mod}\" \"{file}\" was successfuly loaded", mod.Name, file);
        }

        protected void TrasverseTableNodes(XmlNodeList tables, ModInfo mod, bool willOverride)
        {
            _dbService.Connection.BeginTransaction();
            TrasverseTableNodes(
                tables,
                (tableElement, tableName) =>
                    ReadItemIntoDb(tableElement, tableName, mod, willOverride)
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
            if (!Table.Equals(tableName))
            {
                throw new Exception(
                    $"Unexpected table name \"{tableName}\" at \"{typeof(T).Name}\" on file \"{table.BaseURI}\""
                );
            }
            XmlNodeList columnNodes = table.SelectNodes(XML_COLUMN_ELEMENT_NAME);
            TrasverseColumnNodes(columnNodes, mod, willOverride);
        }

        protected void TrasverseColumnNodes(XmlNodeList columns, ModInfo mod, bool willOverride)
        {
            if (columns == null || columns.Count == 0)
            {
                throw new Exception(
                    $"No \"{XML_COLUMN_ELEMENT_NAME}\" elements found in \"{XML_TABLE_ELEMENT_NAME}\""
                );
            }
            I item = new I { ValueModFolder = mod.Folder, IsOverriden = willOverride };
            foreach (XmlNode columnNode in columns)
            {
                if (columnNode.NodeType != XmlNodeType.Element)
                {
                    throw new Exception(
                        $"Unexpected Xml NodeType \"{columnNode.NodeType}\" for \"{XML_COLUMN_ELEMENT_NAME}\" on file \"{columnNode.BaseURI}\""
                    );
                }
                try
                {
                    AssignColumnValueToItem(ref item, (XmlElement)columnNode);
                }
                catch (Exception ex)
                {
                    XmlElement column = (XmlElement)columnNode;
                    throw new Exception(
                        $"Failed to parse \"{column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE)}\": \"{column.InnerText}\" on file \"{columnNode.BaseURI}\" with message: \"{ex.Message}\""
                    );
                }
            }
            SQLiteCommand command = BuildUpsertCommand(
                mod,
                item,
                (sql, parameters) => _dbService.Connection.CreateCommand(sql, parameters)
            );
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Failed to insert \"{mod.Name}\" {item.ItemDescription()} and message: \"{ex.Message}\""
                );
            }
        }

        protected abstract void AssignColumnValueToItem(ref I item, XmlElement columnElement);

        protected abstract SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            I item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        );
    }
}
