using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class TreasureTableItem : DataTypeModelBase
    {
        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("aTreasures")]
        public string Treasures { get; set; }

        [Column("bNested")]
        public bool Nested { get; set; } = false;

        [Column("bSuppress")]
        public bool Suppress { get; set; } = false;

        [Column("bIdentify")]
        public bool Identify { get; set; } = false;
    }
}
