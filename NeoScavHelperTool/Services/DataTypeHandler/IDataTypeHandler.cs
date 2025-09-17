using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandler
{
    public interface IDataTypeHandler
    {
        public string Table { get; }
        public void LoadModIntoDb(string gamePath, ModInfo mod, DataTypeAttribute attribute);
    }
}
