using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Extensions
{
    public static class DataTypeExtension
    {
        public static string GetFilename(this DataType enumVal)
        {
            return enumVal.GetAttributeOfType<DataTypeAttribute>().File;
        }

        public static string GetTable(this DataType enumVal)
        {
            return enumVal.GetAttributeOfType<DataTypeAttribute>().Table;
        }

        public static ModType GetModType(this DataType enumVal)
        {
            return enumVal.GetAttributeOfType<DataTypeAttribute>().Type;
        }
    }
}
