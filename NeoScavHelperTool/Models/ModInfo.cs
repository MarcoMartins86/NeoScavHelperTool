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
        public static string DATA_FOLDER = "data";

        public string Name { get; set; }
        public string Url { get; set; }
        public List<DataType> Files { get; set; }
        public List<ImageInfo> Images { get; set; }
    }
}
