using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class DataFilesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.DataFiles;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strDesc")]
        public string Description { get; set; }

        [Column("fValue")]
        public double Value { get; set; } = 0;

        [Column("strImg")]
        public string Image { get; set; }
    }
}
