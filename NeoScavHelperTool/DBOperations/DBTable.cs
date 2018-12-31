using NeoScavModHelperTool.DBTableAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NeoScavModHelperTool
{
    //Enum with main game database tables and their needed attributes
    public enum EDBTable
    {
        [
            NameSufix("attackmodes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strNotes` TEXT NOT NULL DEFAULT '', `nRange` INTEGER NOT NULL DEFAULT 1, `fDamageCut` REAL NOT NULL DEFAULT 0, `fDamageBlunt` REAL NOT NULL DEFAULT 0, `strChargeProfiles` TEXT NOT NULL, `nPenetration` INTEGER NOT NULL DEFAULT 0, `nType` INTEGER NOT NULL DEFAULT 0, `strSnd`TEXT NOT NULL, `bTransfer` INTEGER NOT NULL DEFAULT 0, `vAttackerConditions`TEXT NOT NULL, `strIMG` TEXT NOT NULL, `fMorale` REAL NOT NULL DEFAULT 0.25, `strWieldPhrase` TEXT NOT NULL, `vAttackPhrases` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "strNotes", "nRange", "fDamageCut", "fDamageBlunt", "strChargeProfiles", "nPenetration", "nType", "strSnd", "bTransfer", "vAttackerConditions", "strIMG", "fMorale", "strWieldPhrase", "vAttackPhrases" }),
            TreeViewDecriptiveColumnsPositions(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eAttackModes,

        [
            NameSufix("barterhexes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `nX` INTEGER NOT NULL DEFAULT 0, `nY` INTEGER NOT NULL DEFAULT 0, `bBuys` INTEGER NOT NULL DEFAULT 0, `nRestockTreasureID` INTEGER NOT NULL DEFAULT 3, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "nX", "nY", "bBuys", "nRestockTreasureID" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0 }),
            PrimaryKeyName("id")
            ]
        eBarterHexes,

        [
            NameSufix("battlemoves"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strID` TEXT NOT NULL, `strName`	TEXT NOT NULL, `strNotes` TEXT NOT NULL, `strSuccess` TEXT NOT NULL, `strFail` TEXT, `strPopUp` TEXT DEFAULT NULL, `vChanceType` TEXT NOT NULL DEFAULT '0,0,0', `vUsConditions` TEXT DEFAULT NULL, `vThemConditions` TEXT DEFAULT NULL, `vPairConditions` TEXT DEFAULT NULL, `vUsFailConditions` TEXT DEFAULT NULL, `vThemFailConditions` TEXT DEFAULT NULL, `vPairFailConditions` TEXT DEFAULT NULL, `vUsPreConditions` TEXT DEFAULT NULL, `vThemPreConditions` TEXT DEFAULT NULL, `nSeeThem` INTEGER DEFAULT 2, `nSeeUs` INTEGER DEFAULT 2, `bAllOutOfRange` INTEGER DEFAULT 0, `bInAttackRange` INTEGER DEFAULT 0, `nMinCharges` INTEGER DEFAULT 0, `nMinRange` INTEGER DEFAULT -1, `nMaxRange` INTEGER DEFAULT -1, `nAttackModeType` INTEGER DEFAULT -1, `vHexTypes` TEXT NOT NULL, `fChance` REAL DEFAULT 1, `fPriority` REAL DEFAULT 0, `fDetect` REAL DEFAULT 1, `fOrder` REAL DEFAULT 0.5, `fFatigue` REAL DEFAULT 0, `bApproach` INTEGER DEFAULT 0, `bOffense` INTEGER DEFAULT 0, `bFallBack` INTEGER DEFAULT 0, `bRetreat` INTEGER DEFAULT 0, `bPosition` INTEGER DEFAULT 0, `bPassive` INTEGER NOT NULL DEFAULT 0, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strID", "strName", "strNotes", "strSuccess", "strFail", "strPopUp", "vChanceType", "vUsConditions", "vThemConditions", "vPairConditions", "vUsFailConditions", "vThemFailConditions", "vPairFailConditions", "vUsPreConditions", "vThemPreConditions", "nSeeThem", "nSeeUs", "bAllOutOfRange", "bInAttackRange", "nMinCharges", "nMinRange", "nMaxRange", "nAttackModeType", "vHexTypes", "fChance", "fPriority", "fDetect", "fOrder", "fFatigue", "bApproach", "bOffense", "bFallBack", "bRetreat", "bPosition", "bPassive" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 2 }),
            PrimaryKeyName("id")
            ]
        eBattleMoves,

        [
            NameSufix("camptypes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strDesc` TEXT NOT NULL, `vImageList` TEXT NOT NULL DEFAULT 'ItmScavengeGrass01.png', `aCapacities` TEXT NOT NULL DEFAULT '30x30', `nTreasureID` INTEGER NOT NULL DEFAULT 3, `m_fAlertness` REAL NOT NULL DEFAULT 0, `m_fVisibility` REAL NOT NULL DEFAULT -0.05, `WetTempAdjustMod` REAL NOT NULL DEFAULT 0, `m_fHealPerHourMod` REAL NOT NULL DEFAULT 0, `fSleepQuality` REAL NOT NULL DEFAULT 0, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strDesc", "vImageList", "aCapacities", "nTreasureID", "m_fAlertness", "m_fVisibility", "WetTempAdjustMod", "m_fHealPerHourMod", "fSleepQuality" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eCampTypes,

        [
            NameSufix("chargeprofiles"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`nID` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strItemID` TEXT NOT NULL, `fPerUse` REAL NOT NULL DEFAULT 0, `fPerHour` REAL NOT NULL DEFAULT 0, `fPerHourEquipped` REAL NOT NULL DEFAULT 0, `fPerHex` REAL DEFAULT 0, `bDegrade` INTEGER NOT NULL DEFAULT 0, PRIMARY KEY(`nID`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "nID", "strName", "strItemID", "fPerUse", "fPerHour", "fPerHourEquipped", "fPerHex", "bDegrade" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("nID")
            ]
        eChargeProfiles,

        [
            NameSufix("conditions"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strDesc` TEXT NOT NULL, `aFieldNames` TEXT NOT NULL, `aModifiers` TEXT NOT NULL, `aEffects` TEXT NOT NULL, `bFatal` INTEGER NOT NULL DEFAULT 0, `vIDNext` TEXT NOT NULL DEFAULT '0', `fDuration` REAL NOT NULL DEFAULT 0, `bPermanent` INTEGER NOT NULL DEFAULT 0, `vChanceNext` TEXT NOT NULL DEFAULT '0', `bStackable` INTEGER NOT NULL DEFAULT 0, `bDisplay` INTEGER NOT NULL DEFAULT 1, `bDisplayOther` INTEGER NOT NULL DEFAULT 0, `bDisplayGameOver` INTEGER NOT NULL DEFAULT 1, `nColor` INTEGER NOT NULL DEFAULT 0, `bResetTimer` INTEGER NOT NULL DEFAULT 1, `bRemoveAll` INTEGER NOT NULL DEFAULT 0, `bRemovePostCombat` INTEGER NOT NULL DEFAULT 0, `nTransferRange` INTEGER NOT NULL DEFAULT -1, `aThresholds` TEXT NOT NULL DEFAULT '', PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "strDesc", "aFieldNames", "aModifiers", "aEffects", "bFatal", "vIDNext", "fDuration", "bPermanent", "vChanceNext", "bStackable", "bDisplay", "bDisplayOther", "bDisplayGameOver", "nColor", "bResetTimer", "bRemoveAll", "bRemovePostCombat", "nTransferRange", "aThresholds" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eConditions,

        [
            NameSufix("containertypes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eContainerTypes,

        [
            NameSufix("creatures"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strNamePublic` TEXT NOT NULL, `strNotes` TEXT NOT NULL, `strImg` TEXT NOT NULL, `vEncounterIDs` TEXT NOT NULL, `nMovesPerTurn` INTEGER NOT NULL, `nTreasureID` INTEGER NOT NULL DEFAULT 3, `nFaction` INTEGER NOT NULL DEFAULT 0, `vAttackModes` TEXT NOT NULL, `vBaseConditions` TEXT NOT NULL, `nCorpseID` INTEGER NOT NULL DEFAULT 3, `vActivities` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "strNamePublic", "strNotes", "strImg", "vEncounterIDs", "nMovesPerTurn", "nTreasureID", "nFaction", "vAttackModes", "vBaseConditions", "nCorpseID", "vActivities" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eCreatures,

        [
            NameSufix("creaturesources"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `nX` INTEGER NOT NULL DEFAULT -1, `nY` INTEGER NOT NULL DEFAULT -1, `nCreatureID` INTEGER NOT NULL DEFAULT 0, `nMin` INTEGER NOT NULL DEFAULT 0, `nMax` INTEGER NOT NULL DEFAULT 0, `fWeight` REAL NOT NULL DEFAULT 1, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "nX", "nY", "nCreatureID", "nMin", "nMax", "fWeight" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eCreatureSources,

        [
            NameSufix("datafiles"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strDesc` TEXT NOT NULL, `fValue` REAL NOT NULL DEFAULT 0, `strImg` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "strDesc", "fValue", "strImg" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 2 }),
            PrimaryKeyName("id")
            ]
        eDatafiles,

        [
            NameSufix("dmcplaces"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strImg` TEXT NOT NULL, `nEncounterID` INTEGER NOT NULL DEFAULT 1, `nX` INTEGER NOT NULL DEFAULT 0, `nY` INTEGER NOT NULL DEFAULT 0, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strImg", "nEncounterID", "nX", "nY" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eDMCPlaces,

        [
            NameSufix("encounters"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strDesc` TEXT NOT NULL, `strImg` TEXT NOT NULL DEFAULT 'EncBlank.png', `nTreasureID` INTEGER NOT NULL DEFAULT 3, `nRemoveTreasureID` INTEGER NOT NULL DEFAULT 3, `aConditions` TEXT NOT NULL DEFAULT '1', `aPreConditions` TEXT NOT NULL DEFAULT '', `fPrice` REAL NOT NULL DEFAULT 0, `aResponses` TEXT NOT NULL, `aMinimapHexes` TEXT NOT NULL DEFAULT '', `bRemoveCreatures` INTEGER NOT NULL DEFAULT 0, `bRemoveUsed` INTEGER NOT NULL DEFAULT 0, `nItemsID` INTEGER NOT NULL DEFAULT 3, `nCreatureID` INTEGER NOT NULL DEFAULT 0, `ptCreatureHex` TEXT NOT NULL DEFAULT '0,0', `ptTeleport` TEXT NOT NULL DEFAULT '0,0', `ptEditor` TEXT NOT NULL DEFAULT '0,0', `nType` INTEGER NOT NULL DEFAULT 0, `fLootChance` REAL NOT NULL DEFAULT 0, `fAccidentChance` REAL NOT NULL DEFAULT 0, `fCreatureChance` REAL NOT NULL DEFAULT 0, `vAccidents` TEXT NOT NULL DEFAULT '1', `vLoot` TEXT NOT NULL DEFAULT '3', PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "strDesc", "strImg", "nTreasureID", "nRemoveTreasureID", "aConditions", "aPreConditions", "fPrice", "aResponses", "aMinimapHexes", "bRemoveCreatures", "bRemoveUsed", "nItemsID", "nCreatureID", "ptCreatureHex", "ptTeleport", "ptEditor", "nType", "fLootChance", "fAccidentChance", "fCreatureChance", "vAccidents", "vLoot" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eEncounters,

        [
            NameSufix("encountertriggers"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `nEncounterID` INTEGER NOT NULL, `fChance` REAL NOT NULL, `bLocBased` INTEGER NOT NULL, `bDateBased` INTEGER NOT NULL, `bHexBased` INTEGER NOT NULL, `bUnique` INTEGER NOT NULL, `bAIPassable` INTEGER NOT NULL DEFAULT 1, `aArea` TEXT NOT NULL, `dateMin` TEXT NOT NULL, `dateMax` TEXT NOT NULL, `aHexTypes` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "nEncounterID", "fChance", "bLocBased", "bDateBased", "bHexBased", "bUnique", "bAIPassable", "aArea", "dateMin", "aHexTypes" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eEncounterTriggers,

        [
           NameSufix("factions"),
           SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `dictFactions` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
           ColumnsNames(new string[] { "id", "strName", "dictFactions" }),
           TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
           ]
        eFactions,

        [
            NameSufix("forbiddenhexes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `nX` INTEGER NOT NULL DEFAULT 0, `nY` INTEGER NOT NULL DEFAULT 0, `strName` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "nX", "nY", "strName" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 3 }),
            PrimaryKeyName("id")
            ]
        eForbiddenHexes,

        [
            NameSufix("gamevars"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`strName` TEXT NOT NULL, `strType` TEXT NOT NULL, `strValue` TEXT NOT NULL, PRIMARY KEY(`strName`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "strName", "strType", "strValue" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 1 }),
            PrimaryKeyName("strName")
            ]
        eGameVars,

        [
            NameSufix("headlines"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strHeadline` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strHeadline" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eHeadlines,

        [
            NameSufix("hextypes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strDesc` TEXT NOT NULL, `nTerrainCost` INTEGER NOT NULL, `nVizLimiter` INTEGER NOT NULL, `nVizIncrease` INTEGER NOT NULL, `nTreasureID` INTEGER NOT NULL, `bPassable` INTEGER NOT NULL, `nScavengeInitialID` INTEGER NOT NULL DEFAULT 3, `nScavengeItemsIDPerHour` INTEGER NOT NULL DEFAULT 25, `nCampItems` INTEGER NOT NULL DEFAULT 5, `vLightLevels` TEXT NOT NULL DEFAULT '0.57,1.0,0.57,0.15', `nDefaultCampID` INTEGER NOT NULL DEFAULT 517, `nMinRange` INTEGER NOT NULL DEFAULT 3, `nMaxRange` INTEGER NOT NULL DEFAULT 6, `vCondIDs` TEXT NOT NULL, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "strDesc", "nTerrainCost", "nVizLimiter", "nVizIncrease", "nTreasureID", "bPassable", "nScavengeInitialID", "nScavengeItemsIDPerHour", "nCampItems", "vLightLevels", "nDefaultCampID", "nMinRange", "nMaxRange", "vCondIDs" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eHextypes,

        [
            NameSufix("images"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`name` TEXT NOT NULL, `small` TEXT NOT NULL DEFAULT '', `big` TEXT NOT NULL DEFAULT '', PRIMARY KEY(`name`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "name", "small", "big" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0 }),
            PrimaryKeyName("name")
            ]
        eImages,

        [
            NameSufix("ingredients"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`nID` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strRequiredProps` TEXT NOT NULL, `strForbidProps` TEXT NOT NULL, PRIMARY KEY(`nID`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "nID", "strName", "strRequiredProps", "strForbidProps" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("nID")
            ]
        eIngredients,

        [
            NameSufix("itemprops"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`nID` INTEGER NOT NULL, `strPropertyName` TEXT NOT NULL, PRIMARY KEY(`nID`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "nID", "strPropertyName" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("nID")
            ]
        eItemProps,

        [
            NameSufix("itemtypes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `nGroupID` INTEGER NOT NULL, `nSubgroupID` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strDesc` TEXT NOT NULL, `strDescAlt` TEXT NOT NULL, `nCondID` INTEGER NOT NULL DEFAULT 1, `vImageList` TEXT NOT NULL, `vSpriteList` TEXT NOT NULL, `vImageUsage` TEXT NOT NULL, `fWeight` REAL NOT NULL DEFAULT 0, `fMonetaryValue` REAL NOT NULL DEFAULT 0, `fMonetaryValueAlt` REAL NOT NULL DEFAULT 0, `fDurability` REAL NOT NULL DEFAULT 1, `fDegradePerHour` REAL NOT NULL DEFAULT 0, `fEquipDegradePerHour` REAL NOT NULL DEFAULT 0, `fDegradePerUse` REAL NOT NULL DEFAULT 0, `vDegradeTreasureIDs` TEXT NOT NULL DEFAULT '3,3', `aEquipConditions` TEXT NOT NULL, `aPossessConditions` TEXT NOT NULL, `aUseConditions` TEXT NOT NULL, `aCapacities` TEXT NOT NULL, `vEquipSlots` TEXT NOT NULL, `vUseSlots` TEXT NOT NULL, `bSocketLocked` INTEGER NOT NULL DEFAULT 0, `vProperties` TEXT NOT NULL, `aContentIDs` TEXT NOT NULL, `nFormatID` TEXT NOT NULL DEFAULT '3', `nTreasureID` TEXT NOT NULL DEFAULT '3', `nComponentID` INTEGER NOT NULL DEFAULT 3, `bMirrored` INTEGER NOT NULL DEFAULT 0, `nSlotDepth` INTEGER NOT NULL DEFAULT 0, `strChargeProfiles` TEXT NOT NULL, `aAttackModes` TEXT NOT NULL, `nStackLimit` INTEGER NOT NULL DEFAULT 1, `aSwitchIDs` TEXT NOT NULL DEFAULT '', `aSounds` TEXT NOT NULL DEFAULT 'cuePickup,cuePutdown', PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "nGroupID", "nSubgroupID", "strName", "strDesc", "strDescAlt", "nCondID", "vImageList", "vSpriteList", "vImageUsage", "fWeight", "fMonetaryValue", "fMonetaryValueAlt", "fDurability", "fDegradePerHour", "fEquipDegradePerHour", "fDegradePerUse", "vDegradeTreasureIDs", "aEquipConditions", "aPossessConditions", "aUseConditions", "aCapacities", "vEquipSlots", "vUseSlots", "bSocketLocked", "vProperties", "aContentIDs", "nFormatID", "nTreasureID", "nComponentID", "bMirrored", "nSlotDepth", "strChargeProfiles", "aAttackModes", "nStackLimit", "aSwitchIDs", "aSounds" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 3 }),
            PrimaryKeyName("id")
            ]
        eItemTypes,

        [
            NameSufix("maps"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (" +
            "`id` INTEGER NOT NULL," +
            "`strName` TEXT NOT NULL," +
            "`strDef` TEXT NOT NULL," +
            "PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "strDef" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eMaps,

        [
            NameSufix("recipes"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`nID` INTEGER NOT NULL, `strName` TEXT NOT NULL, `strSecretName` TEXT NOT NULL DEFAULT '', `strTools` TEXT NOT NULL DEFAULT '', `strConsumed` TEXT NOT NULL DEFAULT '', `strDestroyed` TEXT NOT NULL DEFAULT '', `nTreasureID` TEXT NOT NULL DEFAULT '3', `fHours` REAL NOT NULL DEFAULT 0, `nReverse` INTEGER NOT NULL DEFAULT 0, `nHiddenID` TEXT NOT NULL DEFAULT '0', `bIdentify` INTEGER NOT NULL DEFAULT 0, `bTransferComponents` INTEGER NOT NULL DEFAULT 0, `vAlsoTry` TEXT NOT NULL, `nTempTreasureID` INTEGER NOT NULL DEFAULT 3, `bDegradeOutput` INTEGER NOT NULL DEFAULT 1, `strType` TEXT NOT NULL DEFAULT '', `bScrap` INTEGER NOT NULL DEFAULT 1, PRIMARY KEY(`nID`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "nID", "strName", "strSecretName", "strTools", "strConsumed", "strDestroyed", "nTreasureID", "fHours", "nReverse", "nHiddenID", "bIdentify", "bTransferComponents", "vAlsoTry", "nTempTreasureID", "bDegradeOutput", "strType", "bScrap" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eRecipes,

        [
            NameSufix("treasuretable"),
            SqlCreation("CREATE TABLE IF NOT EXISTS `{0}` (`id` INTEGER NOT NULL, `strName` TEXT NOT NULL, `aTreasures` TEXT NOT NULL, `bNested` INTEGER NOT NULL DEFAULT 0, `bSuppress` INTEGER NOT NULL DEFAULT 0, `bIdentify` INTEGER NOT NULL DEFAULT 0, PRIMARY KEY(`id`)) WITHOUT ROWID;"),
            ColumnsNames(new string[] { "id", "strName", "aTreasures", "bNested", "bSuppress", "bIdentify" }),
            TreeViewDecriptiveColumnsPositionsAttribute(new int[] { 0, 1 }),
            PrimaryKeyName("id")
            ]
        eTreasuretable,                
        eTotal // this must be the last element always!!!!!!!
    }

    public enum EDBImagesTableColumns
    {
        eName,
        eSmall,
        eBig,
        eTotal
    }

    namespace DBTableAttributes
    {
        #region Table Attributes
        public class NameSufixAttribute : Attribute
        {
            private string _value;
            public NameSufixAttribute(string value)
            {
                _value = value;
            }
            public string Value
            {
                get { return _value; }
            }
        }

        public class PrimaryKeyNameAttribute : Attribute
        {
            private string _value;
            public PrimaryKeyNameAttribute(string value)
            {
                _value = value;
            }
            public string Value
            {
                get { return _value; }
            }
        }

        public class TreeViewDecriptiveColumnsPositionsAttribute : Attribute
        {
            private int[] _value;
            public TreeViewDecriptiveColumnsPositionsAttribute(int[] value)
            {
                _value = value;
            }
            public int[] Value
            {
                get { return _value; }
            }
        }

        public class ColumnsNamesAttribute : Attribute
        {
            private string[] _value;
            public ColumnsNamesAttribute(string[] value)
            {
                _value = value;
            }
            public string[] Value
            {
                get { return _value; }
            }
        }

        public class SqlCreationAttribute : Attribute
        {
            private string _value;
            public SqlCreationAttribute(string value)
            {
                _value = value;
            }
            public string Value
            {
                get { return _value; }
            }
        }
        #endregion

        public class DBTableAttributtesFetcher
        {
            private static Hashtable _NameSufixValues = new Hashtable();
            private static Hashtable _PrimaryKeyNameValues = new Hashtable();
            private static Hashtable _SqlCreation = new Hashtable();
            private static Hashtable _TreeViewDecriptiveColumnsPositions = new Hashtable();
            private static Hashtable _ColumnsNames = new Hashtable();

            public static List<string> GetAllNameSufix()
            {
                List<string> output = new List<string>();
                Type type = typeof(EDBTable);

                //Look for our string value associated with fields in this enum
                foreach (FieldInfo fi in type.GetFields())
                {
                    //Check for our custom attribute
                    NameSufixAttribute[] attrs = fi.GetCustomAttributes(typeof(NameSufixAttribute), false) as NameSufixAttribute[];
                    if (attrs.Length > 0)
                        output.Add(attrs[0].Value);
                }

                return output;
            }

            public static string GetNameSufix(EDBTable value)
            {
                string output = null;
                Type type = typeof(EDBTable);

                if (_NameSufixValues.ContainsKey(value))
                    output = (_NameSufixValues[value] as NameSufixAttribute).Value;
                else
                {
                    //Look for our 'NameSufixAttribute' in the field's custom attributes
                    FieldInfo fi = type.GetField(value.ToString());
                    NameSufixAttribute[] attrs = fi.GetCustomAttributes(typeof(NameSufixAttribute), false) as NameSufixAttribute[];
                    if (attrs.Length > 0)
                    {
                        _NameSufixValues.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
                }
                return output;
            }

            public static string GetPrimaryKeyName(EDBTable value)
            {
                string output = null;
                Type type = typeof(EDBTable);

                if (_PrimaryKeyNameValues.ContainsKey(value))
                    output = (_PrimaryKeyNameValues[value] as PrimaryKeyNameAttribute).Value;
                else
                {
                    //Look for our 'PrimaryKeyNameAttribute' in the field's custom attributes
                    FieldInfo fi = type.GetField(value.ToString());
                    PrimaryKeyNameAttribute[] attrs = fi.GetCustomAttributes(typeof(PrimaryKeyNameAttribute), false) as PrimaryKeyNameAttribute[];
                    if (attrs.Length > 0)
                    {
                        _PrimaryKeyNameValues.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
                }
                return output;
            }

            public static int[] GetTreeViewDecriptiveColumnsPositions(EDBTable value)
            {
                int[] output = null;
                Type type = typeof(EDBTable);

                if (_TreeViewDecriptiveColumnsPositions.ContainsKey(value))
                    output = (_TreeViewDecriptiveColumnsPositions[value] as TreeViewDecriptiveColumnsPositionsAttribute).Value;
                else
                {
                    //Look for our 'TreeViewDecriptiveColumnsPositionsAttribute' in the field's custom attributes
                    FieldInfo fi = type.GetField(value.ToString());
                    TreeViewDecriptiveColumnsPositionsAttribute[] attrs = fi.GetCustomAttributes(typeof(TreeViewDecriptiveColumnsPositionsAttribute), false) as TreeViewDecriptiveColumnsPositionsAttribute[];
                    if (attrs.Length > 0)
                    {
                        _TreeViewDecriptiveColumnsPositions.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
                }

                return output;
            }

            public static List<string> GetTreeViewDecriptiveColumnsNames(EDBTable value)
            {
                List<string> output = new List<string>();

                string[] strColumnsNames = GetColumnsNames(value);
                foreach (int nColumnIndex in GetTreeViewDecriptiveColumnsPositions(value))
                {
                    output.Add(strColumnsNames[nColumnIndex]);
                }
              
                return output;
            }

            public static string GetSqlCreation(string table_name_sufix)
            {
                string output = null;

                object oDBTable = Parse(table_name_sufix);
                if(oDBTable != null)
                {
                    EDBTable eDBTable = (EDBTable)oDBTable;
                    output = GetSqlCreation(eDBTable);
                }

                return output;
            }

            public static string GetSqlCreation(EDBTable value)
            {
                string output = null;
                Type type = typeof(EDBTable);

                if (_SqlCreation.ContainsKey(value))
                    output = (_SqlCreation[value] as SqlCreationAttribute).Value;
                else
                {
                    //Look for our 'SqlCreationAttribute' in the field's custom attributes
                    FieldInfo fi = type.GetField(value.ToString());
                    SqlCreationAttribute[] attrs = fi.GetCustomAttributes(typeof(SqlCreationAttribute), false) as SqlCreationAttribute[];
                    if (attrs.Length > 0)
                    {
                        _SqlCreation.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
                }
                return output;
            }

            public static string[] GetColumnsNames(EDBTable value)
            {
                string [] output = null;
                Type type = typeof(EDBTable);

                if (_ColumnsNames.ContainsKey(value))
                    output = (_ColumnsNames[value] as ColumnsNamesAttribute).Value;
                else
                {
                    //Look for our 'ColumnsNamesAttribute' in the field's custom attributes
                    FieldInfo fi = type.GetField(value.ToString());
                    ColumnsNamesAttribute[] attrs = fi.GetCustomAttributes(typeof(ColumnsNamesAttribute), false) as ColumnsNamesAttribute[];
                    if (attrs.Length > 0)
                    {
                        _ColumnsNames.Add(value, attrs[0]);
                        output = attrs[0].Value;
                    }
                }
                return output;
            }

            public static object Parse(string table_name_sufix)
            {
                object output = null;
                string enumStringValue = null;
                Type type = typeof(EDBTable);

                //Look for our string value associated with fields in this enum
                foreach (FieldInfo fi in type.GetFields())
                {
                    //Check for our custom attribute
                    NameSufixAttribute[] attrs = fi.GetCustomAttributes(typeof(NameSufixAttribute), false) as NameSufixAttribute[];
                    if (attrs.Length > 0)
                        enumStringValue = attrs[0].Value;

                    //Check for equality then select actual enum value.
                    if (string.Compare(enumStringValue, table_name_sufix, true) == 0)
                    {
                        output = Enum.Parse(type, fi.Name);
                        break;
                    }
                }

                return output;
            }
        }
    }
}