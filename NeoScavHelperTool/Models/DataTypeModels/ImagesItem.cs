using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class ImagesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Images;
        public override string Identifier => Name.ToString();

        [PrimaryKey, Column("name")]
        public int Name { get; set; }

        [Column("small")]
        public string Small { get; set; }

        [Column("big")]
        public string Big { get; set; }
    }
}
