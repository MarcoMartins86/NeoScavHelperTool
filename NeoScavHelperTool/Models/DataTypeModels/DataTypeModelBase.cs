using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class DataTypeModelBase
    {
        [Column("isOverriden")]
        public bool IsOverriden { get; set; } = false;

        [Column("valueModFolder")]
        public string ValueModFolder { get; set; }
    }
}
