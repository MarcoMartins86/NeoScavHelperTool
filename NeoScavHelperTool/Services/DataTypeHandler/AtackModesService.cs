using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services.DataTypeHandler
{
    public class AtackModesService : DataTypeBaseService
    {
        public const string TABLE = "attackmodes";
        private const string CREATE_TABLE_SQL =
            @"
            CREATE TABLE IF NOT EXISTS `{0}` (
              `id` INTEGER NOT NULL,
              `strName` TEXT NOT NULL,
              `strNotes` TEXT NOT NULL DEFAULT '',
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
              `wasOverriden` INTEGER NOT NULL,
              `valueModFolder` TEXT NOT NULL,
              PRIMARY KEY(`id`)
            ) WITHOUT ROWID;
            ";

        public override string Table => TABLE;

        ILogger<AtackModesService> _logger;
        DatabaseService _dbService;

        public AtackModesService(ILogger<AtackModesService> logger, DatabaseService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        public override void LoadModIntoDb(
            string gamePath,
            ModInfo mod,
            DataTypeAttribute attribute
        )
        {
            // Create the XmlDocument from file
            // XSD valitations will run at loading time
            string file = GetFileFullPath(gamePath, mod, attribute);
            XmlDocument doc = CreateXmlDocument(file);

            _logger.LogTrace("Opened mod \"{mod}\" \"{file}\" successfully", mod.Name, file);

            _dbService.Connection.Execute(string.Format(CREATE_TABLE_SQL, $"{mod.Name}_{TABLE}"));
        }
    }
}
