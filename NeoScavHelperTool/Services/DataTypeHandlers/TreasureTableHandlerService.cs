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
using NeoScavHelperTool.Models.ValueObjects;
using NeoScavHelperTool.Services.DataTypeHandlers.Base;
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class TreasureTableHandlerService
        : NewModBaseHandlerService<TreasureTableHandlerService, TreasureTableItem>
    {
        public const string TABLE = "treasuretable";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `aTreasures` TEXT NOT NULL DEFAULT '', /* added default value, mods are working without this */
              `bNested` INTEGER NOT NULL DEFAULT 0,
              `bSuppress` INTEGER NOT NULL DEFAULT 0,
              `bIdentify` INTEGER NOT NULL DEFAULT 0,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_treasuretable` (
              `id`,
              `strName`,
              `aTreasures`,
              `bNested`,
              `bSuppress`,
              `bIdentify`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @aTreasures,
              @bNested,
              @bSuppress,
              @bIdentify,
              @isOverriden,
              @valueModFolder
            );
            ";

        public TreasureTableHandlerService(
            ILogger<TreasureTableHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(
            ref TreasureTableItem item,
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
                case "aTreasures":
                    item.Treasures = StringList<OrModRefValues<string>, string>.Parse(value);
                    break;
                case "bNested":
                    item.Nested = int.Parse(value) == 1;
                    break;
                case "bSuppress":
                    item.Suppress = int.Parse(value) == 1;
                    break;
                case "bIdentify":
                    item.Identify = int.Parse(value) == 1;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            TreasureTableItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@aTreasures", item.Treasures },
                { "@bNested", item.Nested },
                { "@bSuppress", item.Suppress },
                { "@bIdentify", item.Identify },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
