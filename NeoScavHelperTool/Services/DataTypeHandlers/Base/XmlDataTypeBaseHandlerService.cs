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
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandlers.Base
{
    public abstract class XmlDataTypeBaseHandlerService<T>
        : DataTypeBaseHandlerService<T, XmlElement>
        where T : XmlDataTypeBaseHandlerService<T>
    {
        protected const string XML_ROOT_ELEMENT_NAME = "pma_xml_export";
        protected const string XML_DATABASE_ELEMENT_NAME = "database";
        protected const string XML_TABLE_ELEMENT_NAME = "table";
        protected const string XML_TABLE_NAME_ATTRIBUTE = "name";
        protected const string XML_COLUMN_ELEMENT_NAME = "column";
        protected const string XML_COLUMN_NAME_ATTRIBUTE = "name";
        private static readonly XmlSchemaSet _schemas;

        static XmlDataTypeBaseHandlerService()
        {
            //Resources
            var uri = new Uri("/Resources/Schemas/neogame.xsd", UriKind.Relative);
            var file = System.Windows.Application.GetResourceStream(uri);
            // WpfResourceXmlResolverHelper is not currently being used
            // still let's keep it just in case we need it in the future
            _schemas = new XmlSchemaSet() { XmlResolver = new WpfResourceXmlResolverHelper() { } };
            _schemas.Add(null, XmlReader.Create(file.Stream));
        }

        protected XmlDataTypeBaseHandlerService(
            ILogger<T> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected static XmlDocument CreateXmlDocument(string file)
        {
            // Set the validation settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.Schemas = _schemas;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += (sender, e) =>
            {
                throw new Exception(
                    $"Failed to validate XML file \"{file}\" on \"{e.Exception.LineNumber}:{e.Exception.LinePosition}\" with: \"{e.Message}\""
                );
            };
            // Since many mods have wrong comments we need to remove them
            // Otherwise Microsoft XML parser will complain
            string strXMLFile = File.ReadAllText(file);
            strXMLFile = Regex.Replace(strXMLFile, "<!--[\\s\\S]*?(?=-->)-->", string.Empty);
            // Create the XmlReader object
            using (
                XmlReader reader = XmlReader.Create(new StringReader(strXMLFile), settings, file)
            )
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

        protected static string GetXmlFileFullPath(
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
    }
}
