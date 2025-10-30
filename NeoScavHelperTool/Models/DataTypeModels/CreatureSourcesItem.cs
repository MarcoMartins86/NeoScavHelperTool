using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class CreatureSourcesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.CreatureSources;
        public override string Identifier => Id.ToString();
        public override string DisplayName => Name;

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("nX")]
        public int X { get; set; } = -1;

        [Column("nY")]
        public int Y { get; set; } = -1;

        [Column("nCreatureID")]
        public int CreatureId { get; set; } = 0;

        [Column("nMin")]
        public int Min { get; set; } = 0;

        [Column("nMax")]
        public int Max { get; set; } = 0;

        [Column("fWeight")]
        public double Weight { get; set; } = 1;
    }
}
