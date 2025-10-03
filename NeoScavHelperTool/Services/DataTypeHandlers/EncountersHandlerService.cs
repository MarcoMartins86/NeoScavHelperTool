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
    public class EncountersHandlerService
        : NewModBaseHandlerService<EncountersHandlerService, EncountersItem>
    {
        public const string TABLE = "encounters";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `strDesc` TEXT NOT NULL, 
              `strImg` TEXT NOT NULL DEFAULT 'EncBlank.png', 
              `nTreasureID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */
              `nRemoveTreasureID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */
              `aConditions` TEXT NOT NULL DEFAULT '1',
              `aPreConditions` TEXT NOT NULL DEFAULT '', 
              `fPrice` REAL NOT NULL DEFAULT 0,
              `aResponses` TEXT NOT NULL,
              `aMinimapHexes` TEXT NOT NULL DEFAULT '',
              `bRemoveCreatures` INTEGER NOT NULL DEFAULT 0,
              `bRemoveUsed` INTEGER NOT NULL DEFAULT 0, 
              `nItemsID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */
              `nCreatureID` TEXT NOT NULL DEFAULT '0:0', /* mod:value */
              `ptCreatureHex` TEXT NOT NULL DEFAULT '0,0', 
              `ptTeleport` TEXT NOT NULL DEFAULT '0,0',
              `ptEditor` TEXT NOT NULL DEFAULT '0,0',
              `nType` TEXT NOT NULL DEFAULT '0:0', /* mod:value */
              `fLootChance` REAL NOT NULL DEFAULT 0,
              `fAccidentChance` REAL NOT NULL DEFAULT 0,
              `fCreatureChance` REAL NOT NULL DEFAULT 0,
              `vAccidents` TEXT NOT NULL DEFAULT '1',
              `vLoot` TEXT NOT NULL DEFAULT '3', 
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_encounters` (
              `id`,
              `strName`,
              `strDesc`,
              `strImg`,
              `nTreasureID`,
              `nRemoveTreasureID`,
              `aConditions`,
              `aPreConditions`,
              `fPrice`,
              `aResponses`,
              `aMinimapHexes`,
              `bRemoveCreatures`,
              `bRemoveUsed`,
              `nItemsID`,
              `nCreatureID`,
              `ptCreatureHex`,
              `ptTeleport`,
              `ptEditor`,
              `nType`,
              `fLootChance`,
              `fAccidentChance`,
              `fCreatureChance`,
              `vAccidents`,
              `vLoot`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @strDesc,
              @strImg,
              @nTreasureID,
              @nRemoveTreasureID,
              @aConditions,
              @aPreConditions,
              @fPrice,
              @aResponses,
              @aMinimapHexes,
              @bRemoveCreatures,
              @bRemoveUsed,
              @nItemsID,
              @nCreatureID,
              @ptCreatureHex,
              @ptTeleport,
              @ptEditor,
              @nType,
              @fLootChance,
              @fAccidentChance,
              @fCreatureChance,
              @vAccidents,
              @vLoot,
              @isOverriden,
              @valueModFolder
            );
            ";

        public EncountersHandlerService(
            ILogger<EncountersHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(ref EncountersItem item, XmlElement column)
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
                case "strImg":
                    item.Image = value;
                    break;
                case "nTreasureID":
                    item.TreasureId = ModRefValue<int>.Parse(value);
                    break;
                case "nRemoveTreasureID":
                    item.RemoveTreasureId = ModRefValue<int>.Parse(value);
                    break;
                case "aConditions":
                    item.Conditions = value;
                    break;
                case "aPreConditions":
                    item.PreConditions = value;
                    break;
                case "fPrice":
                    item.Price = string.IsNullOrEmpty(value) /* a mod was like this */
                        ? 0
                        : double.Parse(value);
                    break;
                case "aResponses":
                    item.Responses = value;
                    break;
                case "aMinimapHexes":
                    item.MinimapHexes = value;
                    break;
                case "bRemoveCreatures":
                    item.RemoveCreatures = int.Parse(value) == 1;
                    break;
                case "bRemoveUsed":
                    item.RemoveUsed = int.Parse(value) == 1;
                    break;
                case "nItemsID":
                    item.ItemsId = ModRefValue<int>.Parse(value);
                    break;
                case "nCreatureID":
                    item.CreatureId = ModRefValue<int>.Parse(value);
                    break;
                case "ptCreatureHex":
                    item.CreatureHex = value;
                    break;
                case "ptTeleport":
                    item.Teleport = value;
                    break;
                case "ptEditor":
                    item.Editor = value;
                    break;
                case "nType":
                    item.Type = ModRefValue<int>.Parse(value);
                    break;
                case "fLootChance":
                    item.LootChance = double.Parse(value);
                    break;
                case "fAccidentChance":
                    item.AccidentChance = double.Parse(value);
                    break;
                case "fCreatureChance":
                    item.CreatureChance = double.Parse(value);
                    break;
                case "vAccidents":
                    item.Accidents = value;
                    break;
                case "vLoot":
                    item.Loot = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            EncountersItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@strDesc", item.Description },
                { "@strImg", item.Image },
                { "@nTreasureID", item.TreasureId?.ToString() },
                { "@nRemoveTreasureID", item.RemoveTreasureId?.ToString() },
                { "@aConditions", item.Conditions },
                { "@aPreConditions", item.PreConditions },
                { "@fPrice", item.Price },
                { "@aResponses", item.Responses },
                { "@aMinimapHexes", item.MinimapHexes },
                { "@bRemoveCreatures", item.RemoveCreatures },
                { "@bRemoveUsed", item.RemoveUsed },
                { "@nItemsID", item.ItemsId?.ToString() },
                { "@nCreatureID", item.CreatureId?.ToString() },
                { "@ptCreatureHex", item.CreatureHex },
                { "@ptTeleport", item.Teleport },
                { "@ptEditor", item.Editor },
                { "@nType", item.Type?.ToString() },
                { "@fLootChance", item.LootChance },
                { "@fAccidentChance", item.AccidentChance },
                { "@fCreatureChance", item.CreatureChance },
                { "@vAccidents", item.Accidents },
                { "@vLoot", item.Loot },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
