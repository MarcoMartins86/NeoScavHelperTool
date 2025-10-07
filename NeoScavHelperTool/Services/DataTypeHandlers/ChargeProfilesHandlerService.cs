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
using NeoScavHelperTool.Services.DataTypeHandlers.Base;
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class ChargeProfilesHandlerService
        : NewModBaseHandlerService<ChargeProfilesHandlerService, ChargeProfilesItem>
    {
        public const string TABLE = "chargeprofiles";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `nID` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `strItemID` TEXT NOT NULL,
              `fPerUse` REAL NOT NULL DEFAULT 0,
              `fPerHour` REAL NOT NULL DEFAULT 0,
              `fPerHourEquipped` REAL NOT NULL DEFAULT 0,
              `fPerHex` REAL DEFAULT 0,
              `bDegrade` INTEGER NOT NULL DEFAULT 0,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`nID`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_chargeprofiles` (
              `nID`,
              `strName`,
              `strItemID`,
              `fPerUse`,
              `fPerHour`,
              `fPerHourEquipped`,
              `fPerHex`,
              `bDegrade`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @nID,
              @strName,
              @strItemID,
              @fPerUse,
              @fPerHour,
              @fPerHourEquipped,
              @fPerHex,
              @bDegrade,
              @isOverriden,
              @valueModFolder
            );
            ";

        public ChargeProfilesHandlerService(
            ILogger<ChargeProfilesHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(
            ref ChargeProfilesItem item,
            XmlElement column
        )
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "nID":
                    item.Id = int.Parse(value);
                    break;
                case "strName":
                    item.Name = value;
                    break;
                case "strItemID":
                    item.ItemId = value;
                    break;
                case "fPerUse":
                    item.PerUse = double.Parse(value);
                    break;
                case "fPerHour":
                    item.PerHour = double.Parse(value);
                    break;
                case "fPerHourEquipped":
                    item.PerHourEquipped = double.Parse(value);
                    break;
                case "fPerHex":
                    item.PerHex = double.Parse(value);
                    break;
                case "bDegrade":
                    item.Degrade = int.Parse(value) == 1;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            ChargeProfilesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@nID", item.Id },
                { "@strName", item.Name },
                { "@strItemID", item.ItemId },
                { "@fPerUse", item.PerUse },
                { "@fPerHour", item.PerHour },
                { "@fPerHourEquipped", item.PerHourEquipped },
                { "@fPerHex", item.PerHex },
                { "@bDegrade", item.Degrade },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
