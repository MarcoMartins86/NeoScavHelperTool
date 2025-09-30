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
    public class EncountersItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Encounters;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strDesc")]
        public string Description { get; set; }

        [Column("strImg")]
        public string Image { get; set; } = "EncBlank.png";

        [Column("nTreasureID")]
        public int TreasureId { get; set; } = 3;

        [Column("nRemoveTreasureID")]
        public int RemoveTreasureId { get; set; } = 3;

        [Column("aConditions")]
        public string Conditions { get; set; } = "1";

        [Column("aPreConditions")]
        public string PreConditions { get; set; } = "";

        [Column("fPrice")]
        public double Price { get; set; } = 0;

        [Column("aResponses")]
        public string Responses { get; set; }

        [Column("aMinimapHexes")]
        public string MinimapHexes { get; set; } = "";

        [Column("bRemoveCreatures")]
        public bool RemoveCreatures { get; set; } = false;

        [Column("bRemoveUsed")]
        public bool RemoveUsed { get; set; } = false;

        [Column("nItemsID")]
        public int ItemsId { get; set; } = 3;

        [Column("nCreatureID")]
        public int CreatureId { get; set; } = 0;

        [Column("ptCreatureHex")]
        public string CreatureHex { get; set; } = "0,0";

        [Column("ptTeleport")]
        public string Teleport { get; set; } = "0,0";

        [Column("ptEditor")]
        public string Editor { get; set; } = "0,0";

        [Column("nType")]
        public int Type { get; set; } = 0;

        [Column("fLootChance")]
        public double LootChance { get; set; } = 0;

        [Column("fAccidentChance")]
        public double AccidentChance { get; set; } = 0;

        [Column("fCreatureChance")]
        public double CreatureChance { get; set; } = 0;

        [Column("vAccidents")]
        public string Accidents { get; set; } = "1";

        [Column("vLoot")]
        public string Loot { get; set; } = "3";
    }
}
