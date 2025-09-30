using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class ForbiddenHexesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.ForbiddenHexes;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("nX")]
        public int X { get; set; } = 0;

        [Column("nY")]
        public int Y { get; set; } = 0;

        [Column("strName")]
        public string Name { get; set; }
    }
}
