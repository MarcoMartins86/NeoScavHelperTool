using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using NeoScavHelperTool.Models.ValueObjects;
using SQLite;
using static System.Net.Mime.MediaTypeNames;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class EncountersItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Encounters;
        public override string Identifier => Id.ToString();
        public override string DisplayName => Name;

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strDesc")]
        public string Description { get; set; }

        [Column("strImg")]
        public string Image { get; set; } = "EncBlank.png";

        [Ignore]
        public ModRefValue<int> TreasureId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nTreasureID")]
        public string TreasureIdDb
        {
            get => TreasureId?.ToString();
            set => TreasureId = ModRefValue<int>.Parse(value);
        }

        [Ignore]
        public ModRefValue<int> RemoveTreasureId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nRemoveTreasureID")]
        public string RemoveTreasureIdDb
        {
            get => RemoveTreasureId?.ToString();
            set => RemoveTreasureId = ModRefValue<int>.Parse(value);
        }

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

        [Ignore]
        public ModRefValue<int> ItemsId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nItemsID")]
        public string ItemsIdDb
        {
            get => ItemsId?.ToString();
            set => ItemsId = ModRefValue<int>.Parse(value);
        }

        [Ignore]
        public ModRefValue<int> CreatureId { get; set; } = ModRefValue<int>.Of("0", 0);

        [Column("nCreatureID")]
        public string CreatureIdDb
        {
            get => CreatureId?.ToString();
            set => CreatureId = ModRefValue<int>.Parse(value);
        }

        [Column("ptCreatureHex")]
        public string CreatureHex { get; set; } = "0,0";

        [Column("ptTeleport")]
        public string Teleport { get; set; } = "0,0";

        [Column("ptEditor")]
        public string Editor { get; set; } = "0,0";

        [Ignore]
        public ModRefValue<int> Type { get; set; } = ModRefValue<int>.Of("0", 0);

        [Column("nType")]
        public string TypeDb
        {
            get => Type?.ToString();
            set => Type = ModRefValue<int>.Parse(value);
        }

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
