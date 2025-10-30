using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class GameVarsItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.GameVars;
        public override string Identifier => Name;
        public override string DisplayName => Type;

        [PrimaryKey, Column("strName")]
        public string Name { get; set; }

        [Column("strType")]
        public string Type { get; set; }

        [Column("strValue")]
        public string Value { get; set; }
    }
}
