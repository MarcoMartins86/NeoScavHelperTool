using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public interface IDataTypeHandlerService
    {
        public string Table { get; }
        public void LoadModIntoDb(string gamePath, ModInfo mod, DataTypeAttribute attribute);
        public bool CreateTableIfNotExists(ModInfo mod);
        public void ReadItemIntoDb(
            XmlElement table,
            string tableName,
            ModInfo mod,
            bool willOverride
        );
    }
}
