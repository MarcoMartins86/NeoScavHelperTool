using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Models.DataTypeModels.Base;

namespace NeoScavHelperTool.Services.DataTypeHandlers.Base
{
    public interface IRepositoryDataTypeHandlerService
    {
        public List<DataTypeModelBase> FindAll(string modName);
    }
}
