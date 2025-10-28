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
    public class TreasureTableItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.TreasureTable;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Ignore]
        public StringList<OrModRefValues<string>, string> Treasures { get; set; }

        [Column("aTreasures")]
        public string TreasuresDb
        {
            get => Treasures?.ToString();
            set => Treasures = StringList<OrModRefValues<string>, string>.Parse(value);
        }

        [Column("bNested")]
        public bool Nested { get; set; } = false;

        [Column("bSuppress")]
        public bool Suppress { get; set; } = false;

        [Column("bIdentify")]
        public bool Identify { get; set; } = false;
    }
}
