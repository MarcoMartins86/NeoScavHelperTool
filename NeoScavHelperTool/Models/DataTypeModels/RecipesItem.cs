using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using static System.Net.Mime.MediaTypeNames;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class RecipesItem : DataTypeModelBase
    {
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

        [Column("nTreasureID")]
        public int TreasureId { get; set; } = 3;

        [Column("fHours")]
        public double Hours { get; set; } = 0;

        [Column("nReverse")]
        public int Reverse { get; set; } = 0;

        [Column("nHiddenID")]
        public int HiddenId { get; set; } = 0;

        [Column("bIdentify")]
        public bool Identify { get; set; } = false;

        [Column("bTransferComponents")]
        public bool TransferComponents { get; set; } = false;

        [Column("vAlsoTry")]
        public string AlsoTry { get; set; }

        [Column("nTempTreasureID")]
        public int TempTreasureId { get; set; } = 3;

        [Column("bDegradeOutput")]
        public bool DegradeOutput { get; set; } = true;

        [Column("strType")]
        public string Type { get; set; }

        [Column("bScrap")]
        public bool Scrap { get; set; } = true;
    }
}
