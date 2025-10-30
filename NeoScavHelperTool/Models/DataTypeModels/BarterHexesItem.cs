using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class BarterHexesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.BarterHexes;
        public override string Identifier => Id.ToString();
        public override string DisplayName => $"({X},{Y})";

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("nX")]
        public int X { get; set; } = 0;

        [Column("nY")]
        public int Y { get; set; } = 0;

        [Column("bBuys")]
        public bool Buys { get; set; } = false;

        [Column("nRestockTreasureID")]
        public int RestockTreasureId { get; set; } = 3;
    }
}
