using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Helpers;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Services.DataTypeHandlers;

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
                case AttackModesHandlerService.TABLE:
                    modMetadata.HasAttackModes = true;
                    break;
                case BarterHexesHandlerService.TABLE:
                    modMetadata.HasBarterHexes = true;
                    break;
                case BattleMovesHandlerService.TABLE:
                    modMetadata.HasBattleMoves = true;
                    break;
                case CampTypesHandlerService.TABLE:
                    modMetadata.HasCampTypes = true;
                    break;
                case ChargeProfilesHandlerService.TABLE:
                    modMetadata.HasChargeProfiles = true;
                    break;
                case ConditionsHandlerService.TABLE:
                    modMetadata.HasConditions = true;
                    break;
                case ContainerTypesHandlerService.TABLE:
                    modMetadata.HasContainerTypes = true;
                    break;
                case CreaturesHandlerService.TABLE:
                    modMetadata.HasCreatures = true;
                    break;
                case CreatureSourcesHandlerService.TABLE:
                    modMetadata.HasCreatureSources = true;
                    break;
                case DataFilesHandlerService.TABLE:
                    modMetadata.HasDataFiles = true;
                    break;
                case DmcPlacesHandlerService.TABLE:
                    modMetadata.HasDmcPlaces = true;
                    break;
                case EncountersHandlerService.TABLE:
                    modMetadata.HasEncounters = true;
                    break;
                case EncounterTriggersHandlerService.TABLE:
                    modMetadata.HasEncounterTriggers = true;
                    break;
                case FactionsHandlerService.TABLE:
                    modMetadata.HasFactions = true;
                    break;
                case ForbiddenHexesHandlerService.TABLE:
                    modMetadata.HasForbiddenHexes = true;
                    break;
                case GameVarsHandlerService.TABLE:
                    modMetadata.HasGameVars = true;
                    break;
                case HeadlinesHandlerService.TABLE:
                    modMetadata.HasHeadlines = true;
                    break;
                case HexTypesHandlerService.TABLE:
                    modMetadata.HasHexTypes = true;
                    break;
                case IngredientsHandlerService.TABLE:
                    modMetadata.HasIngredients = true;
                    break;
                case ItemPropsHandlerService.TABLE:
                    modMetadata.HasItemProps = true;
                    break;
                case ItemTypesHandlerService.TABLE:
                    modMetadata.HasItemTypes = true;
                    break;
                case MapsHandlerService.TABLE:
                    modMetadata.HasMaps = true;
                    break;
                case RecipesHandlerService.TABLE:
                    modMetadata.HasRecipes = true;
                    break;
                case TreasureTableHandlerService.TABLE:
                    modMetadata.HasTreasureTable = true;
                    break;
                case ImagesHandlerService.TABLE:
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
