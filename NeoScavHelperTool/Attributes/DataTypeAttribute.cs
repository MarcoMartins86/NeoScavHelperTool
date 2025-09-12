using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoScavHelperTool.Attributes
{
    public class DataTypeAttribute : Attribute
    {
        public string File { get; set; }
    }
}
