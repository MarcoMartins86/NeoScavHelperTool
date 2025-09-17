using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Resolvers;
using System.Xml.Schema;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandler
{
    public abstract class DataTypeBaseService : IDataTypeHandler
    {
        private static readonly XmlSchemaSet _schemas;

        static DataTypeBaseService()
        {
            //Resources
            var uri = new Uri("/Resources/Schemas/neogame.xsd", UriKind.Relative);
            var file = App.GetResourceStream(uri);
            // WpfResourceXmlResolverHelper is not currently being used
            // still let's keep it just in case we need it in the future
            _schemas = new XmlSchemaSet() { XmlResolver = new WpfResourceXmlResolverHelper() { } };
            _schemas.Add(null, XmlReader.Create(file.Stream));
        }

        public abstract string Table { get; }

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
                xml.Load(reader);

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

        public abstract void LoadModIntoDb(
            string gamePath,
            ModInfo mod,
            DataTypeAttribute attribute
        );
    }
}
