using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class ConditionsItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Conditions;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strDesc")]
        public string Description { get; set; }

        [Column("aFieldNames")]
        public string FieldNames { get; set; }

        [Column("aModifiers")]
        public string Modifiers { get; set; }

        [Column("aEffects")]
        public string Effects { get; set; }

        [Column("bFatal")]
        public bool Fatal { get; set; } = false;

        [Column("vIDNext")]
        public string IdNext { get; set; } = "0";

        [Column("fDuration")]
        public double Duration { get; set; } = 0;

        [Column("bPermanent")]
        public bool Permanent { get; set; } = false;

        [Column("vChanceNext")]
        public string ChanceNext { get; set; } = "0";

        [Column("bStackable")]
        public bool Stackable { get; set; } = false;

        [Column("bDisplay")]
        public bool Display { get; set; } = true;

        [Column("bDisplayOther")]
        public bool DisplayOther { get; set; } = false;

        [Column("bDisplayGameOver")]
        public bool DisplayGameOver { get; set; } = true;

        [Column("nColor")]
        public int Color { get; set; } = 0;

        [Column("bResetTimer")]
        public bool ResetTimer { get; set; } = true;

        [Column("bRemoveAll")]
        public bool RemoveAll { get; set; } = false;

        [Column("bRemovePostCombat")]
        public bool RemovePostCombat { get; set; } = false;

        [Column("nTransferRange")]
        public int nTransferRange { get; set; } = -1;

        [Column("aThresholds")]
        public string Thresholds { get; set; } = "";
    }
}
