using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.Models
{
    public class ModInfo
    {
        public string Name { get; set; }
        public string Folder { get; set; }
        public ISet<DataType> Files { get; set; }
        public IList<ImageInfo> Images { get; set; }
    }
}
