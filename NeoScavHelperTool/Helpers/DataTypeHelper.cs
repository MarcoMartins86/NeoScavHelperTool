using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CommunityToolkit.Mvvm.DependencyInjection;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Extensions;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Helpers
{
    public static class DataTypeHelper
    {
        private static readonly Dictionary<string, DataType> _fileToDataType;
        private static readonly Dictionary<DataType, DataTypeAttribute> _dataTypeToAttributes;
        private static readonly Dictionary<string, Type> _tableToHandler;

        static DataTypeHelper()
        {
            var dataTypes = Enum.GetValues(typeof(DataType));
            _fileToDataType = new Dictionary<string, DataType>(dataTypes.Length);
            _dataTypeToAttributes = new Dictionary<DataType, DataTypeAttribute>(dataTypes.Length);
            _tableToHandler = new Dictionary<string, Type>(dataTypes.Length);
            foreach (DataType item in dataTypes)
            {
                DataTypeAttribute attribute = item.GetAttributeOfType<DataTypeAttribute>();
                if (!string.IsNullOrEmpty(attribute.File))
                {
                    _fileToDataType.Add(attribute.File, item);
                }
                _dataTypeToAttributes.Add(item, attribute);
                if (!string.IsNullOrEmpty(attribute.Table))
                {
                    _tableToHandler.Add(attribute.Table, attribute.Handler);
                }
            }
        }

        public static bool TryGetDataTypeFromFile(string file, out DataType dataType)
        {
            return _fileToDataType.TryGetValue(Path.GetFileName(file), out dataType);
        }

        public static bool TryGetAttributeFromDataType(
            DataType type,
            out DataTypeAttribute attribute
        )
        {
            return _dataTypeToAttributes.TryGetValue(type, out attribute);
        }

        private static bool TryGetHandlerTypeFromTable(string table, out Type handler)
        {
            return _tableToHandler.TryGetValue(table, out handler);
        }

        public static T TryGetHandlerFromTable<T>(string tableName)
            where T : class
        {
            if (!DataTypeHelper.TryGetHandlerTypeFromTable(tableName, out var type))
            {
                throw new Exception($"Unexpected error, unknown tableName: \"{tableName}\"");
            }

            if (Ioc.Default.GetService(type) is T handler)
            {
                return handler;
            }
            else
            {
                throw new Exception(
                    "Unexpected error, check the Handler attribute assignement in DataType enum"
                );
            }
        }
    }
}
