using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Helpers;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services
{
    public class ModsMetadataService
    {
        private readonly ILogger<ModsMetadataService> _logger;
        private readonly DatabaseService _dbService;

        private int _nextOrder = -1;
        private Dictionary<string, ModMetadata> _modsMetadata = new();

        public ICollection<string> Mods => _modsMetadata.Keys;

        public ModsMetadataService(ILogger<ModsMetadataService> logger, DatabaseService dbService)
        {
            this._logger = logger;
            this._dbService = dbService;
        }

        public void AddTableToMod(ModInfo mod, string table)
        {
            ModMetadata modMetadata;
            if (!_modsMetadata.TryGetValue(mod.Name, out modMetadata))
            {
                modMetadata = new ModMetadata() { Name = mod.Name, Order = _nextOrder++ };
                _modsMetadata.Add(mod.Name, modMetadata);
            }

            switch (table)
            {
                case "attackmodes":
                    modMetadata.HasAttackModes = true;
                    break;
                case "barterhexes":
                    modMetadata.HasBarterHexes = true;
                    break;
                case "battlemoves":
                    modMetadata.HasBattleMoves = true;
                    break;
                case "camptypes":
                    modMetadata.HasCampTypes = true;
                    break;
                case "chargeprofiles":
                    modMetadata.HasChargeProfiles = true;
                    break;
                case "conditions":
                    modMetadata.HasConditions = true;
                    break;
                case "containertypes":
                    modMetadata.HasContainerTypes = true;
                    break;
                case "creatures":
                    modMetadata.HasCreatures = true;
                    break;
                case "creaturesources":
                    modMetadata.HasCreatureSources = true;
                    break;
                case "datafiles":
                    modMetadata.HasDataFiles = true;
                    break;
                case "dmcplaces":
                    modMetadata.HasDmcPlaces = true;
                    break;
                case "encounters":
                    modMetadata.HasEncounters = true;
                    break;
                case "encountertriggers":
                    modMetadata.HasEncounterTriggers = true;
                    break;
                case "factions":
                    modMetadata.HasFactions = true;
                    break;
                case "forbiddenhexes":
                    modMetadata.HasForbiddenHexes = true;
                    break;
                case "gamevars":
                    modMetadata.HasGameVars = true;
                    break;
                case "headlines":
                    modMetadata.HasHeadlines = true;
                    break;
                case "hextypes":
                    modMetadata.HasHexTypes = true;
                    break;
                case "ingredients":
                    modMetadata.HasIngredients = true;
                    break;
                case "itemprops":
                    modMetadata.HasItemProps = true;
                    break;
                case "itemtypes":
                    modMetadata.HasItemTypes = true;
                    break;
                case "maps":
                    modMetadata.HasMaps = true;
                    break;
                case "recipes":
                    modMetadata.HasRecipes = true;
                    break;
                case "treasuretable":
                    modMetadata.HasTreasureTable = true;
                    break;
                case "images":
                    modMetadata.HasImages = true;
                    break;
                default:
                    throw new Exception($"Invalid table: \"{table}\"");
            }
        }

        public void PersistModsMetadata()
        {
            _dbService.Connection.InsertAll(_modsMetadata.Select((pair) => pair.Value));
        }

        public ICollection<string> GetModTypes(string modName)
        {
            return _modsMetadata[modName].Types;
        }
    }
}
