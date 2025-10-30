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
    public class AttackModesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.AttackModes;
        public override string Identifier => Id.ToString();
        public override string DisplayName => Name;

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strNotes")]
        public string Notes { get; set; } = "";

        [Column("nRange")]
        public int Range { get; set; } = 1;

        [Column("fDamageCut")]
        public double DamageCut { get; set; } = 0;

        [Column("fDamageBlunt")]
        public double DamageBlunt { get; set; } = 0;

        [Column("strChargeProfiles")]
        public string ChargeProfiles { get; set; }

        [Column("nPenetration")]
        public double Penetration { get; set; } = 0;

        [Column("nType")]
        public int Type { get; set; } = 0;

        [Column("strSnd")]
        public string Sound { get; set; }

        [Column("bTransfer")]
        public bool Transfer { get; set; } = false;

        [Column("vAttackerConditions")]
        public string AttackerConditions { get; set; }

        [Column("strIMG")]
        public string Image { get; set; }

        [Column("fMorale")]
        public double Morale { get; set; } = 0.25;

        [Column("strWieldPhrase")]
        public string WieldPhrase { get; set; }

        [Column("vAttackPhrases")]
        public string AttackPhrases { get; set; }
    }
}
