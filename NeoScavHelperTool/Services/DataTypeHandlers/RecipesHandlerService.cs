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
    public class RecipesHandlerService
        : NewModBaseHandlerService<RecipesHandlerService, RecipesItem>
    {
        public const string TABLE = "recipes";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `nID` INTEGER NOT NULL, 
              `strName` TEXT NOT NULL,
              `strSecretName` TEXT NOT NULL DEFAULT '',
              `strTools` TEXT NOT NULL DEFAULT '',
              `strConsumed` TEXT NOT NULL DEFAULT '',
              `strDestroyed` TEXT NOT NULL DEFAULT '',
              `nTreasureID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */
              `fHours` REAL NOT NULL DEFAULT 0,
              `nReverse` INTEGER NOT NULL DEFAULT 0, 
              `nHiddenID` TEXT NOT NULL DEFAULT '0:0', /* mod:value */
              `bIdentify` INTEGER NOT NULL DEFAULT 0,
              `bTransferComponents` INTEGER NOT NULL DEFAULT 0,
              `vAlsoTry` TEXT NOT NULL, 
              `nTempTreasureID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */
              `bDegradeOutput` INTEGER NOT NULL DEFAULT 1,
              `strType` TEXT NOT NULL DEFAULT '', /* added default value, mods are working without this */
              `bScrap` INTEGER NOT NULL DEFAULT 1, 
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`nID`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_recipes` (
              `nID`,
              `strName`,
              `strSecretName`,
              `strTools`,
              `strConsumed`,
              `strDestroyed`,
              `nTreasureID`,
              `fHours`,
              `nReverse`,
              `nHiddenID`,
              `bIdentify`,
              `bTransferComponents`,
              `vAlsoTry`,
              `nTempTreasureID`,
              `bDegradeOutput`,
              `strType`,
              `bScrap`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @nID,
              @strName,
              @strSecretName,
              @strTools,
              @strConsumed,
              @strDestroyed,
              @nTreasureID,
              @fHours,
              @nReverse,
              @nHiddenID,
              @bIdentify,
              @bTransferComponents,
              @vAlsoTry,
              @nTempTreasureID,
              @bDegradeOutput,
              @strType,
              @bScrap,
              @isOverriden,
              @valueModFolder
            );
            ";

        public RecipesHandlerService(
            ILogger<RecipesHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(ref RecipesItem item, XmlElement column)
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
                case "strSecretName":
                    item.SecretName = value;
                    break;
                case "strTools":
                    item.Tools = value;
                    break;
                case "strConsumed":
                    item.Consumed = value;
                    break;
                case "strDestroyed":
                    item.Destroyed = value;
                    break;
                case "nTreasureID":
                    item.TreasureId = ModRefValue<int>.Parse(value);
                    break;
                case "fHours":
                    item.Hours = double.Parse(value);
                    break;
                case "nReverse":
                    item.Reverse = string.IsNullOrEmpty(value) /* a mod was like this */
                        ? 0
                        : int.Parse(value);
                    break;
                case "nHiddenID":
                    item.HiddenId = ModRefValue<int>.Parse(value);
                    break;
                case "bIdentify":
                    item.Identify = int.Parse(value) == 1;
                    break;
                case "bTransferComponents":
                    item.TransferComponents = int.Parse(value) == 1;
                    break;
                case "vAlsoTry":
                    item.AlsoTry = value;
                    break;
                case "nTempTreasureID":
                    item.TempTreasureId = ModRefValue<int>.Parse(value);
                    break;
                case "bDegradeOutput":
                    item.DegradeOutput = int.Parse(value) == 1;
                    break;
                case "strType":
                    item.Type = value;
                    break;
                case "bScrap":
                    item.Scrap = int.Parse(value) == 1;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            RecipesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@nID", item.Id },
                { "@strName", item.Name },
                { "@strSecretName", item.SecretName },
                { "@strTools", item.Tools },
                { "@strConsumed", item.Consumed },
                { "@strDestroyed", item.Destroyed },
                { "@nTreasureID", item.TreasureId },
                { "@fHours", item.Hours },
                { "@nReverse", item.Reverse },
                { "@nHiddenID", item.HiddenId },
                { "@bIdentify", item.Identify },
                { "@bTransferComponents", item.TransferComponents },
                { "@vAlsoTry", item.AlsoTry },
                { "@nTempTreasureID", item.TempTreasureId },
                { "@bDegradeOutput", item.DegradeOutput },
                { "@strType", item.Type },
                { "@bScrap", item.Scrap },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
