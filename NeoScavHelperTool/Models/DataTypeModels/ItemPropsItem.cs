using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class ItemPropsItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.ItemProps;
        public override string Identifier => Id.ToString();
        public override string DisplayName => PropertyName;

        [PrimaryKey, Column("nID")]
        public int Id { get; set; }

        [Column("strPropertyName")]
        public string PropertyName { get; set; }
    }
}
