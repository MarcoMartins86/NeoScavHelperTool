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
    public class HexTypesHandlerService
        : NewModBaseHandlerService<HexTypesHandlerService, HexTypesItem>
    {
        public const string TABLE = "hextypes";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL, 
              `strDesc` TEXT NOT NULL,
              `nTerrainCost` INTEGER NOT NULL,
              `nVizLimiter` INTEGER NOT NULL,
              `nVizIncrease` INTEGER NOT NULL,
              `nTreasureID` INTEGER NOT NULL,
              `bPassable` INTEGER NOT NULL, 
              `nScavengeInitialID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */
              `nScavengeItemsIDPerHour` TEXT NOT NULL DEFAULT '0:25', /* mod:value */
              `nCampItems` INTEGER NOT NULL DEFAULT 5, 
              `vLightLevels` TEXT NOT NULL DEFAULT '0.57,1.0,0.57,0.15',
              `nDefaultCampID` TEX NOT NULL DEFAULT '0:517', /* mod:value */
              `nMinRange` INTEGER NOT NULL DEFAULT 3,
              `nMaxRange` INTEGER NOT NULL DEFAULT 6,
              `vCondIDs` TEXT NOT NULL,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_hextypes` (
              `id`,
              `strName`,
              `strDesc`,
              `nTerrainCost`,
              `nVizLimiter`,
              `nVizIncrease`,
              `nTreasureID`,
              `bPassable`,
              `nScavengeInitialID`,
              `nScavengeItemsIDPerHour`,
              `nCampItems`,
              `vLightLevels`,
              `nDefaultCampID`,
              `nMinRange`,
              `nMaxRange`,
              `vCondIDs`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @strDesc,
              @nTerrainCost,
              @nVizLimiter,
              @nVizIncrease,
              @nTreasureID,
              @bPassable,
              @nScavengeInitialID,
              @nScavengeItemsIDPerHour,
              @nCampItems,
              @vLightLevels,
              @nDefaultCampID,
              @nMinRange,
              @nMaxRange,
              @vCondIDs,
              @isOverriden,
              @valueModFolder
            );
            ";

        public HexTypesHandlerService(
            ILogger<HexTypesHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(ref HexTypesItem item, XmlElement column)
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
                case "nTerrainCost":
                    item.TerrainCost = int.Parse(value);
                    break;
                case "nVizLimiter":
                    item.VizLimiter = int.Parse(value);
                    break;
                case "nVizIncrease":
                    item.VizIncrease = int.Parse(value);
                    break;
                case "nTreasureID":
                    item.TreasureId = int.Parse(value);
                    break;
                case "bPassable":
                    item.Passable = int.Parse(value) == 1;
                    break;
                case "nScavengeInitialID":
                    item.ScavengeInitialId = ModRefValue<int>.Parse(value);
                    break;
                case "nScavengeItemsIDPerHour":
                    item.ScavengeItemsIdPerHour = ModRefValue<int>.Parse(value);
                    break;
                case "nCampItems":
                    item.CampItems = int.Parse(value);
                    break;
                case "vLightLevels":
                    item.LightLevels = value;
                    break;
                case "nDefaultCampID":
                    item.DefaultCampId = ModRefValue<int>.Parse(value);
                    break;
                case "nMinRange":
                    item.MinRange = int.Parse(value);
                    break;
                case "nMaxRange":
                    item.MaxRange = int.Parse(value);
                    break;
                case "vCondIDs":
                    item.ConditionsIds = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            HexTypesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@strDesc", item.Description },
                { "@nTerrainCost", item.TerrainCost },
                { "@nVizLimiter", item.VizLimiter },
                { "@nVizIncrease", item.VizIncrease },
                { "@nTreasureID", item.TreasureId },
                { "@bPassable", item.Passable },
                { "@nScavengeInitialID", item.ScavengeInitialId?.ToString() },
                { "@nScavengeItemsIDPerHour", item.ScavengeItemsIdPerHour?.ToString() },
                { "@nCampItems", item.CampItems },
                { "@vLightLevels", item.LightLevels },
                { "@nDefaultCampID", item.DefaultCampId?.ToString() },
                { "@nMinRange", item.MinRange },
                { "@nMaxRange", item.MaxRange },
                { "@vCondIDs", item.ConditionsIds },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
