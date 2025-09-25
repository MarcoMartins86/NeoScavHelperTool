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
    public class EncounterTriggersHandlerService
        : NewModBaseHandlerService<EncounterTriggersHandlerService, EncounterTriggersItem>
    {
        public const string TABLE = "encountertriggers";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL, 
              `nEncounterID` INTEGER NOT NULL, 
              `fChance` REAL NOT NULL,
              `bLocBased` INTEGER NOT NULL, 
              `bDateBased` INTEGER NOT NULL,
              `bHexBased` INTEGER NOT NULL,
              `bUnique` INTEGER NOT NULL,
              `bAIPassable` INTEGER NOT NULL DEFAULT 1,
              `aArea` TEXT NOT NULL,
              `dateMin` TEXT NOT NULL, 
              `dateMax` TEXT NOT NULL, 
              `aHexTypes` TEXT NOT NULL,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_encountertriggers` (
              `id`,
              `strName`,
              `nEncounterID`,
              `fChance`,
              `bLocBased`,
              `bDateBased`,
              `bHexBased`,
              `bUnique`,
              `bAIPassable`,
              `aArea`,
              `dateMin`,
              `dateMax`,
              `aHexTypes`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @nEncounterID,
              @fChance,
              @bLocBased,
              @bDateBased,
              @bHexBased,
              @bUnique,
              @bAIPassable,
              @aArea,
              @dateMin,
              @dateMax,
              @aHexTypes,
              @isOverriden,
              @valueModFolder
            );
            ";

        public EncounterTriggersHandlerService(
            ILogger<EncounterTriggersHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        protected override void AssignColumnValueToItem(
            ref EncounterTriggersItem item,
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
                case "nEncounterID":
                    item.EncounterId = int.Parse(value);
                    break;
                case "fChance":
                    item.Chance = double.Parse(value);
                    break;
                case "bLocBased":
                    item.LocBased = int.Parse(value) == 1;
                    break;
                case "bDateBased":
                    item.DateBased = int.Parse(value) == 1;
                    break;
                case "bHexBased":
                    item.HexBased = int.Parse(value) == 1;
                    break;
                case "bUnique":
                    item.Unique = int.Parse(value) == 1;
                    break;
                case "bAIPassable":
                    item.AIPassable = int.Parse(value) == 1;
                    break;
                case "aArea":
                    item.Area = value;
                    break;
                case "dateMin":
                    item.DateMin = value;
                    break;
                case "dateMax":
                    item.DateMax = value;
                    break;
                case "aHexTypes":
                    item.HexTypes = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            EncounterTriggersItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@nEncounterID", item.EncounterId },
                { "@fChance", item.Chance },
                { "@bLocBased", item.LocBased },
                { "@bDateBased", item.DateBased },
                { "@bHexBased", item.HexBased },
                { "@bUnique", item.Unique },
                { "@bAIPassable", item.AIPassable },
                { "@aArea", item.Area },
                { "@dateMin", item.DateMin },
                { "@dateMax", item.DateMax },
                { "@aHexTypes", item.HexTypes },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
