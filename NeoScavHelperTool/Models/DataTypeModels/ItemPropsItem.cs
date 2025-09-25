using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class ItemPropsItem : DataTypeModelBase
    {
        [PrimaryKey, Column("nID")]
        public int Id { get; set; }

        [Column("strPropertyName")]
        public string PropertyName { get; set; }
    }
}
