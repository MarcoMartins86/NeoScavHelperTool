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

        [PrimaryKey, Column("id")]
        public int Id { get; set; }

        [Column("strHeadline")]
        public string Headline { get; set; }
    }
}
