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
    public class IngredientsItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Ingredients;
        protected override string Identifier => Id.ToString();

        [PrimaryKey, Column("nID")]
        public int Id { get; set; }

        [Column("strName")]
        public string Name { get; set; }

        [Column("strRequiredProps")]
        public StringList<AndModRefValues<int>, int> RequiredProps { get; set; }

        [Column("strForbidProps")]
        public StringList<AndModRefValues<int>, int> ForbidProps { get; set; }
    }
}
