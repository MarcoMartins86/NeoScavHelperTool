using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandlers.Base
{
    public interface ILoadingDataTypeHandlerService<T>
    {
        public string Table { get; }
        public void LoadIntoDb(string gamePath, ModInfo mod, DataTypeAttribute attribute);
        public bool CreateTableIfNotExists(ModInfo mod);
        public void ReadItemIntoDb(T context, ModInfo mod, bool willOverride);
    }
}
