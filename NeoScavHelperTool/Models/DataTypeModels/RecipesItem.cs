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
    public class RecipesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Recipes;
        public override string Identifier => Id.ToString();

        [PrimaryKey, Column("nID")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strSecretName")]
        public string SecretName { get; set; } = "";

        [Column("strTools")]
        public string Tools { get; set; } = "";

        [Column("strConsumed")]
        public string Consumed { get; set; } = "";

        [Column("strDestroyed")]
        public string Destroyed { get; set; } = "";

        [Ignore]
        public ModRefValue<int> TreasureId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nTreasureID")]
        public string TreasureIdDb
        {
            get => TreasureId?.ToString();
            set => TreasureId = ModRefValue<int>.Parse(value);
        }

        [Column("fHours")]
        public double Hours { get; set; } = 0;

        [Column("nReverse")]
        public int Reverse { get; set; } = 0;

        [Ignore]
        public ModRefValue<int> HiddenId { get; set; } = ModRefValue<int>.Of("0", 0);

        [Column("nHiddenID")]
        public string HiddenIdDb
        {
            get => HiddenId?.ToString();
            set => HiddenId = ModRefValue<int>.Parse(value);
        }

        [Column("bIdentify")]
        public bool Identify { get; set; } = false;

        [Column("bTransferComponents")]
        public bool TransferComponents { get; set; } = false;

        [Column("vAlsoTry")]
        public string AlsoTry { get; set; }

        [Ignore]
        public ModRefValue<int> TempTreasureId { get; set; } = ModRefValue<int>.Of("0", 3);

        [Column("nTempTreasureID")]
        public string TempTreasureIdDb
        {
            get => TempTreasureId?.ToString();
            set => TempTreasureId = ModRefValue<int>.Parse(value);
        }

        [Column("bDegradeOutput")]
        public bool DegradeOutput { get; set; } = true;

        [Column("strType")]
        public string Type { get; set; } = "";

        [Column("bScrap")]
        public bool Scrap { get; set; } = true;
    }
}
