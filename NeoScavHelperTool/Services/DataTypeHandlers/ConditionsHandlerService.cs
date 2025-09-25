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
    public class ConditionsHandlerService
        : NewModBaseHandlerService<ConditionsHandlerService, ConditionsItem>
    {
        public const string TABLE = "conditions";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL, 
              `strName` TEXT NOT NULL,
              `strDesc` TEXT NOT NULL,
              `aFieldNames` TEXT NOT NULL, 
              `aModifiers` TEXT NOT NULL,
              `aEffects` TEXT NOT NULL,
              `bFatal` INTEGER NOT NULL DEFAULT 0,
              `vIDNext` TEXT NOT NULL DEFAULT '0',
              `fDuration` REAL NOT NULL DEFAULT 0,
              `bPermanent` INTEGER NOT NULL DEFAULT 0,
              `vChanceNext` TEXT NOT NULL DEFAULT '0',
              `bStackable` INTEGER NOT NULL DEFAULT 0,
              `bDisplay` INTEGER NOT NULL DEFAULT 1,
              `bDisplayOther` INTEGER NOT NULL DEFAULT 0, 
              `bDisplayGameOver` INTEGER NOT NULL DEFAULT 1,
              `nColor` INTEGER NOT NULL DEFAULT 0,
              `bResetTimer` INTEGER NOT NULL DEFAULT 1,
              `bRemoveAll` INTEGER NOT NULL DEFAULT 0, 
              `bRemovePostCombat` INTEGER NOT NULL DEFAULT 0,
              `nTransferRange` INTEGER NOT NULL DEFAULT -1,
              `aThresholds` TEXT NOT NULL,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;        
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_conditions` (
              `id`,
              `strName`,
              `strDesc`,
              `aFieldNames`,
              `aModifiers`,
              `aEffects`,
              `bFatal`,
              `vIDNext`,
              `fDuration`,
              `bPermanent`,
              `vChanceNext`,
              `bStackable`,
              `bDisplay`,
              `bDisplayOther`,
              `bDisplayGameOver`,
              `nColor`,
              `bResetTimer`,
              `bRemoveAll`,
              `bRemovePostCombat`,
              `nTransferRange`,
              `aThresholds`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @strDesc,
              @aFieldNames,
              @aModifiers,
              @aEffects,
              @bFatal,
              @vIDNext,
              @fDuration,
              @bPermanent,
              @vChanceNext,
              @bStackable,
              @bDisplay,
              @bDisplayOther,
              @bDisplayGameOver,
              @nColor,
              @bResetTimer,
              @bRemoveAll,
              @bRemovePostCombat,
              @nTransferRange,
              @aThresholds,
              @isOverriden,
              @valueModFolder
            );
            ";

        public ConditionsHandlerService(
            ILogger<ConditionsHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(ref ConditionsItem item, XmlElement column)
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
                case "strDesc":
                    item.Description = value;
                    break;
                case "aFieldNames":
                    item.FieldNames = value;
                    break;
                case "aModifiers":
                    item.Modifiers = value;
                    break;
                case "aEffects":
                    item.Effects = value;
                    break;
                case "bFatal":
                    item.Fatal = int.Parse(value) == 1;
                    break;
                case "vIDNext":
                    item.IdNext = value;
                    break;
                case "fDuration":
                    item.Duration = double.Parse(value);
                    break;
                case "bPermanent":
                    item.Permanent = int.Parse(value) == 1;
                    break;
                case "vChanceNext":
                    item.ChanceNext = value;
                    break;
                case "bStackable":
                    item.Stackable = int.Parse(value) == 1;
                    break;
                case "bDisplay":
                    item.Display = int.Parse(value) == 1;
                    break;
                case "bDisplayOther":
                    item.DisplayOther = int.Parse(value) == 1;
                    break;
                case "bDisplayGameOver":
                    item.DisplayGameOver = int.Parse(value) == 1;
                    break;
                case "nColor":
                    item.Color = int.Parse(value);
                    break;
                case "bResetTimer":
                    item.ResetTimer = int.Parse(value) == 1;
                    break;
                case "bRemoveAll":
                    item.RemoveAll = int.Parse(value) == 1;
                    break;
                case "bRemovePostCombat":
                    item.RemovePostCombat = int.Parse(value) == 1;
                    break;
                case "nTransferRange":
                    item.nTransferRange = int.Parse(value);
                    break;
                case "aThresholds":
                    item.Thresholds = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            ConditionsItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@strDesc", item.Description },
                { "@aFieldNames", item.FieldNames },
                { "@aModifiers", item.Modifiers },
                { "@aEffects", item.Effects },
                { "@bFatal", item.Fatal },
                { "@vIDNext", item.IdNext },
                { "@fDuration", item.Duration },
                { "@bPermanent", item.Permanent },
                { "@vChanceNext", item.ChanceNext },
                { "@bStackable", item.Stackable },
                { "@bDisplay", item.Display },
                { "@bDisplayOther", item.DisplayOther },
                { "@bDisplayGameOver", item.DisplayGameOver },
                { "@nColor", item.Color },
                { "@bResetTimer", item.ResetTimer },
                { "@bRemoveAll", item.RemoveAll },
                { "@bRemovePostCombat", item.RemovePostCombat },
                { "@nTransferRange", item.nTransferRange },
                { "@aThresholds", item.Thresholds },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
