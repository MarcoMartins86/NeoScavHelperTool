using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.Models
{
    public class ModInfo
    {
        public static string VANILLA_MOD_NAME = "0";
        public static string NEW_MOD_TYPE_DATA_FOLDER = "data";

        public string Name { get; set; }
        public string Folder { get; set; }
        public ISet<DataType> Files { get; set; }
        public IList<ImageInfo> Images { get; set; }
    }
}
