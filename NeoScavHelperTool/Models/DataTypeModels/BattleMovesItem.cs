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
    public class BattleMovesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.BattleMoves;
        public override string Identifier => Id.ToString();

        public override string DisplayName => Name;

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strID")]
        public string strId { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strNotes")]
        public string Notes { get; set; }

        [Column("strSuccess")]
        public string Success { get; set; }

        [Column("strFail")]
        public string Fail { get; set; }

        [Column("strPopUp")]
        public string PopUp { get; set; }

        [Column("vChanceType")]
        public string ChanceType { get; set; } = "0,0,0";

        [Column("vUsConditions")]
        public string UsConditions { get; set; }

        [Column("vThemConditions")]
        public string ThemConditions { get; set; }

        [Column("vPairConditions")]
        public string PairConditions { get; set; }

        [Column("vUsFailConditions")]
        public string UsFailConditions { get; set; }

        [Column("vThemFailConditions")]
        public string ThemFailConditions { get; set; }

        [Column("vPairFailConditions")]
        public string PairFailConditions { get; set; }

        [Column("vUsPreConditions")]
        public string UsPreConditions { get; set; }

        [Column("vThemPreConditions")]
        public string ThemPreConditions { get; set; }

        [Column("nSeeThem")]
        public int SeeThem { get; set; } = 2;

        [Column("nSeeUs")]
        public int SeeUs { get; set; } = 2;

        [Column("bAllOutOfRange")]
        public bool AllOutOfRange { get; set; } = false;

        [Column("bInAttackRange")]
        public bool InAttackRange { get; set; } = false;

        [Column("nMinCharges")]
        public int MinCharges { get; set; } = 0;

        [Column("nMinRange")]
        public int MinRange { get; set; } = -1;

        [Column("nMaxRange")]
        public int MaxRange { get; set; } = -1;

        [Column("nAttackModeType")]
        public int AttackModeType { get; set; } = -1;

        [Column("vHexTypes")]
        public string HexTypes { get; set; }

        [Column("fChance")]
        public double Chance { get; set; } = 1;

        [Column("fPriority")]
        public double Priority { get; set; } = 0;

        [Column("fDetect")]
        public double Detect { get; set; } = 1;

        [Column("fOrder")]
        public double Order { get; set; } = 0.5;

        [Column("fFatigue")]
        public double Fatigue { get; set; } = 0;

        [Column("bApproach")]
        public bool Approach { get; set; } = false;

        [Column("bOffense")]
        public bool Offense { get; set; } = false;

        [Column("bFallBack")]
        public bool FallBack { get; set; } = false;

        [Column("bRetreat")]
        public bool Retreat { get; set; } = false;

        [Column("bPosition")]
        public bool Position { get; set; } = false;

        [Column("bPassive")]
        public bool Passive { get; set; } = false;
    }
}
