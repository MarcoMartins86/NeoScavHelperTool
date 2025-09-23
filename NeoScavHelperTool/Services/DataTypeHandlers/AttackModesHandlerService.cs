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
using SQLite;

namespace NeoScavHelperTool.Services.DataTypeHandlers
{
    public class AttackModesHandlerService
        : NewModBaseHandlerService<AttackModesHandlerService, AttackModesItem>
    {
        public const string TABLE = "attackmodes";
        public override string Table => TABLE;

        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `strNotes` TEXT NOT NULL,
              `nRange` INTEGER NOT NULL DEFAULT 1,
              `fDamageCut` REAL NOT NULL DEFAULT 0,
              `fDamageBlunt` REAL NOT NULL DEFAULT 0,
              `strChargeProfiles` TEXT NOT NULL,
              `nPenetration` INTEGER NOT NULL DEFAULT 0,
              `nType` INTEGER NOT NULL DEFAULT 0,
              `strSnd`TEXT NOT NULL,
              `bTransfer` INTEGER NOT NULL DEFAULT 0,
              `vAttackerConditions` TEXT NOT NULL,
              `strIMG` TEXT NOT NULL,
              `fMorale` REAL NOT NULL DEFAULT 0.25,
              `strWieldPhrase` TEXT NOT NULL,
              `vAttackPhrases` TEXT NOT NULL,
              `isOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";
        protected override string CreateTableSql => CREATE_TABLE_SQL;

        private const string UPSERT_ITEM_SQL =
            @"
            INSERT OR REPLACE INTO `{0}_attackmodes` (
              `id`,
              `strName`,
              `strNotes`,
              `nRange`,
              `fDamageCut`,
              `fDamageBlunt`,
              `strChargeProfiles`,
              `nPenetration`,
              `nType`,
              `strSnd`,
              `bTransfer`,
              `vAttackerConditions`,
              `strIMG`,
              `fMorale`,
              `strWieldPhrase`,
              `vAttackPhrases`,
              `isOverriden`,
              `valueModFolder`
            ) VALUES (
              @id,
              @strName,
              @strNotes,
              @nRange,
              @fDamageCut,
              @fDamageBlunt,
              @strChargeProfiles,
              @nPenetration,
              @nType,
              @strSnd,
              @bTransfer,
              @vAttackerConditions,
              @strIMG,
              @fMorale,
              @strWieldPhrase,
              @vAttackPhrases,
              @isOverriden,
              @valueModFolder
            );
            ";

        public AttackModesHandlerService(
            ILogger<AttackModesHandlerService> logger,
            DatabaseService dbService
        )
            : base(logger, dbService) { }

        public override void ReadItemIntoDb(
            XmlElement table,
            string tableName,
            ModInfo mod,
            bool willOverride
        )
        {
            if (!TABLE.Equals(tableName))
            {
                throw new Exception(
                    $"Unexpected table name \"{tableName}\" at \"{nameof(AttackModesHandlerService)}\" on file \"{table.BaseURI}\""
                );
            }
            XmlNodeList columnNodes = table.SelectNodes(XML_COLUMN_ELEMENT_NAME);
            TrasverseColumnNodes(columnNodes, mod, willOverride);
        }

        protected override void AssignColumnValueToItem(ref AttackModesItem item, XmlElement column)
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
                case "strNotes":
                    item.Notes = value;
                    break;
                case "nRange":
                    item.Range = int.Parse(value);
                    break;
                case "fDamageCut":
                    item.DamageCut = double.Parse(value);
                    break;
                case "fDamageBlunt":
                    item.DamageBlunt = double.Parse(value);
                    break;
                case "strChargeProfiles":
                    item.ChargeProfiles = value;
                    break;
                case "nPenetration":
                    item.Penetration = int.Parse(value);
                    break;
                case "nType":
                    item.Type = int.Parse(value);
                    break;
                case "strSnd":
                    item.Sound = value;
                    break;
                case "bTransfer":
                    item.Transfer = int.Parse(value) == 1;
                    break;
                case "vAttackerConditions":
                    item.AttackerConditions = value;
                    break;
                case "strIMG":
                    item.Image = value;
                    break;
                case "fMorale":
                    item.Morale = double.Parse(value);
                    break;
                case "strWieldPhrase":
                    item.WieldPhrase = value;
                    break;
                case "vAttackPhrases":
                    item.AttackPhrases = value;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected \"{XML_COLUMN_ELEMENT_NAME}\" \"{XML_COLUMN_NAME_ATTRIBUTE}\" attribute on \"{TABLE}\": \"{propertyName}\":\"{value}\" and file \"{column.BaseURI}\""
                    );
            }
        }

        protected override SQLiteCommand BuildUpsertCommand(
            ModInfo mod,
            AttackModesItem item,
            Func<string, Dictionary<string, object>, SQLiteCommand> builder
        )
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                { "@id", item.Id },
                { "@strName", item.Name },
                { "@strNotes", item.Notes },
                { "@nRange", item.Range },
                { "@fDamageCut", item.DamageCut },
                { "@fDamageBlunt", item.DamageBlunt },
                { "@strChargeProfiles", item.ChargeProfiles },
                { "@nPenetration", item.Penetration },
                { "@nType", item.Type },
                { "@strSnd", item.Sound },
                { "@bTransfer", item.Transfer },
                { "@vAttackerConditions", item.AttackerConditions },
                { "@strIMG", item.Image },
                { "@fMorale", item.Morale },
                { "@strWieldPhrase", item.WieldPhrase },
                { "@vAttackPhrases", item.AttackPhrases },
                { "@isOverriden", item.IsOverriden },
                { "@valueModFolder", item.ValueModFolder },
            };
            return builder.Invoke(string.Format(UPSERT_ITEM_SQL, mod.Name), values);
        }
    }
}
