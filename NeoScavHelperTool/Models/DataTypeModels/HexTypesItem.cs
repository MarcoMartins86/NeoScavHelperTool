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
    public class HexTypesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.HexTypes;
        public override string Identifier => Id.ToString();
        public override string DisplayName => Name;

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strDesc")]
        public string Description { get; set; }

        [Column("nTerrainCost")]
        public int TerrainCost { get; set; }

        [Column("nVizLimiter")]
        public int VizLimiter { get; set; }

        [Column("nVizIncrease")]
        public int VizIncrease { get; set; }

        [Column("nTreasureID")]
        public int TreasureId { get; set; }

        [Column("bPassable")]
        public bool Passable { get; set; }

        [Ignore]
        public ModRefValue<int> ScavengeInitialId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nScavengeInitialID")]
        public string ScavengeInitialIdDb
        {
            get => ScavengeInitialId?.ToString();
            set => ScavengeInitialId = ModRefValue<int>.Parse(value);
        }

        [Ignore]
        public ModRefValue<int> ScavengeItemsIdPerHour { get; set; } = ModRefValue<int>.Of("0", 25);

        [Column("nScavengeItemsIDPerHour")]
        public string ScavengeItemsIdPerHourDb
        {
            get => ScavengeItemsIdPerHour?.ToString();
            set => ScavengeItemsIdPerHour = ModRefValue<int>.Parse(value);
        }

        [Column("nCampItems")]
        public int CampItems { get; set; } = 5;

        [Column("vLightLevels")]
        public string LightLevels { get; set; } = "0.57,1.0,0.57,0.15";

        [Ignore]
        public ModRefValue<int> DefaultCampId { get; set; } = ModRefValue<int>.Of("0", 517);

        [Column("nDefaultCampID")]
        public string DefaultCampIdDb
        {
            get => DefaultCampId?.ToString();
            set => DefaultCampId = ModRefValue<int>.Parse(value);
        }

        [Column("nMinRange")]
        public int MinRange { get; set; } = 3;

        [Column("nMaxRange")]
        public int MaxRange { get; set; } = 6;

        [Column("vCondIDs")]
        public string ConditionsIds { get; set; }
    }
}
