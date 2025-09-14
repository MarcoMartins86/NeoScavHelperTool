using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Attributes
{
    public class DataTypeAttribute : Attribute
    {
        public string File { get; set; }
        public ModType Type { get; set; }
    }
}
