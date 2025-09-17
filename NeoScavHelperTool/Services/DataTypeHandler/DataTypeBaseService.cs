using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Resolvers;
using System.Xml.Schema;
using NeoScavHelperTool.Helper;

namespace NeoScavHelperTool.Services.DataTypeHandler
{
    public static class DataTypeBaseService
    {
        private static readonly XmlSchemaSet _schemas;

        static DataTypeBaseService()
        {
            //Resources
            var uri = new Uri("/Resources/Schemas/neogame.xsd", UriKind.Relative);
            var file = App.GetResourceStream(uri);
            // WpfResourceXmlResolverHelper is not being used currently
            // still let's keep it just in case we need it in the future
            _schemas = new XmlSchemaSet() { XmlResolver = new WpfResourceXmlResolverHelper() { } };
            _schemas.Add(null, XmlReader.Create(file.Stream));
        }

        public static XmlDocument LoadDocumentWithValidation(string file)
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
            XmlReader reader = XmlReader.Create(file, settings);

            // Create and load the XmlDocument (will trigger validations)
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);

            return xml;
        }
    }
}
