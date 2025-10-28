using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using NeoScavHelperTool.Models.ValueObjects;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class CampTypesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.CampTypes;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strDesc")]
        public string Description { get; set; }

        [Ignore]
        public ModRefValue<string> ImageList { get; set; } =
            ModRefValue<string>.Of("0", "ItmScavengeGrass01.png");

        [Column("vImageList")]
        public string ImageListDb
        {
            get => ImageList?.ToString();
            set => ImageList = ModRefValue<string>.Parse(value);
        }

        [Column("aCapacities")]
        public string Capacities { get; set; } = "30x30";

        [Ignore]
        public ModRefValue<int> TreasureId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nTreasureID")]
        public string TreasureIdDb
        {
            get => TreasureId?.ToString();
            set => TreasureId = ModRefValue<int>.Parse(value);
        }

        [Column("m_fAlertness")]
        public double Alertness { get; set; } = 0;

        [Column("m_fVisibility")]
        public double Visibility { get; set; } = -0.05;

        [Column("WetTempAdjustMod")]
        public double WetTempAdjustMod { get; set; } = 0;

        [Column("m_fHealPerHourMod")]
        public double HealPerHourMod { get; set; } = 0;

        [Column("fSleepQuality")]
        public double SleepQuality { get; set; } = 0;
    }
}
