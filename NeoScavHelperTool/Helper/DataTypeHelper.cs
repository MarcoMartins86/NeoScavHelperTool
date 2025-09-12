using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Extension;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Helper
{
    public static class DataTypeHelper
    {
        private static readonly Dictionary<string, DataType> _fileToDataType;

        static DataTypeHelper()
        {
            var dataTypes = Enum.GetValues(typeof(DataType));
            _fileToDataType = new Dictionary<string, DataType>(dataTypes.Length);
            foreach (DataType item in dataTypes)
            {
                _fileToDataType.Add(item.GetAttributeOfType<DataTypeAttribute>().File, item);
            }
        }

        public static bool TryGetDataTypeFromFile(string file, out DataType dataType)
        {
            return _fileToDataType.TryGetValue(Path.GetFileName(file), out dataType);
        }
    }
}
