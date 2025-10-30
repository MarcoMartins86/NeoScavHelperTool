using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using SQLite;

namespace NeoScavHelperTool.Models.DataTypeModels
{
    public class HeadlinesItem : DataTypeModelBase
    {
        protected override DataType DataType => DataType.Headlines;
        public override string Identifier => Id.ToString();
        public override string DisplayName
        {
            get
            {
                int index = Headline.IndexOf("\n");
                return index == -1 ? Headline : Headline.Substring(0, index);
            }
        }

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strHeadline")]
        public string Headline { get; set; }
    }
}
