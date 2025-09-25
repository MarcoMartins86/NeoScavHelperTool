using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class IngredientsItem : DataTypeModelBase
    {
        [PrimaryKey, Column("nID")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strRequiredProps")]
        public string RequiredProps { get; set; }

        [Column("strForbidProps")]
        public string ForbidProps { get; set; }
    }
}
