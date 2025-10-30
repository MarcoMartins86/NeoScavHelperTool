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
    public class CreaturesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Creatures;
        public override string Identifier => Id.ToString();
        public override string DisplayName => Name;

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strNamePublic")]
        public string NamePublic { get; set; }

        [Column("strNotes")]
        public string Notes { get; set; }

        [Column("strImg")]
        public string Image { get; set; }

        [Column("vEncounterIDs")]
        public string EncounterIds { get; set; }

        [Column("nMovesPerTurn")]
        public int MovesPerTurn { get; set; }

        [Ignore]
        public ModRefValue<int> TreasureId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nTreasureID")]
        public string TreasureIdDb
        {
            get => TreasureId?.ToString();
            set => TreasureId = ModRefValue<int>.Parse(value);
        }

        [Ignore]
        public ModRefValue<int> Faction { get; set; } = ModRefValue<int>.Of("0", 0);

        [Column("nFaction")]
        public string FactionDb
        {
            get => Faction?.ToString();
            set => Faction = ModRefValue<int>.Parse(value);
        }

        [Column("vAttackModes")]
        public string AttackModes { get; set; }

        [Column("vBaseConditions")]
        public string BaseConditions { get; set; }

        [Ignore]
        public ModRefValue<int> CorpseId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nCorpseID")]
        public string CorpseIdDb
        {
            get => CorpseId?.ToString();
            set => CorpseId = ModRefValue<int>.Parse(value);
        }

        [Column("vActivities")]
        public string Activities { get; set; }
    }
}
