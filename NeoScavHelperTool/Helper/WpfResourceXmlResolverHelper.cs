using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NeoScavHelperTool.Helper
{
    public class WpfResourceXmlResolverHelper : XmlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (!absoluteUri.IsFile)
            {
                throw new Exception($"Uri \"{absoluteUri}\" is not a file");
            }

            var uri = new Uri(
                $"/Resources/Schemas/{Path.GetFileName(absoluteUri.AbsolutePath)}",
                UriKind.Relative
            );
            var file = App.GetResourceStream(uri);
            return file.Stream;
        }
    }
}
