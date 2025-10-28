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

        [Ignore]
        public StringList<AndModRefValues<int>, int> RequiredProps { get; set; }

        [Column("strRequiredProps")]
        public string RequiredPropsDb
        {
            get => RequiredProps?.ToString();
            set => RequiredProps = StringList<AndModRefValues<int>, int>.Parse(value);
        }

        [Ignore]
        public StringList<AndModRefValues<int>, int> ForbidProps { get; set; }

        [Column("strForbidProps")]
        public string ForbidPropsDb
        {
            get => ForbidProps?.ToString();
            set => ForbidProps = StringList<AndModRefValues<int>, int>.Parse(value);
        }
    }
}
