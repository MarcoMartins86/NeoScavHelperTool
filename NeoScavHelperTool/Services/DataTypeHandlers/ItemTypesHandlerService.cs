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
    public class ItemTypesHandlerService
        : NewModBaseHandlerService<ItemTypesHandlerService, ItemTypesItem>
    {
        public const string TABLE = "itemtypes";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `nGroupID` INTEGER NOT NULL, 
              `nSubgroupID` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `strDesc` TEXT NOT NULL,
              `strDescAlt` TEXT NOT NULL,
              `nCondID` TEXT NOT NULL DEFAULT '1', /* mod:value */
              `vImageList` TEXT NOT NULL, 
              `vSpriteList` TEXT NOT NULL, 
              `vImageUsage` TEXT NOT NULL,
              `fWeight` REAL NOT NULL DEFAULT 0,
              `fMonetaryValue` REAL NOT NULL DEFAULT 0, 
              `fMonetaryValueAlt` REAL NOT NULL DEFAULT 0, 
              `fDurability` REAL NOT NULL DEFAULT 1,    
              `fDegradePerHour` REAL NOT NULL DEFAULT 0, 
              `fEquipDegradePerHour` REAL NOT NULL DEFAULT 0,
              `fDegradePerUse` REAL NOT NULL DEFAULT 0, 
              `vDegradeTreasureIDs` TEXT NOT NULL DEFAULT '3,3',
              `aEquipConditions` TEXT NOT NULL,
              `aPossessConditions` TEXT NOT NULL, 
              `aUseConditions` TEXT NOT NULL,
              `aCapacities` TEXT NOT NULL, 
              `vEquipSlots` TEXT NOT NULL, 
              `vUseSlots` TEXT NOT NULL,
              `bSocketLocked` INTEGER NOT NULL DEFAULT 0,
              `vProperties` TEXT NOT NULL, 
              `aContentIDs` TEXT NOT NULL, 
              `nFormatID` TEXT NOT NULL DEFAULT '3', /* mod:value */
              `nTreasureID` TEXT NOT NULL DEFAULT '3', /* mod:value */
              `nComponentID` TEXT NOT NULL DEFAULT '3', /* mod:value */
              `bMirrored` INTEGER NOT NULL DEFAULT 0,
              `nSlotDepth` INTEGER NOT NULL DEFAULT 0,
              `strChargeProfiles` TEXT NOT NULL,
              `aAttackModes` TEXT NOT NULL,
              `nStackLimit` INTEGER NOT NULL DEFAULT 1, 
              `aSwitchIDs` TEXT NOT NULL DEFAULT '',
              `aSounds` TEXT NOT NULL DEFAULT 'cuePickup,cuePutdown',
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_itemtypes` (
              `id`,
              `nGroupID`,
              `nSubgroupID`,
              `strName`,
              `strDesc`,
              `strDescAlt`,
              `nCondID`,
              `vImageList`,
              `vSpriteList`,
              `vImageUsage`,
              `fWeight`,
              `fMonetaryValue`,
              `fMonetaryValueAlt`,
              `fDurability`,
              `fDegradePerHour`,
              `fEquipDegradePerHour`,
              `fDegradePerUse`,
              `vDegradeTreasureIDs`,
              `aEquipConditions`,
              `aPossessConditions`,
              `aUseConditions`,
              `aCapacities`,
              `vEquipSlots`,
              `vUseSlots`,
              `bSocketLocked`,
              `vProperties`,
              `aContentIDs`,
              `nFormatID`,
              `nTreasureID`,
              `nComponentID`,
              `bMirrored`,
              `nSlotDepth`,
              `strChargeProfiles`,
              `aAttackModes`,
              `nStackLimit`,
              `aSwitchIDs`,
              `aSounds`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @nGroupID,
              @nSubgroupID,
              @strName,
              @strDesc,
              @strDescAlt,
              @nCondID,
              @vImageList,
              @vSpriteList,
              @vImageUsage,
              @fWeight,
              @fMonetaryValue,
              @fMonetaryValueAlt,
              @fDurability,
              @fDegradePerHour,
              @fEquipDegradePerHour,
              @fDegradePerUse,
              @vDegradeTreasureIDs,
              @aEquipConditions,
              @aPossessConditions,
              @aUseConditions,
              @aCapacities,
              @vEquipSlots,
              @vUseSlots,
              @bSocketLocked,
              @vProperties,
              @aContentIDs,
              @nFormatID,
              @nTreasureID,
              @nComponentID,
              @bMirrored,
              @nSlotDepth,
              @strChargeProfiles,
              @aAttackModes,
              @nStackLimit,
              @aSwitchIDs,
              @aSounds,
              @isOverriden,
              @valueModFolder
            );
            ";

        public ItemTypesHandlerService(
            ILogger<ItemTypesHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(ref ItemTypesItem item, XmlElement column)
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "id":
                    item.Id = int.Parse(value);
                    break;
                case "nGroupID":
                    item.GroupId = int.Parse(value);
                    break;
                case "nSubgroupID":
                    item.SubgroupId = int.Parse(value);
                    break;
                case "strName":
                    item.Name = value;
                    break;
                case "strDesc":
                    item.Description = value;
                    break;
                case "strDescAlt":
                    item.DescriptionAlternative = value;
                    break;
                case "nCondID":
                    item.ConditionId = ModRefValue<int>.Parse(value);
                    break;
                case "vImageList":
                    item.ImageList = ListModRefValue<string>.Parse(value);
                    break;
                case "vSpriteList":
                    item.SpriteList = value;
                    break;
                case "vImageUsage":
                    item.ImageUsage = value;
                    break;
                case "fWeight":
                    item.Weight = double.Parse(value);
                    break;
                case "fMonetaryValue":
                    item.MonetaryValue = double.Parse(value);
                    break;
                case "fMonetaryValueAlt":
                    item.MonetaryValueAlternative = double.Parse(value);
                    break;
                case "fDurability":
                    item.Durability = double.Parse(value);
                    break;
                case "fDegradePerHour":
                    item.DegradePerHour = double.Parse(value);
                    break;
                case "fEquipDegradePerHour":
                    item.EquipDegradePerHour = double.Parse(value);
                    break;
                case "fDegradePerUse":
                    item.DegradePerUse = double.Parse(value);
                    break;
                case "vDegradeTreasureIDs":
                    item.DegradeTreasureIds = value;
                    break;
                case "aEquipConditions":
                    item.EquipConditions = value;
                    break;
                case "aPossessConditions":
                    item.PossessConditions = value;
                    break;
                case "aUseConditions":
                    item.UseConditions = value;
                    break;
                case "aCapacities":
                    item.Capacities = value;
                    break;
                case "vEquipSlots":
                    item.EquipSlots = value;
                    break;
                case "vUseSlots":
                    item.UseSlots = value;
                    break;
                case "bSocketLocked":
                    item.SocketLocked = int.Parse(value) == 1;
                    break;
                case "vProperties":
                    item.Properties = ListModRefValue<int>.Parse(value);
                    break;
                case "aContentIDs":
                    item.ContentIds = ListModRefValue<int>.Parse(value);
                    break;
                case "nFormatID":
                    item.FormatId = ModRefValue<int>.Parse(value);
                    break;
                case "nTreasureID":
                    item.TreasureId = ModRefValue<int>.Parse(value);
                    break;
                case "nComponentID":
                    item.ComponentId = ModRefValue<int>.Parse(value);
                    break;
                case "bMirrored":
                    item.Mirrored = int.Parse(value) == 1;
                    break;
                case "nSlotDepth":
                    item.SlotDepth = int.Parse(value);
                    break;
                case "strChargeProfiles":
                    item.ChargeProfiles = value;
                    break;
                case "aAttackModes":
                    item.AttackModes = value;
                    break;
                case "nStackLimit":
                    item.StackLimit = int.Parse(value);
                    break;
                case "aSwitchIDs":
                    item.SwitchIds = value;
                    break;
                case "aSounds":
                    item.Sounds = ListModRefValue<string>.Parse(value);
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            ItemTypesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@nGroupID", item.GroupId },
                { "@nSubgroupID", item.SubgroupId },
                { "@strName", item.Name },
                { "@strDesc", item.Description },
                { "@strDescAlt", item.DescriptionAlternative },
                { "@nCondID", item.ConditionId.ToString() },
                { "@vImageList", item.ImageList.ToString() },
                { "@vSpriteList", item.SpriteList },
                { "@vImageUsage", item.ImageUsage },
                { "@fWeight", item.Weight },
                { "@fMonetaryValue", item.MonetaryValue },
                { "@fMonetaryValueAlt", item.MonetaryValueAlternative },
                { "@fDurability", item.Durability },
                { "@fDegradePerHour", item.DegradePerHour },
                { "@fEquipDegradePerHour", item.EquipDegradePerHour },
                { "@fDegradePerUse", item.DegradePerUse },
                { "@vDegradeTreasureIDs", item.DegradeTreasureIds },
                { "@aEquipConditions", item.EquipConditions },
                { "@aPossessConditions", item.PossessConditions },
                { "@aUseConditions", item.UseConditions },
                { "@aCapacities", item.Capacities },
                { "@vEquipSlots", item.EquipSlots },
                { "@vUseSlots", item.UseSlots },
                { "@bSocketLocked", item.SocketLocked },
                { "@vProperties", item.Properties.ToString() },
                { "@aContentIDs", item.ContentIds.ToString() },
                { "@nFormatID", item.FormatId.ToString() },
                { "@nTreasureID", item.TreasureId.ToString() },
                { "@nComponentID", item.ComponentId.ToString() },
                { "@bMirrored", item.Mirrored },
                { "@nSlotDepth", item.SlotDepth },
                { "@strChargeProfiles", item.ChargeProfiles },
                { "@aAttackModes", item.AttackModes },
                { "@nStackLimit", item.StackLimit },
                { "@aSwitchIDs", item.SwitchIds },
                { "@aSounds", item.Sounds.ToString() },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
