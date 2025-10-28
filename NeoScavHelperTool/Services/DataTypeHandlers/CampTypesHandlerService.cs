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
    public class CampTypesHandlerService
        : NewModBaseHandlerService<CampTypesHandlerService, CampTypesItem>
    {
        public const string TABLE = "camptypes";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL, 
              `strDesc` TEXT NOT NULL,
              `vImageList` TEXT NOT NULL DEFAULT '0:ItmScavengeGrass01.png',
              `aCapacities` TEXT NOT NULL DEFAULT '30x30',
              `nTreasureID` TEXT NOT NULL DEFAULT '0:3', 
              `m_fAlertness` REAL NOT NULL DEFAULT 0,
              `m_fVisibility` REAL NOT NULL DEFAULT -0.05,
              `WetTempAdjustMod` REAL NOT NULL DEFAULT 0,
              `m_fHealPerHourMod` REAL NOT NULL DEFAULT 0,
              `fSleepQuality` REAL NOT NULL DEFAULT 0, 
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_camptypes` (
              `id`,
              `strDesc`,
              `vImageList`,
              `aCapacities`,
              `nTreasureID`,
              `m_fAlertness`,
              `m_fVisibility`,
              `WetTempAdjustMod`,
              `m_fHealPerHourMod`,
              `fSleepQuality`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strDesc,
              @vImageList,
              @aCapacities,
              @nTreasureID,
              @m_fAlertness,
              @m_fVisibility,
              @WetTempAdjustMod,
              @m_fHealPerHourMod,
              @fSleepQuality,
              @isOverriden,
              @valueModFolder
            );
            ";

        public CampTypesHandlerService(
            ILogger<CampTypesHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(ref CampTypesItem item, XmlElement column)
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "id":
                    item.Id = int.Parse(value);
                    break;
                case "strDesc":
                    item.Description = value;
                    break;
                case "vImageList":
                    item.ImageListDb = value;
                    break;
                case "aCapacities":
                    item.Capacities = value;
                    break;
                case "nTreasureID":
                    item.TreasureIdDb = value;
                    break;
                case "m_fAlertness":
                    item.Alertness = double.Parse(value);
                    break;
                case "m_fVisibility":
                    item.Visibility = double.Parse(value);
                    break;
                case "WetTempAdjustMod":
                    item.WetTempAdjustMod = double.Parse(value);
                    break;
                case "m_fHealPerHourMod":
                    item.HealPerHourMod = double.Parse(value);
                    break;
                case "fSleepQuality":
                    item.SleepQuality = double.Parse(value);
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            CampTypesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strDesc", item.Description },
                { "@vImageList", item.ImageListDb },
                { "@aCapacities", item.Capacities },
                { "@nTreasureID", item.TreasureIdDb },
                { "@m_fAlertness", item.Alertness },
                { "@m_fVisibility", item.Visibility },
                { "@WetTempAdjustMod", item.WetTempAdjustMod },
                { "@m_fHealPerHourMod", item.HealPerHourMod },
                { "@fSleepQuality", item.SleepQuality },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
