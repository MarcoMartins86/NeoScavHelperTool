using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace NeoScavModHelperTool
{
    public class DBOperations
    {
        private static string _sqlCommandSelectAllTableData = "SELECT * FROM `{0}`";
        private static string _sqlCommandSelectAllFromTableWhereColumnEqualValue = "SELECT * FROM `{0}` WHERE `{1}`=@value";
        private static string _sqlCommandSelectColumnFromTableWhereColumnEqualValue = "SELECT `{0}` FROM `{1}` WHERE `{2}`=@value";
        private static string _sqlCommandSelectColumnFromTableWhereMultipleColumnEqualValue = "SELECT `{0}` FROM `{1}` WHERE @multiple";
        private static string _sqlCommandSelectColumnsFromTableWhereMultipleColumnEqualValue = "SELECT {0} FROM `{1}` WHERE @multiple";
        private static string _sqlCommandSelectColumnsFromTableWhereColumnEqualValue = "SELECT {0} FROM `{1}` WHERE `{2}`=@value";
        private static string _sqlCommandSelectTablesWithNameLike = "SELECT `name` FROM `sqlite_master` WHERE `type`='table' and `name` like @name;";
        private static string _sqlCommandSelectValueFromConfiguration = "SELECT `value` FROM `configuration` WHERE `name`=@name";
        private static string _sqlCommandInsertOrReplaceIntoConfiguration = "INSERT OR REPLACE INTO `configuration` (`name`, `value`) VALUES(@name,@value);";
        private static string _sqlCommandInsertOrReplaceIntoAnyTable = "INSERT OR REPLACE INTO `{0}` ({1}) VALUES(@values);";
        private static string _sqlCommandInsertOrReplaceIntoFileHash = "INSERT OR REPLACE INTO `file_hash` (`file`, `hash`) values (@file, @hash);";
        private static string _sqlCommandPragmaTableInfo = "PRAGMA table_info(`{0}`)";

        private SQLiteConnection _dbFsConnection;
        public SQLiteConnection DbFsConnection => _dbFsConnection;
        private SQLiteConnection _dbMemConnection;
        public SQLiteConnection DbMemConnection => _dbMemConnection;

        private static Dictionary<string, string> _sqlCommandsCreationTables;
        private static List<string> _listTableNameSufix;
        private static List<int[]> _listTableWantedValues;

        private static List<string> _listAttackmodesColumnNames;
        public static List<string> ListAttackmodesColumnNames => _listAttackmodesColumnNames;

        private static List<string> _listBattlemovesColumnNames;
        public static List<string> ListBattlemovesColumnNames => _listBattlemovesColumnNames;

        private static List<string> _listChargeprofilesColumnNames;
        public static List<string> ListChargeprofilesColumnNames => _listChargeprofilesColumnNames;

        private static List<string> _listConditionsColumnNames;
        public static List<string> ListConditionsColumnNames => _listConditionsColumnNames;

        private static List<string> _listContainertypesColumnNames;
        public static List<string> ListContainertypesColumnNames => _listContainertypesColumnNames;

        private static List<string> _listCreaturesColumnNames;
        public static List<string> ListCreaturesColumnNames => _listCreaturesColumnNames;

        private static List<string> _listCreaturesourcesColumnNames;
        public static List<string> ListCreaturesourcesColumnNames => _listCreaturesourcesColumnNames;

        public void Init()
        {
            App._splashScreen.UpdateMessage("Initializing database");
            //The sufix of MODs tables common names
            _listTableNameSufix = new List<string>{
                "attackmodes",
                "barterhexes",
                "battlemoves",
                "camptypes",
                "chargeprofiles",
                "conditions",
                "containertypes",
                "creatures",
                "creaturesources",
                "datafiles",
                "dmcplaces",
                "encounters",
                "encountertriggers",
                "factions",
                "forbiddenhexes",
                "gamevars",
                "headlines",
                "hextypes",
                "images",
                "ingredients",
                "itemprops",
                "itemtypes",
                "maps",
                "recipes",
                "treasuretable"
            };

            _listTableWantedValues = new List<int[]>
            {
                new int[] {0, 1}, //attackmodes
                new int[] {0, -1}, //barterhexes
                new int[] {0, 2}, //battlemoves
                new int[] {0, 1}, //camptypes
                new int[] {0, 1}, //chargeprofiles
                new int[] {0, 1}, //conditions
                new int[] {0, 1}, //containertypes
                new int[] {0, 1}, //creatures
                new int[] {0, 1}, //creaturesources
                new int[] {0, 2}, //datafiles
                new int[] {0, 1}, //dmcplaces
                new int[] {0, 1}, //encounters
                new int[] {0, 1}, //encountertriggers
                new int[] {0, 1}, //factions
                new int[] {0, 3}, //forbiddenhexes
                new int[] {-1, 1}, //gamevars
                new int[] {0, 1}, //headlines
                new int[] {0, 1}, //hextypes
                new int[] {-1, 1}, //images
                new int[] {0, 1}, //ingredients
                new int[] {0, 1}, //itemprops
                new int[] {0, 3}, //itemtypes
                new int[] {0, 1}, //maps
                new int[] {0, 1}, //recipes
                new int[] {0, 1} //treasuretable
            };

            _listAttackmodesColumnNames = new List<string>
            {
                "id",
                "strName",
                "strNotes",
                "nRange",
                "fDamageCut",
                "fDamageBlunt",
                "strChargeProfiles",
                "nPenetration",
                "nType",
                "strSnd",
                "bTransfer",
                "vAttackerConditions",
                "strIMG",
                "fMorale",
                "strWieldPhrase",
                "vAttackPhrases"
            };

            _listBattlemovesColumnNames = new List<string>
            {
                "id",
                "strID",
                "strName",
                "strNotes",
                "strSuccess",
                "strFail",
                "strPopUp",
                "vChanceType",
                "vUsConditions",
                "vThemConditions",
                "vPairConditions",
                "vUsFailConditions",
                "vThemFailConditions",
                "vPairFailConditions",
                "vUsPreConditions",
                "vThemPreConditions",
                "nSeeThem",
                "nSeeUs",
                "bAllOutOfRange",
                "bInAttackRange",
                "nMinCharges",
                "nMinRange",
                "nMaxRange",
                "nAttackModeType",
                "vHexTypes",
                "fChance",
                "fPriority",
                "fDetect",
                "fOrder",
                "fFatigue",
                "bApproach",
                "bOffense",
                "bFallBack",
                "bRetreat",
                "bPosition",
                "bPassive"
            };

            _listChargeprofilesColumnNames = new List<string>
            {
                "nID",
                "strName",
                "strItemID",
                "fPerUse",
                "fPerHour",
                "fPerHourEquipped",
                "fPerHex",
                "bDegrade"
            };

            _listConditionsColumnNames = new List<string>
            {
                "id",
                "strName",
                "strDesc",
                "aFieldNames",
                "aModifiers",
                "aEffects",
                "bFatal",
                "vIDNext",
                "fDuration",
                "bPermanent",
                "vChanceNext",
                "bStackable",
                "bDisplay",
                "bDisplayOther",
                "bDisplayGameOver",
                "nColor",
                "bResetTimer",
                "bRemoveAll",
                "bRemovePostCombat",
                "nTransferRange",
                "aThresholds"
            };

            _listContainertypesColumnNames = new List<string>
            {
                "id",
                "strName"
            };

            _listCreaturesColumnNames = new List<string>
            {
                "id",
                "strName",
                "strNamePublic",
                "strNotes",
                "strImg",
                "vEncounterIDs",
                "nMovesPerTurn",
                "nTreasureID",
                "nFaction",
                "vAttackModes",
                "vBaseConditions",
                "nCorpseID",
                "vActivities"
            };

            _listCreaturesourcesColumnNames = new List<string>
            {
                "id",
                "strName", 
                "nX",
                "nY",
                "nCreatureID",
                "nMin",
                "nMax",
                "fWeight" 
            };

            //All tables creation sql commands will be in this dictionary with the exception of special tables
            _sqlCommandsCreationTables = new Dictionary<string, string>();

            _sqlCommandsCreationTables.Add("attackmodes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strNotes` TEXT NOT NULL DEFAULT ''," +
                "`nRange` INTEGER NOT NULL DEFAULT 1," +
                "`fDamageCut` REAL NOT NULL DEFAULT 0," +
                "`fDamageBlunt` REAL NOT NULL DEFAULT 0," +
                "`strChargeProfiles` TEXT NOT NULL," +
                "`nPenetration` INTEGER NOT NULL DEFAULT 0," +
                "`nType` INTEGER NOT NULL DEFAULT 0," +
                "`strSnd`TEXT NOT NULL," +
                "`bTransfer` INTEGER NOT NULL DEFAULT 0," +
                "`vAttackerConditions`TEXT NOT NULL," +
                "`strIMG` TEXT NOT NULL," +
                "`fMorale` REAL NOT NULL DEFAULT 0.25," +
                "`strWieldPhrase` TEXT NOT NULL," +
                "`vAttackPhrases` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("barterhexes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`nX` INTEGER NOT NULL DEFAULT 0," +
                "`nY` INTEGER NOT NULL DEFAULT 0," +
                "`bBuys` INTEGER NOT NULL DEFAULT 0," +
                "`nRestockTreasureID` INTEGER NOT NULL DEFAULT 3," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("battlemoves",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strID` TEXT NOT NULL," +
                "`strName`	TEXT NOT NULL," +
                "`strNotes` TEXT NOT NULL," +
                "`strSuccess` TEXT NOT NULL," +
                "`strFail` TEXT," +
                "`strPopUp` TEXT DEFAULT NULL," +
                "`vChanceType` TEXT NOT NULL DEFAULT '0,0,0'," +
                "`vUsConditions` TEXT DEFAULT NULL," +
                "`vThemConditions` TEXT DEFAULT NULL," +
                "`vPairConditions` TEXT DEFAULT NULL," +
                "`vUsFailConditions` TEXT DEFAULT NULL," +
                "`vThemFailConditions` TEXT DEFAULT NULL," +
                "`vPairFailConditions` TEXT DEFAULT NULL," +
                "`vUsPreConditions` TEXT DEFAULT NULL," +
                "`vThemPreConditions` TEXT DEFAULT NULL," +
                "`nSeeThem` INTEGER DEFAULT 2," +
                "`nSeeUs` INTEGER DEFAULT 2," +
                "`bAllOutOfRange` INTEGER DEFAULT 0," +
                "`bInAttackRange` INTEGER DEFAULT 0," +
                "`nMinCharges` INTEGER DEFAULT 0," +
                "`nMinRange` INTEGER DEFAULT -1," +
                "`nMaxRange` INTEGER DEFAULT -1," +
                "`nAttackModeType` INTEGER DEFAULT -1," +
                "`vHexTypes` TEXT NOT NULL," +
                "`fChance` REAL DEFAULT 1," +
                "`fPriority` REAL DEFAULT 0," +
                "`fDetect` REAL DEFAULT 1," +
                "`fOrder` REAL DEFAULT 0.5," +
                "`fFatigue` REAL DEFAULT 0," +
                "`bApproach` INTEGER DEFAULT 0," +
                "`bOffense` INTEGER DEFAULT 0," +
                "`bFallBack` INTEGER DEFAULT 0," +
                "`bRetreat` INTEGER DEFAULT 0," +
                "`bPosition` INTEGER DEFAULT 0," +
                "`bPassive` INTEGER NOT NULL DEFAULT 0," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("camptypes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strDesc` TEXT NOT NULL," +
                "`vImageList` TEXT NOT NULL DEFAULT 'ItmScavengeGrass01.png'," +
                "`aCapacities` TEXT NOT NULL DEFAULT '30x30'," +
                "`nTreasureID` INTEGER NOT NULL DEFAULT 3," +
                "`m_fAlertness` REAL NOT NULL DEFAULT 0," +
                "`m_fVisibility` REAL NOT NULL DEFAULT -0.05," +
                "`WetTempAdjustMod` REAL NOT NULL DEFAULT 0," +
                "`m_fHealPerHourMod` REAL NOT NULL DEFAULT 0," +
                "`fSleepQuality` REAL NOT NULL DEFAULT 0," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("chargeprofiles",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`nID` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strItemID` TEXT NOT NULL," +
                "`fPerUse` REAL NOT NULL DEFAULT 0," +
                "`fPerHour` REAL NOT NULL DEFAULT 0," +
                "`fPerHourEquipped` REAL NOT NULL DEFAULT 0," +
                "`fPerHex` REAL DEFAULT 0," +
                "`bDegrade` INTEGER NOT NULL DEFAULT 0," +
                "PRIMARY KEY(`nID`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("conditions",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id`INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strDesc` TEXT NOT NULL," +
                "`aFieldNames` TEXT NOT NULL," +
                "`aModifiers` TEXT NOT NULL," +
                "`aEffects` TEXT NOT NULL," +
                "`bFatal` INTEGER NOT NULL DEFAULT 0," +
                "`vIDNext` TEXT NOT NULL DEFAULT '0'," +
                "`fDuration` REAL NOT NULL DEFAULT 0," +
                "`bPermanent` INTEGER NOT NULL DEFAULT 0," +
                "`vChanceNext` TEXT NOT NULL DEFAULT '0'," +
                "`bStackable` INTEGER NOT NULL DEFAULT 0," +
                "`bDisplay` INTEGER NOT NULL DEFAULT 1," +
                "`bDisplayOther` INTEGER NOT NULL DEFAULT 0," +
                "`bDisplayGameOver` INTEGER NOT NULL DEFAULT 1," +
                "`nColor` INTEGER NOT NULL DEFAULT 0," +
                "`bResetTimer` INTEGER NOT NULL DEFAULT 1," +
                "`bRemoveAll` INTEGER NOT NULL DEFAULT 0," +
                "`bRemovePostCombat` INTEGER NOT NULL DEFAULT 0," +
                "`nTransferRange` INTEGER NOT NULL DEFAULT -1," +
                "`aThresholds` TEXT NOT NULL DEFAULT ''," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("containertypes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id`INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("creatures",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strNamePublic` TEXT NOT NULL," +
                "`strNotes` TEXT NOT NULL," +
                "`strImg` TEXT NOT NULL," +
                "`vEncounterIDs` TEXT NOT NULL," +
                "`nMovesPerTurn` INTEGER NOT NULL," +
                "`nTreasureID` INTEGER NOT NULL DEFAULT 3," +
                "`nFaction` INTEGER NOT NULL DEFAULT 0," +
                "`vAttackModes` TEXT NOT NULL," +
                "`vBaseConditions` TEXT NOT NULL," +
                "`nCorpseID` INTEGER NOT NULL DEFAULT 3," +
                "`vActivities` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("creaturesources",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`nX` INTEGER NOT NULL DEFAULT -1," +
                "`nY` INTEGER NOT NULL DEFAULT -1," +
                "`nCreatureID` INTEGER NOT NULL DEFAULT 0," +
                "`nMin` INTEGER NOT NULL DEFAULT 0," +
                "`nMax` INTEGER NOT NULL DEFAULT 0," +
                "`fWeight` REAL NOT NULL DEFAULT 1," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("datafiles",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strDesc` TEXT NOT NULL," +
                "`fValue` REAL NOT NULL DEFAULT 0," +
                "`strImg` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("dmcplaces",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strImg` TEXT NOT NULL," +
                "`nEncounterID` INTEGER NOT NULL DEFAULT 1," +
                "`nX` INTEGER NOT NULL DEFAULT 0," +
                "`nY` INTEGER NOT NULL DEFAULT 0," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("encounters",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strDesc` TEXT NOT NULL," +
                "`strImg` TEXT NOT NULL DEFAULT 'EncBlank.png'," +
                "`nTreasureID` INTEGER NOT NULL DEFAULT 3," +
                "`nRemoveTreasureID` INTEGER NOT NULL DEFAULT 3," +
                "`aConditions` TEXT NOT NULL DEFAULT '1'," +
                "`aPreConditions` TEXT NOT NULL DEFAULT ''," +
                "`fPrice` REAL NOT NULL DEFAULT 0," +
                "`aResponses` TEXT NOT NULL," +
                "`aMinimapHexes` TEXT NOT NULL DEFAULT ''," +
                "`bRemoveCreatures` INTEGER NOT NULL DEFAULT 0," +
                "`bRemoveUsed` INTEGER NOT NULL DEFAULT 0," +
                "`nItemsID` INTEGER NOT NULL DEFAULT 3," +
                "`nCreatureID` INTEGER NOT NULL DEFAULT 0," +
                "`ptCreatureHex` TEXT NOT NULL DEFAULT '0,0'," +
                "`ptTeleport` TEXT NOT NULL DEFAULT '0,0'," +
                "`ptEditor` TEXT NOT NULL DEFAULT '0,0'," +
                "`nType` INTEGER NOT NULL DEFAULT 0," +
                "`fLootChance` REAL NOT NULL DEFAULT 0," +
                "`fAccidentChance` REAL NOT NULL DEFAULT 0," +
                "`fCreatureChance` REAL NOT NULL DEFAULT 0," +
                "`vAccidents` TEXT NOT NULL DEFAULT '1'," +
                "`vLoot` TEXT NOT NULL DEFAULT '3'," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("encountertriggers",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`nEncounterID` INTEGER NOT NULL," +
                "`fChance` REAL NOT NULL," +
                "`bLocBased` INTEGER NOT NULL," +
                "`bDateBased` INTEGER NOT NULL," +
                "`bHexBased` INTEGER NOT NULL," +
                "`bUnique` INTEGER NOT NULL," +
                "`bAIPassable` INTEGER NOT NULL DEFAULT 1," +
                "`aArea` TEXT NOT NULL," +
                "`dateMin` TEXT NOT NULL," +
                "`dateMax` TEXT NOT NULL," +
                "`aHexTypes` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("factions",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`dictFactions` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("forbiddenhexes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`nX` INTEGER NOT NULL DEFAULT 0," +
                "`nY` INTEGER NOT NULL DEFAULT 0," +
                "`strName` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("gamevars",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`strName` TEXT NOT NULL," +
                "`strType` TEXT NOT NULL," +
                "`strValue` TEXT NOT NULL," +
                "PRIMARY KEY(`strName`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("headlines",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strHeadline` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("hextypes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strDesc` TEXT NOT NULL," +
                "`nTerrainCost` INTEGER NOT NULL," +
                "`nVizLimiter` INTEGER NOT NULL," +
                "`nVizIncrease` INTEGER NOT NULL," +
                "`nTreasureID` INTEGER NOT NULL," +
                "`bPassable` INTEGER NOT NULL," +
                "`nScavengeInitialID` INTEGER NOT NULL DEFAULT 3," +
                "`nScavengeItemsIDPerHour` INTEGER NOT NULL DEFAULT 25," +
                "`nCampItems` INTEGER NOT NULL DEFAULT 5," +
                "`vLightLevels` TEXT NOT NULL DEFAULT '0.57,1.0,0.57,0.15'," +
                "`nDefaultCampID` INTEGER NOT NULL DEFAULT 517," +
                "`nMinRange` INTEGER NOT NULL DEFAULT 3," +
                "`nMaxRange` INTEGER NOT NULL DEFAULT 6," +
                "`vCondIDs` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("images",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`name` TEXT NOT NULL," +
                "`small` TEXT NOT NULL DEFAULT ''," +
                "`big` TEXT NOT NULL DEFAULT ''," +
                "PRIMARY KEY(`name`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("ingredients",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`nID` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strRequiredProps` TEXT NOT NULL," +
                "`strForbidProps` TEXT NOT NULL," +
                "PRIMARY KEY(`nID`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("itemprops",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`nID` INTEGER NOT NULL," +
                "`strPropertyName` TEXT NOT NULL," +
                "PRIMARY KEY(`nID`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("itemtypes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`nGroupID` INTEGER NOT NULL," +
                "`nSubgroupID` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strDesc` TEXT NOT NULL," +
                "`strDescAlt` TEXT NOT NULL," +
                "`nCondID` INTEGER NOT NULL DEFAULT 1," +
                "`vImageList` TEXT NOT NULL," +
                "`vSpriteList` TEXT NOT NULL," +
                "`vImageUsage` TEXT NOT NULL," +
                "`fWeight` REAL NOT NULL DEFAULT 0," +
                "`fMonetaryValue` REAL NOT NULL DEFAULT 0," +
                "`fMonetaryValueAlt` REAL NOT NULL DEFAULT 0," +
                "`fDurability` REAL NOT NULL DEFAULT 1," +
                "`fDegradePerHour` REAL NOT NULL DEFAULT 0," +
                "`fEquipDegradePerHour` REAL NOT NULL DEFAULT 0," +
                "`fDegradePerUse` REAL NOT NULL DEFAULT 0," +
                "`vDegradeTreasureIDs` TEXT NOT NULL DEFAULT '3,3'," +
                "`aEquipConditions` TEXT NOT NULL," +
                "`aPossessConditions` TEXT NOT NULL," +
                "`aUseConditions` TEXT NOT NULL," +
                "`aCapacities` TEXT NOT NULL," +
                "`vEquipSlots` TEXT NOT NULL," +
                "`vUseSlots` TEXT NOT NULL," +
                "`bSocketLocked` INTEGER NOT NULL DEFAULT 0," +
                "`vProperties` TEXT NOT NULL," +
                "`aContentIDs` TEXT NOT NULL," +
                "`nFormatID` TEXT NOT NULL DEFAULT '3'," +
                "`nTreasureID` TEXT NOT NULL DEFAULT '3'," +
                "`nComponentID` INTEGER NOT NULL DEFAULT 3," +
                "`bMirrored` INTEGER NOT NULL DEFAULT 0," +
                "`nSlotDepth` INTEGER NOT NULL DEFAULT 0," +
                "`strChargeProfiles` TEXT NOT NULL," +
                "`aAttackModes` TEXT NOT NULL," +
                "`nStackLimit` INTEGER NOT NULL DEFAULT 1," +
                "`aSwitchIDs` TEXT NOT NULL DEFAULT ''," +
                "`aSounds` TEXT NOT NULL DEFAULT 'cuePickup,cuePutdown'," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("maps",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strDef` TEXT NOT NULL," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("recipes",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`nID` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`strSecretName` TEXT NOT NULL DEFAULT ''," +
                "`strTools` TEXT NOT NULL DEFAULT ''," +
                "`strConsumed` TEXT NOT NULL DEFAULT ''," +
                "`strDestroyed` TEXT NOT NULL DEFAULT ''," +
                "`nTreasureID` TEXT NOT NULL DEFAULT '3'," +
                "`fHours` REAL NOT NULL DEFAULT 0," +
                "`nReverse` INTEGER NOT NULL DEFAULT 0," +
                "`nHiddenID` TEXT NOT NULL DEFAULT '0'," +
                "`bIdentify` INTEGER NOT NULL DEFAULT 0," +
                "`bTransferComponents` INTEGER NOT NULL DEFAULT 0," +
                "`vAlsoTry` TEXT NOT NULL," +
                "`nTempTreasureID` INTEGER NOT NULL DEFAULT 3," +
                "`bDegradeOutput` INTEGER NOT NULL DEFAULT 1," +
                "`strType` TEXT NOT NULL DEFAULT ''," +
                "`bScrap` INTEGER NOT NULL DEFAULT 1," +
                "PRIMARY KEY(`nID`)) WITHOUT ROWID;");
            _sqlCommandsCreationTables.Add("treasuretable",
                "CREATE TABLE IF NOT EXISTS `{0}` (" +
                "`id` INTEGER NOT NULL," +
                "`strName` TEXT NOT NULL," +
                "`aTreasures` TEXT NOT NULL," +
                "`bNested` INTEGER NOT NULL DEFAULT 0," +
                "`bSuppress` INTEGER NOT NULL DEFAULT 0," +
                "`bIdentify` INTEGER NOT NULL DEFAULT 0," +
                "PRIMARY KEY(`id`)) WITHOUT ROWID;");

            //1st - Create the needed folder and db file on appdata if it doesn't exist already
            var dbFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NeoScavHelperTool", "db.sqlite");

            if (File.Exists(dbFilename) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbFilename));
                SQLiteConnection.CreateFile(dbFilename);
            }

            _dbFsConnection = new SQLiteConnection(connectionString: "Data Source=" + dbFilename + ";Version=3;Compress=True;UTF8Encoding=True;");
            DbFsConnection.Open();

            //2nd - try to create file hash table if not exists
            SQLiteCommand command = new SQLiteCommand(DbFsConnection);
            command.CommandText = "CREATE TABLE IF NOT EXISTS `file_hash` (`file` TEXT NOT NULL, `hash` BLOB NOT NULL, PRIMARY KEY(`file`)) WITHOUT ROWID;";
            command.ExecuteNonQuery();
            //3rd - try to create configuration table if not exists
            command.Reset();
            command.CommandText = "CREATE TABLE IF NOT EXISTS `configuration` (`name` TEXT NOT NULL, `value` TEXT NOT NULL, PRIMARY KEY(`name`)) WITHOUT ROWID;";
            command.ExecuteNonQuery();
            //4th - check if we have the game directory in the configuration table and if try with known dirs if not ask it to the user
            App._splashScreen.UpdateMessage("Checking for game folder");
            command.Reset();
            command.CommandText = _sqlCommandSelectValueFromConfiguration;
            command.Parameters.Add(new SQLiteParameter("@name", "GameFolder"));
            SQLiteDataReader reader = command.ExecuteReader();
            string strGameFolder = string.Empty;
            if (reader.HasRows && reader.Read())
            {
                strGameFolder = reader.GetString(0);
                reader.Close();
                //Let's just do a quick check to see if it is still valid
                if (File.Exists(Path.Combine(strGameFolder, "NEOScavenger.exe")) == false)
                    strGameFolder = string.Empty; //clear the string since game folder changed
            }
            //check if we need to find the game folder
            if (string.IsNullOrEmpty(strGameFolder))
            {
                //try on steam folders, first on Program Files x86
                strGameFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Steam", "steamapps", "common", "NEO Scavenger");
                if (File.Exists(Path.Combine(strGameFolder, "NEOScavenger.exe")) == false) //if not found try on Program Files
                {
                    strGameFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Steam", "steamapps", "common", "NEO Scavenger");
                    if (File.Exists(Path.Combine(strGameFolder, "NEOScavenger.exe")) == false) //if not ask it to the user
                    {
                        strGameFolder = App._splashScreen.AskNeoScavengerGameFolder();
                        if (string.IsNullOrEmpty(strGameFolder))
                        {
                            App.I.DisplayMessageDialog("Choosen directory is not the game folder. Application will now exit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, true);
                            return;
                        }
                    }
                }
                //either way if it reaches here we can save the value to the database
                command.Reset();
                command.CommandText = _sqlCommandInsertOrReplaceIntoConfiguration;
                command.Parameters.Add(new SQLiteParameter("@name", "GameFolder"));
                command.Parameters.Add(new SQLiteParameter("@value", strGameFolder));
                command.ExecuteNonQuery();
                command.Reset();
            }
            //5th - start parsing vanilla game xml, checking for changes first comparing previous parse hash
            App._splashScreen.UpdateMessage("Parsing vanilla game files into database");
            ParseType0(strGameFolder, "0", "vanilla");
            //6th - start parsing mods xml, checking for changes first comparing previous parse hash
            ParseGetModsFile(strGameFolder);
            //7th - Finally create an in-memory DB with the values as the game see them
            CreateInMemGameDatabase();
        }

        public void Close()
        {
            if (DbFsConnection != null)
                DbFsConnection.Close();
            if (DbMemConnection != null)
                DbMemConnection.Close();
        }

        private void ParseElementIntoDB(string str_file, XmlReader reader, string str_table_name_prefix, int n_type_index, SQLiteTransaction sqlTransaction)
        {
            SQLiteCommand command = new SQLiteCommand(_sqlCommandInsertOrReplaceIntoAnyTable, DbFsConnection, sqlTransaction);
            bool bElementStarted = false;
            bool bEndedTableElement = false;
            string strParameterName = string.Empty;
            List<string> listParameterNames = new List<string>();
            List<string> listParameterValues = new List<string>();
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) &&
                    ((string.Compare(reader.Name.ToLower(), "column") == 0)))
                {
                    if (!bElementStarted) //continue only if no element was started
                    {
                        strParameterName = reader.GetAttribute(0); //get the parameter name
                        bElementStarted = true;
                    }
                    else
                        throw new Exception("Error parsing " + str_file);
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    if (bElementStarted)
                    {
                        listParameterNames.Add(strParameterName);
                        string value = reader.Value;
                        listParameterValues.Add(value);
                        strParameterName = string.Empty;
                    }
                    else
                        throw new Exception("Error parsing " + str_file);

                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    string strElementName = reader.Name.ToLower();
                    if (string.Compare(strElementName, "column") == 0)
                    {
                        //if strParameterName is not empty it means that there were no value
                        if (string.IsNullOrEmpty(strParameterName) == false)
                        {
                            listParameterNames.Add(strParameterName);
                            listParameterValues.Add("");
                        }
                        bElementStarted = false;
                    }
                    else if (string.Compare(strElementName, "table") == 0)
                    {
                        bEndedTableElement = true;
                        break; //all needed parameters to execute the DB query were read at this point
                    }
                }
            }
            if (bEndedTableElement)
            {
                command.CommandText = string.Format(command.CommandText,
                    str_table_name_prefix + "_" + _listTableNameSufix[n_type_index],//parameter {0} table name
                    $"`{string.Join("`,`", listParameterNames)}`" //parameter {1} columns
                    );
                command.AddArrayParameters("values", listParameterValues);
                command.ExecuteNonQuery();
            }
            else
                throw new Exception("Error parsing " + str_file);
        }

        private void ParseXMLFile(string str_file, string str_table_name_prefix, SQLiteTransaction sqlTransaction)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreProcessingInstructions = true;
            bool bErrorWhileParsing = true;
            //since many mods have wrong comments we need to remove them
            string strXMLFile = File.ReadAllText(str_file);
            strXMLFile = Regex.Replace(strXMLFile, "<!--[\\s\\S]*?(?=-->)--\\>", string.Empty);
            using (TextReader sr = new StringReader(strXMLFile))
            {
                using (XmlReader reader = XmlReader.Create(sr, settings))
                {
                    reader.MoveToContent();
                    //read until reach the <database name="neogame"> entry so then we can start parsing for real
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) &&
                            (string.Compare(reader.Name.ToLower(), "database") == 0) &&
                            (string.Compare(reader.GetAttribute(0).ToLower(), "neogame") == 0))
                        {
                            bErrorWhileParsing = false;
                            break;
                        }
                    }
                    //Check if the expected element was found
                    if (bErrorWhileParsing)
                        throw new Exception("Error parsing " + str_file);
                    //Now we will continue parsing and adding values to the DB
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) &&
                            ((string.Compare(reader.Name.ToLower(), "table") == 0)))
                        {
                            int index = _listTableNameSufix.IndexOf(reader.GetAttribute(0).ToLower());
                            if (index != -1)
                                ParseElementIntoDB(str_file, reader, str_table_name_prefix, index, sqlTransaction);
                        }
                    }
                    /*switch (reader.NodeType)
                       {
                           case XmlNodeType.Element:
                               Console.Write("<{0}>", reader.Name);
                               break;
                           case XmlNodeType.Text:
                               Console.Write(reader.Value);
                               break;
                           case XmlNodeType.CDATA:
                               Console.Write("<![CDATA[{0}]]>", reader.Value);
                               break;
                           case XmlNodeType.ProcessingInstruction:
                               Console.Write("<?{0} {1}?>", reader.Name, reader.Value);
                               break;
                           case XmlNodeType.Comment:
                               Console.Write("<!--{0}-->", reader.Value);
                               break;
                           case XmlNodeType.XmlDeclaration:
                               Console.Write("<?xml version='1.0'?>");
                               break;
                           case XmlNodeType.Document:
                               break;
                           case XmlNodeType.DocumentType:
                               Console.Write("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
                               break;
                           case XmlNodeType.EntityReference:
                               Console.Write(reader.Name);
                               break;
                           case XmlNodeType.EndElement:
                               Console.Write("</{0}>", reader.Name);
                               break;
                       }*/
                }
            }
        }

        private bool CheckSavedFileHash(string file, out byte[] file_hash)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    file_hash = md5.ComputeHash(stream);
                }
            }

            SQLiteCommand checkHashCommand = new SQLiteCommand(DbFsConnection);
            checkHashCommand.CommandText = "SELECT COUNT(`file`) FROM `file_hash` WHERE `file`=@file and `hash`=@hash";
            checkHashCommand.Parameters.Add(new SQLiteParameter("@file", file.ToLower()));
            checkHashCommand.Parameters.Add(new SQLiteParameter("@hash", file_hash));

            bool bAreEqual = checkHashCommand.ExecuteScalar().ToString() != "0";
            return bAreEqual;
        }

        private void SaveFileHash(string file, byte[] file_hash, SQLiteTransaction transaction)
        {
            SQLiteCommand saveHashCommand = new SQLiteCommand(_sqlCommandInsertOrReplaceIntoFileHash, DbFsConnection, transaction);
            saveHashCommand.Parameters.Add(new SQLiteParameter("@file", file.ToLower()));
            saveHashCommand.Parameters.Add(new SQLiteParameter("@hash", file_hash));
            saveHashCommand.ExecuteNonQuery();
        }

        private void ParseGetImagesFile(string file, string table_name, string str_folder)
        {
            //only parse the file if it was changed since last time
            byte[] fileHash = null;
            if (CheckSavedFileHash(file, out fileHash) == false)
            {
                string strFileContent = File.ReadAllText(file);
                //Let's do one transaction per file to speed things up
                SQLiteTransaction sqlTransaction = DbFsConnection.BeginTransaction();
                //Create the table if it does not exist already

                SQLiteCommand command = new SQLiteCommand(_sqlCommandsCreationTables["images"], DbFsConnection, sqlTransaction);
                command.CommandText = string.Format(command.CommandText, table_name);
                command.ExecuteNonQuery();

                string[] splittedFileContent = strFileContent.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                //Ignore the first two since they will correspond to "nRows=2326" and "nCols=2"
                for (int index = 2; index < splittedFileContent.Length; index++)
                {
                    command.Reset();
                    //Access only to the anme of the image
                    string strImageName = splittedFileContent[index].Split('=')[1].TrimEnd(new char[] { '\r', '\n' });

                    string strImageBaseName = Path.GetFileNameWithoutExtension(strImageName);
                    if (strImageBaseName.StartsWith("x2")) //ignore this
                        continue;
                    command.CommandText = string.Format(_sqlCommandInsertOrReplaceIntoAnyTable,
                        table_name,//parameter {0} table name
                        $"`{string.Join("`,`", new List<string> { "name", "small", "big" })}`" //parameter {1} columns
                        );
                    command.AddArrayParameters("values", new List<string> { strImageBaseName,
                            Path.Combine(str_folder, "img", strImageName),
                            Path.Combine(str_folder, "img", "x2_" + strImageName)});
                    command.ExecuteNonQuery();
                }
                //now save file hash to table
                SaveFileHash(file, fileHash, sqlTransaction);
                //finally commit the transaction
                sqlTransaction.Commit();
            }
        }

        private void ParseType1(string str_folder, string str_mod_name, string str_mod_folder_name)
        {
            string strType1NeogameFile = Path.Combine(str_folder, "neogame.xml").ToLower();

            App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": neogame.xml");
            //only parse the file if it was changed since last time
            byte[] fileHash = null;
            if (CheckSavedFileHash(strType1NeogameFile, out fileHash) == false)
            {
                //Let's do one transaction per file to speed things up
                SQLiteTransaction sqlTransaction = DbFsConnection.BeginTransaction();
                //Create all tables for this mod
                SQLiteCommand command = new SQLiteCommand(DbFsConnection);
                command.Transaction = sqlTransaction;
                for (int nIndex = 0; nIndex < _listTableNameSufix.Count; nIndex++)
                {
                    command.Reset();
                    string strTableName = str_mod_name + "_" + str_mod_folder_name + "_" + _listTableNameSufix[nIndex];
                    command.CommandText = string.Format(_sqlCommandsCreationTables[_listTableNameSufix[nIndex]], strTableName);
                    command.ExecuteNonQuery();
                }
                //parse the xml file to DB
                ParseXMLFile(strType1NeogameFile, str_mod_name + "_" + str_mod_folder_name, sqlTransaction);
                //now save file hash to table
                SaveFileHash(strType1NeogameFile, fileHash, sqlTransaction);
                //finally commit the transaction
                sqlTransaction.Commit();
            }

            //After parsing all the data files let's check if there are any getimages.php to parse
            string strImageFile = Path.Combine(str_folder, "getimages.php");
            if (File.Exists(strImageFile))
            {
                App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": getimages.php");
                ParseGetImagesFile(strImageFile, str_mod_name + "_" + str_mod_folder_name + "_" + "images", str_folder);
            }
            //If everything went well we will add to the mod list
            App.I.Mods.Add(str_mod_name + "_" + str_mod_folder_name);
        }
        //Type 0 (ones with data folder)
        private void ParseType0(string str_folder, string str_mod_name, string str_mod_folder_name)
        {
            string strType0DataFolder = Path.Combine(str_folder, "data");
            IEnumerable<string> enumFiles = Directory.EnumerateFiles(strType0DataFolder);
            foreach (string file in enumFiles)
            {
                string strFileName = Path.GetFileName(file).ToLower();
                int nIndex = _listTableNameSufix.IndexOf(Path.GetFileNameWithoutExtension(strFileName));
                if (nIndex > -1 && (string.Compare(Path.GetExtension(strFileName), ".xml") == 0)) //only parse supported xml files
                {
                    App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": " + strFileName);
                    //only parse the file if it was changed since last time
                    byte[] fileHash = null;
                    if (CheckSavedFileHash(file, out fileHash) == false)
                    {
                        //Let's do one transaction per file to speed things up
                        SQLiteTransaction sqlTransaction = DbFsConnection.BeginTransaction();
                        //Create the table if it does not exist already
                        string strTableName = str_mod_name + "_" + str_mod_folder_name + "_" + _listTableNameSufix[nIndex];
                        SQLiteCommand command = new SQLiteCommand(_sqlCommandsCreationTables[_listTableNameSufix[nIndex]], DbFsConnection, sqlTransaction);
                        command.CommandText = string.Format(command.CommandText, strTableName);
                        command.ExecuteNonQuery();
                        //parse the xml file to DB
                        ParseXMLFile(file, str_mod_name + "_" + str_mod_folder_name, sqlTransaction);
                        //now save file hash to table
                        SaveFileHash(file, fileHash, sqlTransaction);
                        //finally commit the transaction
                        sqlTransaction.Commit();
                    }
                }
            }
            //After parsing all the data files let's check if there are any getimages.php to parse
            string strImageFile = Path.Combine(str_folder, "getimages.php");
            if (File.Exists(strImageFile))
            {
                App._splashScreen.UpdateMessage("Parsing " + str_mod_folder_name + ": getimages.php");
                ParseGetImagesFile(strImageFile, str_mod_name + "_" + str_mod_folder_name + "_" + "images", str_folder);
            }
            //If everything went well we will add to the mod list
            App.I.Mods.Add(str_mod_name + "_" + str_mod_folder_name);
        }

        private void ParseGetModsFile(string str_game_folder)
        {
            string strGetModsFile = Path.Combine(str_game_folder, "getmods.php");
            if (File.Exists(strGetModsFile))
            {
                string strGetModsContent = File.ReadAllText(strGetModsFile);
                string[] splittedContent = strGetModsContent.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                //let's ignore the first line
                for (int index = 1; index < splittedContent.Length; index++)
                {
                    string strModName = splittedContent[index].Split('=')[1].TrimEnd(new char[] { '\r', '\n' }); //split the name
                    index++; //increase the index since the folder should be next
                    string strModFolder = splittedContent[index].Split('=')[1].TrimEnd(new char[] { '\r', '\n', '\\', '/' }); //split the folder
                    string strModBaseFolder = string.Empty;
                    //to differenciate let's add two folders to the table name or else there could be collision
                    string[] splittedFolders = strModFolder.Split('/');
                    strModBaseFolder = splittedFolders[splittedFolders.Length - 2] + "_" + splittedFolders[splittedFolders.Length - 1];

                    //now let's check for mod type
                    string strModFullFolderPath = Path.Combine(str_game_folder, strModFolder);
                    string strType1NeogameFile = Path.Combine(strModFullFolderPath, "neogame.xml");
                    if (File.Exists(strType1NeogameFile))//If neogame.xml file exists, this is a type 1 mod
                    {
                        ParseType1(strModFullFolderPath, strModName, strModBaseFolder);
                    }
                    else
                    {
                        ParseType0(strModFullFolderPath, strModName, strModBaseFolder);
                    }
                }
            }
        }

        private void CreateInMemGameDatabase()
        {
            App._splashScreen.UpdateMessage("Finalizing game database");

#if DEBUG
            //For debug porpose only
            var dbFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NeoScavHelperTool", "mem.sqlite");
            _dbMemConnection = new SQLiteConnection(connectionString: "Data Source=" + dbFilename + ";Version=3;Compress=True;UTF8Encoding=True;");
#else
            _dbMemConnection = new SQLiteConnection(connectionString: "Data Source=:memory:;Version=3;Compress=True;UTF8Encoding=True;");
#endif
            DbMemConnection.Open();

            //let's fill the TreeViewViewerTypes with the types
            _listTableNameSufix.ForEach(type =>
            {
                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = type;
                MainWindow.ListTypeTreeItems.Add(treeItem);
            });
            //let's fill the TreeViewViewerMods with the mods where 0 will only be represented as 0_vanilla
            App.I.Mods.Select(mod => mod.Split('_')[0]).Distinct().ToList().ForEach(modPrefix =>
            {
                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = modPrefix;
                MainWindow.ListModsTreeItems.Add(treeItem);
            });

            //Now we will fetch by mod order data to fill the DB overwriting the previous mod data
            SQLiteCommand fsCommand = new SQLiteCommand(DbFsConnection);
            foreach (string mod in App.I.Mods)
            {
                fsCommand.Reset();
                fsCommand.CommandText = _sqlCommandSelectTablesWithNameLike;
                fsCommand.Parameters.Add(new SQLiteParameter("@name", mod + "%"));
                //First get existent tables in file system database for this mod
                List<string> listTables = new List<string>();
                using (SQLiteDataReader reader = fsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listTables.Add(reader.GetString(0));
                    }
                }
                //now fetch all values from the file system table and insert them on the memory table                
                foreach (string tableName in listTables)
                {
                    SQLiteTransaction dataTableTransaction = DbMemConnection.BeginTransaction(); //transaction per table                    
                    SQLiteCommand command = new SQLiteCommand(DbMemConnection);
                    command.Transaction = dataTableTransaction;
                    //First fetch table column names
                    fsCommand.Reset();
                    fsCommand.CommandText = string.Format(_sqlCommandPragmaTableInfo, tableName);
                    List<string> listColumnNames = new List<string>();
                    using (SQLiteDataReader reader = fsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listColumnNames.Add(reader.GetString(1));
                        }
                    }
                    //Now fetch their values while insert them into the mem DB
                    fsCommand.Reset();
                    fsCommand.CommandText = string.Format(_sqlCommandSelectAllTableData, tableName);
                    using (SQLiteDataReader reader = fsCommand.ExecuteReader())
                    {
                        if (reader.HasRows) //continue only if there are values on this table
                        {
                            string[] splittedTableName = tableName.Split('_');
                            string strTableNameSufix = splittedTableName[splittedTableName.Length - 1];
                            string[] splittedModName = mod.Split('_');
                            string strMemTableName = splittedModName[0] + '_' + strTableNameSufix;
                            //create this mod table if needed
                            command.CommandText = string.Format(_sqlCommandsCreationTables[strTableNameSufix], strMemTableName);
                            command.ExecuteNonQuery();

                            //now we need to find or add the the table prefix in the tree view
                            //Type group
                            //1st Get the existent item
                            TreeViewItem typeNode0Item = MainWindow.ListTypeTreeItems.Find(item => string.Compare(item.Header.ToString(), strTableNameSufix) == 0);
                            //2nd check if it has the wanted table item
                            TreeViewItem typeNode1Item = typeNode0Item.Items.Cast<TreeViewItem>().FirstOrDefault(item => string.Compare(item.Header.ToString(), splittedModName[0]) == 0);
                            if (typeNode1Item == null)
                            {
                                typeNode1Item = new TreeViewItem();
                                typeNode1Item.Header = splittedModName[0];
                                typeNode0Item.Items.Add(typeNode1Item);
                            }
                            //Mod group
                            //1st Get the existent item
                            TreeViewItem modNode0Item = MainWindow.ListModsTreeItems.Find(item => string.Compare(item.Header.ToString(), splittedModName[0]) == 0);
                            //2nd check if it has the wanted table item
                            TreeViewItem modNode1Item = modNode0Item.Items.Cast<TreeViewItem>().FirstOrDefault(item => string.Compare(item.Header.ToString(), strTableNameSufix) == 0);
                            if (modNode1Item == null)
                            {
                                modNode1Item = new TreeViewItem();
                                modNode1Item.Header = strTableNameSufix;
                                modNode0Item.Items.Add(modNode1Item);
                            }

                            while (reader.Read())
                            {
                                object[] arrayColumnValues = new object[listColumnNames.Count];                                
                                reader.GetValues(arrayColumnValues);
                                List<object> listColumnValues = arrayColumnValues.ToList();

                                command.Reset();
                                command.CommandText = string.Format(_sqlCommandInsertOrReplaceIntoAnyTable,
                                    strMemTableName,//parameter {0} table name
                                    $"`{string.Join("`,`", listColumnNames)}`" //parameter {1} columns
                                    );
                                command.AddArrayParameters("values", listColumnValues);
                                command.ExecuteNonQuery();

                                //Fill the tree view items with the wanted info
                                TreeViewItemLeaf.DataType dataType = (TreeViewItemLeaf.DataType)_listTableNameSufix.IndexOf(strTableNameSufix);
                                int[] wantedValuesColumns = _listTableWantedValues[(int)dataType];

                                string strFirst = string.Empty;
                                bool bIsSecondPrimaryKey = false;
                                if (wantedValuesColumns[0] >= 0)
                                {
                                    strFirst = listColumnValues[wantedValuesColumns[0]].ToString();
                                }
                                else
                                    bIsSecondPrimaryKey = true;
                                string strSecond = string.Empty;
                                if (wantedValuesColumns[1] >= 0)
                                {
                                    strSecond = listColumnValues[wantedValuesColumns[1]].ToString();
                                }
                                TreeViewItemLeaf newItem = new TreeViewItemLeaf(strFirst, strSecond, listColumnNames[0],
                                    bIsSecondPrimaryKey, dataType, strMemTableName);
                                //Finally add items to both tree views if they are not there already
                                if (0 == typeNode1Item.Items.Cast<TreeViewItemLeaf>().Where(item => string.Compare(item.PrimaryKeyValue, newItem.PrimaryKeyValue) == 0).Count())
                                {
                                    typeNode1Item.Items.Add(newItem);
                                    modNode1Item.Items.Add(newItem);
                                }
                            }
                        }
                    }
                    dataTableTransaction.Commit();
                }
            }
        }

        public List<object> GetColumnsValuesFromMemoryTable(string str_column_search, string str_column_value_search,
            string str_table_name, List<string> list_column_values_retrieves)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnsFromTableWhereColumnEqualValue,
                $"`{string.Join("`,`", list_column_values_retrieves)}`", str_table_name, str_column_search);
            command.Parameters.Add(new SQLiteParameter("value", str_column_value_search));
            object[] values = new object[list_column_values_retrieves.Count];
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                reader.GetValues(values);
            }

            return values.ToList();
        }

        public List<object> GetColumnsValuesFromMemoryTableMultipleAndConditions(List<string> list_column_search, List<string> list_column_value_search,
            string str_table_name, List<string> list_column_values_retrieves)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnsFromTableWhereMultipleColumnEqualValue,
                $"`{string.Join("`,`", list_column_values_retrieves)}`", str_table_name);
            command.AddMultipleAndConditions("multiple", list_column_search, list_column_value_search);
            List<object> listReturn = null; 
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    object[] values = new object[list_column_values_retrieves.Count];
                    reader.Read();
                    reader.GetValues(values);
                    listReturn = values.ToList();
                }
            }

            return listReturn;
        }

        public string GetColumnValueFromMemoryTableMultipleAndConditions(List<string> list_column_search, List<string> list_column_value_search,
            string str_table_name, string str_column_value_retrieve)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnFromTableWhereMultipleColumnEqualValue, str_column_value_retrieve, str_table_name);
            command.AddMultipleAndConditions("multiple", list_column_search, list_column_value_search);

            return command.ExecuteScalar().ToString();
        }

        public string GetColumnValueFromMemoryTable(string str_column_search, string str_column_value_search,
            string str_table_name, string str_column_value_retrieve)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnFromTableWhereColumnEqualValue, str_column_value_retrieve, str_table_name, str_column_search);
            command.Parameters.Add(new SQLiteParameter("@value", str_column_value_search));

            return command.ExecuteScalar().ToString();
        }

        public string GetImagePathFromMemory(string str_name, string str_table_name, bool is_big)
        {
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectColumnFromTableWhereColumnEqualValue, is_big ? "big" : "small", str_table_name, "name");
            command.Parameters.Add(new SQLiteParameter("@value", str_name));

            return command.ExecuteScalar().ToString();
        }

        public List<object> GetAllDataOfAnItemFromMemory(string str_value, string str_column_name, string str_table_name)
        {
            List<object> listValues = new List<object>();
            SQLiteCommand command = new SQLiteCommand(_dbMemConnection);
            command.CommandText = string.Format(_sqlCommandSelectAllFromTableWhereColumnEqualValue, str_table_name, str_column_name);
            command.Parameters.Add(new SQLiteParameter("@value", str_value));
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                for (int column = 0; column < reader.FieldCount; column++)
                    listValues.Add(reader.GetValue(column));
            }

            return listValues;
        }
    }
}
