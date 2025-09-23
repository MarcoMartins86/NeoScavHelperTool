using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class ChargeProfilesItem : DataTypeModelBase
    {
        [PrimaryKey, Column("nID")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strItemID")]
        public string ItemId { get; set; }

        [Column("fPerUse")]
        public double PerUse { get; set; } = 0;

        [Column("fPerHour")]
        public double PerHour { get; set; } = 0;

        [Column("fPerHourEquipped")]
        public double PerHourEquipped { get; set; } = 0;

        [Column("fPerHex")]
        public double PerHex { get; set; } = 0;

        [Column("bDegrade")]
        public bool Degrade { get; set; } = false;
    }
}
