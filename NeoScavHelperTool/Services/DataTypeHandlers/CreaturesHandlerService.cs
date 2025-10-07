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
    public class CreaturesHandlerService
        : NewModBaseHandlerService<CreaturesHandlerService, CreaturesItem>
    {
        public const string TABLE = "creatures";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `strNamePublic` TEXT NOT NULL, 
              `strNotes` TEXT NOT NULL, 
              `strImg` TEXT NOT NULL,
              `vEncounterIDs` TEXT NOT NULL,
              `nMovesPerTurn` INTEGER NOT NULL, 
              `nTreasureID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */ 
              `nFaction` TEXT NOT NULL DEFAULT '0:0', /* mod:value */
              `vAttackModes` TEXT NOT NULL, 
              `vBaseConditions` TEXT NOT NULL,
              `nCorpseID` TEXT NOT NULL DEFAULT '0:3', /* mod:value */
              `vActivities` TEXT NOT NULL, 
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_creatures` (
              `id`,
              `strName`,
              `strNamePublic`,
              `strNotes`,
              `strImg`,
              `vEncounterIDs`,
              `nMovesPerTurn`,
              `nTreasureID`,
              `nFaction`,
              `vAttackModes`,
              `vBaseConditions`,
              `nCorpseID`,
              `vActivities`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @strNamePublic,
              @strNotes,
              @strImg,
              @vEncounterIDs,
              @nMovesPerTurn,
              @nTreasureID,
              @nFaction,
              @vAttackModes,
              @vBaseConditions,
              @nCorpseID,
              @vActivities,
              @isOverriden,
              @valueModFolder
            );
            ";

        public CreaturesHandlerService(
            ILogger<CreaturesHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(ref CreaturesItem item, XmlElement column)
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
                case "strNamePublic":
                    item.NamePublic = value;
                    break;
                case "strNotes":
                    item.Notes = value;
                    break;
                case "strImg":
                    item.Image = value;
                    break;
                case "vEncounterIDs":
                    item.EncounterIds = value;
                    break;
                case "nMovesPerTurn":
                    item.MovesPerTurn = int.Parse(value);
                    break;
                case "nTreasureID":
                    item.TreasureId = ModRefValue<int>.Parse(value);
                    break;
                case "nFaction":
                    item.Faction = ModRefValue<int>.Parse(value);
                    break;
                case "vAttackModes":
                    item.AttackModes = value;
                    break;
                case "vBaseConditions":
                    item.BaseConditions = value;
                    break;
                case "nCorpseID":
                    item.CorpseId = ModRefValue<int>.Parse(value);
                    break;
                case "vActivities":
                    item.Activities = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            CreaturesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@strNamePublic", item.NamePublic },
                { "@strNotes", item.Notes },
                { "@strImg", item.Image },
                { "@vEncounterIDs", item.EncounterIds },
                { "@nMovesPerTurn", item.MovesPerTurn },
                { "@nTreasureID", item.TreasureId?.ToString() },
                { "@nFaction", item.Faction?.ToString() },
                { "@vAttackModes", item.AttackModes },
                { "@vBaseConditions", item.BaseConditions },
                { "@nCorpseID", item.CorpseId?.ToString() },
                { "@vActivities", item.Activities },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
