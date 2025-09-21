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
    public class BarterHexesHandlerService
        : NewModBaseHandlerService<BarterHexesHandlerService, BarterHexesItem>
    {
        public const string TABLE = "barterhexes";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `nX` INTEGER NOT NULL DEFAULT 0,
              `nY` INTEGER NOT NULL DEFAULT 0,
              `bBuys` INTEGER NOT NULL DEFAULT 0,
              `nRestockTreasureID` INTEGER NOT NULL DEFAULT 3,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_barterhexes` (
              `id`,
              `nX`,
              `nY`,
              `bBuys`,
              `nRestockTreasureID`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @nX,
              @nY,
              @bBuys,
              @nRestockTreasureID,
              @isOverriden,
              @valueModFolder
            );
            ";

        public BarterHexesHandlerService(
            ILogger<BarterHexesHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        public override void ReadItemIntoDb(
            XmlElement table,
            string tableName,
            ModInfo mod,
            bool willOverride
        )
        {
            if (!TABLE.Equals(tableName))
            {
                throw new Exception(
                    $"Unexpected table name \"{tableName}\" at \"{nameof(BarterHexesHandlerService)}\" on file \"{table.BaseURI}\""
                );
            }
            XmlNodeList columnNodes = table.SelectNodes(XML_COLUMN_ELEMENT_NAME);
            TrasverseColumnNodes(columnNodes, mod, willOverride);
        }

        protected override void AssignColumnValueToItem(ref BarterHexesItem item, XmlElement column)
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "id":
                    item.Id = int.Parse(value);
                    break;
                case "nX":
                    item.X = int.Parse(value);
                    break;
                case "nY":
                    item.Y = int.Parse(value);
                    break;
                case "bBuys":
                    item.Buys = int.Parse(value) == 1;
                    break;
                case "nRestockTreasureID":
                    item.RestockTreasureId = int.Parse(value);
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            BarterHexesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@nX", item.X },
                { "@nY", item.Y },
                { "@bBuys", item.Buys },
                { "@nRestockTreasureID", item.RestockTreasureId },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
