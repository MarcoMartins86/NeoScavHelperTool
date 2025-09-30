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
    public class CreaturesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Creatures;
        protected override string Identifier => Id.ToString();

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

        [Column("nTreasureID")]
        public int TreasureId { get; set; } = 3;

        [Column("nFaction")]
        public int Faction { get; set; } = 0;

        [Column("vAttackModes")]
        public string AttackModes { get; set; }

        [Column("vBaseConditions")]
        public string BaseConditions { get; set; }

        [Column("nCorpseID")]
        public int CorpseId { get; set; } = 3;

        [Column("vActivities")]
        public string Activities { get; set; }
    }
}
