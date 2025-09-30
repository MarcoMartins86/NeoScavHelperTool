using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Resolvers;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandlers.Base
{
    public abstract class DataTypeBaseHandlerService<T> : IDataTypeHandlerService
        where T : DataTypeBaseHandlerService<T>
    {
        protected const string XML_ROOT_ELEMENT_NAME = "pma_xml_export";
        protected const string XML_DATABASE_ELEMENT_NAME = "database";
        protected const string XML_TABLE_ELEMENT_NAME = "table";
        protected const string XML_TABLE_NAME_ATTRIBUTE = "name";
        protected const string XML_COLUMN_ELEMENT_NAME = "column";
        protected const string XML_COLUMN_NAME_ATTRIBUTE = "name";
        private static readonly XmlSchemaSet _schemas;

        public abstract string Table { get; }
        protected abstract string CreateTableSql { get; }

        protected readonly ILogger<T> _logger;
        protected readonly DatabaseService _dbService;

        protected DataTypeBaseHandlerService(ILogger<T> logger, DatabaseService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        static DataTypeBaseHandlerService()
        {
            //Resources
            var uri = new Uri("/Resources/Schemas/neogame.xsd", UriKind.Relative);
            var file = System.Windows.Application.GetResourceStream(uri);
            // WpfResourceXmlResolverHelper is not currently being used
            // still let's keep it just in case we need it in the future
            _schemas = new XmlSchemaSet() { XmlResolver = new WpfResourceXmlResolverHelper() { } };
            _schemas.Add(null, XmlReader.Create(file.Stream));
        }

        protected static XmlDocument CreateXmlDocument(string file)
        {
            // Set the validation settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas = _schemas;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += (sender, e) =>
            {
                throw new Exception(
                    $"Failed to validate XML file \"{file}\" on \"{e.Exception.LineNumber}:{e.Exception.LinePosition}\" with: \"{e.Message}\""
                );
            };

            // Create the XmlReader object.
            using (XmlReader reader = XmlReader.Create(file, settings))
            {
                // Create and load the XmlDocument (will trigger validations)
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(reader);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Validation failed on \"{file}\" with message: \"{ex.Message}\""
                    );
                }

                return xml;
            }
        }

        protected static string GetFileFullPath(
            string gamePath,
            ModInfo mod,
            DataTypeAttribute attribute
        )
        {
            return attribute.Type == ModType.New
                ? Path.Combine(
                    gamePath,
                    mod.Folder,
                    NeoScavPhpParserService.NEW_MOD_TYPE_DATA_FOLDER,
                    attribute.File
                )
                : Path.Combine(gamePath, mod.Folder, attribute.File);
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
            }

            return !tableAlreadyExists;
        }

        protected static XmlNodeList GetTableNodes(XmlDocument doc)
        {
            if (
                doc.DocumentElement == null
                || !XML_ROOT_ELEMENT_NAME.Equals(doc.DocumentElement.Name)
            )
            {
                throw new Exception(
                    $"Invalid XML file \"{doc.BaseURI}\", could not find the root element \"{XML_ROOT_ELEMENT_NAME}\""
                );
            }

            XmlNodeList databases = doc.DocumentElement.SelectNodes(XML_DATABASE_ELEMENT_NAME);
            if (databases == null || databases.Count > 1)
            {
                throw new Exception(
                    $"Invalid XML file \"{doc.BaseURI}\", \"{XML_DATABASE_ELEMENT_NAME}\" was not found or was defined more than once"
                );
            }

            XmlNode databaseNode = databases.Item(0);
            if (databaseNode.NodeType != XmlNodeType.Element)
            {
                throw new Exception(
                    $"Unexpected Xml NodeType \"{databaseNode.NodeType}\" for \"{XML_DATABASE_ELEMENT_NAME}\" on file \"{databaseNode.BaseURI}\""
                );
            }

            return databaseNode.SelectNodes(XML_TABLE_ELEMENT_NAME);
        }

        protected void TrasverseTableNodes(XmlNodeList tables, Action<XmlElement, string> consumer)
        {
            if (tables == null || tables.Count == 0)
            {
                _logger.LogDebug("No tables found on file");
                return;
            }
            foreach (XmlNode tableNode in tables)
            {
                if (tableNode.NodeType != XmlNodeType.Element)
                {
                    throw new Exception(
                        $"Unexpected Xml NodeType \"{tableNode.NodeType}\" for \"{XML_TABLE_ELEMENT_NAME}\" on file \"{tableNode.BaseURI}\""
                    );
                }
                XmlElement tableElement = (XmlElement)tableNode;
                string tableName = tableElement.GetAttribute(XML_TABLE_NAME_ATTRIBUTE);

                consumer.Invoke(tableElement, tableName);
            }
        }

        public abstract void LoadModIntoDb(
            string gamePath,
            ModInfo mod,
            DataTypeAttribute attribute
        );
        public abstract void ReadItemIntoDb(
            XmlElement table,
            string tableName,
            ModInfo mod,
            bool willOverride
        );
    }
}
