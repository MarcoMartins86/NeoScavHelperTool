using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;
using static System.Net.Mime.MediaTypeNames;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class EncounterTriggersItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.EncounterTriggers;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("nEncounterID")]
        public int EncounterId { get; set; }

        [Column("fChance")]
        public double Chance { get; set; }

        [Column("bLocBased")]
        public bool LocBased { get; set; }

        [Column("bDateBased")]
        public bool DateBased { get; set; }

        [Column("bHexBased")]
        public bool HexBased { get; set; }

        [Column("bUnique")]
        public bool Unique { get; set; }

        [Column("bAIPassable")]
        public bool AIPassable { get; set; } = true;

        [Column("aArea")]
        public string Area { get; set; }

        [Column("dateMin")]
        public string DateMin { get; set; }

        [Column("dateMax")]
        public string DateMax { get; set; }

        [Column("aHexTypes")]
        public string HexTypes { get; set; }
    }
}
