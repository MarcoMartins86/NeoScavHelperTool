using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels.Base
{
    public abstract class DataTypeModelBase
    {
        [Column("isOverriden")]
        public bool IsOverriden { get; set; } = false;

        [Column("valueModFolder")]
        public string ValueModFolder { get; set; }

        // this data below is just to help building better errors during DB insertion
        protected abstract DataType DataType { get; }
        protected abstract string Identifier { get; }

        public string ItemDescription()
        {
            return $"\"{DataType}\" item with id: \"{Identifier}\"";
        }
    }
}
