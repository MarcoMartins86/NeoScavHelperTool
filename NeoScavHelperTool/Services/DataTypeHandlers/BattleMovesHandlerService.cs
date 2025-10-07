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
    public class BattleMovesHandlerService
        : NewModBaseHandlerService<BattleMovesHandlerService, BattleMovesItem>
    {
        public const string TABLE = "battlemoves";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL, 
              `strID` TEXT NOT NULL, 
              `strName` TEXT NOT NULL, 
              `strNotes` TEXT NOT NULL,
              `strSuccess` TEXT NOT NULL, 
              `strFail` TEXT DEFAULT NULL, 
              `strPopUp` TEXT NOT NULL,
              `vChanceType` TEXT NOT NULL DEFAULT '0,0,0',
              `vUsConditions` TEXT DEFAULT NULL,
              `vThemConditions` TEXT DEFAULT NULL,
              `vPairConditions` TEXT DEFAULT NULL,
              `vUsFailConditions` TEXT DEFAULT NULL,
              `vThemFailConditions` TEXT DEFAULT NULL, 
              `vPairFailConditions` TEXT DEFAULT NULL,
              `vUsPreConditions` TEXT DEFAULT NULL,
              `vThemPreConditions` TEXT DEFAULT NULL, 
              `nSeeThem` INTEGER DEFAULT 2, 
              `nSeeUs` INTEGER DEFAULT 2,
              `bAllOutOfRange` INTEGER DEFAULT 0,
              `bInAttackRange` INTEGER DEFAULT 0,
              `nMinCharges` INTEGER DEFAULT 0,
              `nMinRange` INTEGER DEFAULT -1,
              `nMaxRange` INTEGER DEFAULT -1, 
              `nAttackModeType` INTEGER DEFAULT -1, 
              `vHexTypes` TEXT NOT NULL,
              `fChance` REAL DEFAULT 1, 
              `fPriority` REAL DEFAULT 0,
              `fDetect` REAL DEFAULT 1,
              `fOrder` REAL DEFAULT 0.5, 
              `fFatigue` REAL DEFAULT 0,
              `bApproach` INTEGER DEFAULT 0, 
              `bOffense` INTEGER DEFAULT 0, 
              `bFallBack` INTEGER DEFAULT 0, 
              `bRetreat` INTEGER DEFAULT 0,
              `bPosition` INTEGER DEFAULT 0, 
              `bPassive` INTEGER NOT NULL DEFAULT 0,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_battlemoves` (
              `id`,
              `strID`,
              `strName`,
              `strNotes`,
              `strSuccess`,
              `strFail`,
              `strPopUp`,
              `vChanceType`,
              `vUsConditions`,
              `vThemConditions`,
              `vPairConditions`,
              `vUsFailConditions`,
              `vThemFailConditions`,
              `vPairFailConditions`,
              `vUsPreConditions`,
              `vThemPreConditions`,
              `nSeeThem`,
              `nSeeUs`,
              `bAllOutOfRange`,
              `bInAttackRange`,
              `nMinCharges`,
              `nMinRange`,
              `nMaxRange`,
              `nAttackModeType`,
              `vHexTypes`,
              `fChance`,
              `fPriority`,
              `fDetect`,
              `fOrder`,
              `fFatigue`,
              `bApproach`,
              `bOffense`,
              `bFallBack`,
              `bRetreat`,
              `bPosition`,
              `bPassive`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strID,
              @strName,
              @strNotes,
              @strSuccess,
              @strFail,
              @strPopUp,
              @vChanceType,
              @vUsConditions,
              @vThemConditions,
              @vPairConditions,
              @vUsFailConditions,
              @vThemFailConditions,
              @vPairFailConditions,
              @vUsPreConditions,
              @vThemPreConditions,
              @nSeeThem,
              @nSeeUs,
              @bAllOutOfRange,
              @bInAttackRange,
              @nMinCharges,
              @nMinRange,
              @nMaxRange,
              @nAttackModeType,
              @vHexTypes,
              @fChance,
              @fPriority,
              @fDetect,
              @fOrder,
              @fFatigue,
              @bApproach,
              @bOffense,
              @bFallBack,
              @bRetreat,
              @bPosition,
              @bPassive,
              @isOverriden,
              @valueModFolder
            );
            ";

        public BattleMovesHandlerService(
            ILogger<BattleMovesHandlerService> logger,
            DatabaseService dbService,
            ModsMetadataService modsMetadataService
        )
            : base(logger, dbService, modsMetadataService) { }

        protected override void AssignColumnValueToItem(ref BattleMovesItem item, XmlElement column)
        {
            string propertyName = column.GetAttribute(XML_COLUMN_NAME_ATTRIBUTE);
            string value = column.InnerText;
            switch (propertyName)
            {
                case "id":
                    item.Id = int.Parse(value);
                    break;
                case "strID":
                    item.strId = value;
                    break;
                case "strName":
                    item.Name = value;
                    break;
                case "strNotes":
                    item.Notes = value;
                    break;
                case "strSuccess":
                    item.Success = value;
                    break;
                case "strFail":
                    item.Fail = value;
                    break;
                case "strPopUp":
                    item.PopUp = value;
                    break;
                case "vChanceType":
                    item.ChanceType = value;
                    break;
                case "vUsConditions":
                    item.UsConditions = value;
                    break;
                case "vThemConditions":
                    item.ThemConditions = value;
                    break;
                case "vPairConditions":
                    item.PairConditions = value;
                    break;
                case "vUsFailConditions":
                    item.UsFailConditions = value;
                    break;
                case "vThemFailConditions":
                    item.ThemFailConditions = value;
                    break;
                case "vPairFailConditions":
                    item.PairFailConditions = value;
                    break;
                case "vUsPreConditions":
                    item.UsPreConditions = value;
                    break;
                case "vThemPreConditions":
                    item.ThemPreConditions = value;
                    break;
                case "nSeeThem":
                    item.SeeThem = int.Parse(value);
                    break;
                case "nSeeUs":
                    item.SeeUs = int.Parse(value);
                    break;
                case "bAllOutOfRange":
                    item.AllOutOfRange = int.Parse(value) == 1;
                    break;
                case "bInAttackRange":
                    item.InAttackRange = int.Parse(value) == 1;
                    break;
                case "nMinCharges":
                    item.MinCharges = int.Parse(value);
                    break;
                case "nMinRange":
                    item.MinRange = int.Parse(value);
                    break;
                case "nMaxRange":
                    item.MaxRange = int.Parse(value);
                    break;
                case "nAttackModeType":
                    item.AttackModeType = int.Parse(value);
                    break;
                case "vHexTypes":
                    item.HexTypes = value;
                    break;
                case "fChance":
                    item.Chance = double.Parse(value);
                    break;
                case "fPriority":
                    item.Priority = double.Parse(value);
                    break;
                case "fDetect":
                    item.Detect = double.Parse(value);
                    break;
                case "fOrder":
                    item.Order = double.Parse(value);
                    break;
                case "fFatigue":
                    item.Fatigue = double.Parse(value);
                    break;
                case "bApproach":
                    item.Approach = int.Parse(value) == 1;
                    break;
                case "bOffense":
                    item.Offense = int.Parse(value) == 1;
                    break;
                case "bFallBack":
                    item.FallBack = int.Parse(value) == 1;
                    break;
                case "bRetreat":
                    item.Retreat = int.Parse(value) == 1;
                    break;
                case "bPosition":
                    item.Position = int.Parse(value) == 1;
                    break;
                case "bPassive":
                    item.Passive = int.Parse(value) == 1;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            BattleMovesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strID", item.strId },
                { "@strName", item.Name },
                { "@strNotes", item.Notes },
                { "@strSuccess", item.Success },
                { "@strFail", item.Fail },
                { "@strPopUp", item.PopUp },
                { "@vChanceType", item.ChanceType },
                { "@vUsConditions", item.UsConditions },
                { "@vThemConditions", item.ThemConditions },
                { "@vPairConditions", item.PairConditions },
                { "@vUsFailConditions", item.UsFailConditions },
                { "@vThemFailConditions", item.ThemFailConditions },
                { "@vPairFailConditions", item.PairFailConditions },
                { "@vUsPreConditions", item.UsPreConditions },
                { "@vThemPreConditions", item.ThemPreConditions },
                { "@nSeeThem", item.SeeThem },
                { "@nSeeUs", item.SeeUs },
                { "@bAllOutOfRange", item.AllOutOfRange },
                { "@bInAttackRange", item.InAttackRange },
                { "@nMinCharges", item.MinCharges },
                { "@nMinRange", item.MinRange },
                { "@nMaxRange", item.MaxRange },
                { "@nAttackModeType", item.AttackModeType },
                { "@vHexTypes", item.HexTypes },
                { "@fChance", item.Chance },
                { "@fPriority", item.Priority },
                { "@fDetect", item.Detect },
                { "@fOrder", item.Order },
                { "@fFatigue", item.Fatigue },
                { "@bApproach", item.Approach },
                { "@bOffense", item.Offense },
                { "@bFallBack", item.FallBack },
                { "@bRetreat", item.Retreat },
                { "@bPosition", item.Position },
                { "@bPassive", item.Passive },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
