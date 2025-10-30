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

        public abstract string Identifier { get; }
        public abstract string DisplayName { get; }

        // this data below is just to help building better errors during DB insertion
        protected abstract DataType DataType { get; }

        public string ItemErrorDescription()
        {
            return $"\"{DataType}\" item with id: \"{Identifier}\"";
        }
    }
}
