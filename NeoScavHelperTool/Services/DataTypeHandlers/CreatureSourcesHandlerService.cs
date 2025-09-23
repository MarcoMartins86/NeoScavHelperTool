using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Models.DataTypeModels;
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class CreatureSourcesHandlerService
        : NewModBaseHandlerService<CreatureSourcesHandlerService, CreatureSourcesItem>
    {
        public const string TABLE = "creaturesources";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `nX` INTEGER NOT NULL DEFAULT -1, 
              `nY` INTEGER NOT NULL DEFAULT -1,
              `nCreatureID` INTEGER NOT NULL DEFAULT 0,
              `nMin` INTEGER NOT NULL DEFAULT 0,
              `nMax` INTEGER NOT NULL DEFAULT 0,
              `fWeight` REAL NOT NULL DEFAULT 1,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_creaturesources` (
              `id`,
              `strName`,
              `nX`,
              `nY`,
              `nCreatureID`,
              `nMin`,
              `nMax`,
              `fWeight`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @nX,
              @nY,
              @nCreatureID,
              @nMin,
              @nMax,
              @fWeight,
              @isOverriden,
              @valueModFolder
            );
            ";

        public CreatureSourcesHandlerService(
            ILogger<CreatureSourcesHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(
            ref CreatureSourcesItem item,
            XmlElement column
        )
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "id":
                    item.Id = int.Parse(value);
                    break;
                case "strName":
                    item.Name = value;
                    break;
                case "nX":
                    item.X = int.Parse(value);
                    break;
                case "nY":
                    item.Y = int.Parse(value);
                    break;
                case "nCreatureID":
                    item.CreatureId = int.Parse(value);
                    break;
                case "nMin":
                    item.Min = int.Parse(value);
                    break;
                case "nMax":
                    item.Max = int.Parse(value);
                    break;
                case "fWeight":
                    item.Weight = double.Parse(value);
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            CreatureSourcesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@nX", item.X },
                { "@nY", item.Y },
                { "@nCreatureID", item.CreatureId },
                { "@nMin", item.Min },
                { "@nMax", item.Max },
                { "@fWeight", item.Weight },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
