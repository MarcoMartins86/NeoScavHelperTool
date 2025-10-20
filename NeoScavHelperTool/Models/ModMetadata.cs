using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Extensions;
using SQLite;

namespace NeoScavHelperTool.Models
{
    [Table("mods_metadata", WithoutRowId = true)]
    public class ModMetadata
    {
        private SortedSet<string> _types = new SortedSet<string>();
        private bool _hasAttackModes;
        private bool _hasBarterHexes;
        private bool _hasBattleMoves;
        private bool _hasCampTypes;
        private bool _hasChargeProfiles;
        private bool _hasConditions;
        private bool _hasContainerTypes;
        private bool _hasCreatures;
        private bool _hasCreatureSources;
        private bool _hasDataFiles;
        private bool _hasDmcPlaces;
        private bool _hasEncounters;
        private bool _hasEncounterTriggers;
        private bool _hasFactions;
        private bool _hasForbiddenHexes;
        private bool _hasGameVars;
        private bool _hasHeadlines;
        private bool _hasHexTypes;
        private bool _hasImages;
        private bool _hasIngredients;
        private bool _hasItemProps;
        private bool _hasItemTypes;
        private bool _hasMaps;
        private bool _hasRecipes;
        private bool _hasTreasureTable;

        [Ignore]
        public SortedSet<string> Types => _types;

        [PrimaryKey]
        public string Name { get; set; }
        public int Order { get; set; }
        public string Folder { get; set; }
        public bool HasAttackModes
        {
            get => _hasAttackModes;
            set
            {
                RefreshTypesList(value, DataType.AttackModes);
                _hasAttackModes = value;
            }
        }
        public bool HasBarterHexes
        {
            get => _hasBarterHexes;
            set
            {
                RefreshTypesList(value, DataType.BarterHexes);
                _hasBarterHexes = value;
            }
        }
        public bool HasBattleMoves
        {
            get => _hasBattleMoves;
            set
            {
                RefreshTypesList(value, DataType.BattleMoves);
                _hasBattleMoves = value;
            }
        }
        public bool HasCampTypes
        {
            get => _hasCampTypes;
            set
            {
                RefreshTypesList(value, DataType.CampTypes);
                _hasCampTypes = value;
            }
        }
        public bool HasChargeProfiles
        {
            get => _hasChargeProfiles;
            set
            {
                RefreshTypesList(value, DataType.ChargeProfiles);
                _hasChargeProfiles = value;
            }
        }
        public bool HasConditions
        {
            get => _hasConditions;
            set
            {
                RefreshTypesList(value, DataType.Conditions);
                _hasConditions = value;
            }
        }
        public bool HasContainerTypes
        {
            get => _hasContainerTypes;
            set
            {
                RefreshTypesList(value, DataType.ContainerTypes);
                _hasContainerTypes = value;
            }
        }
        public bool HasCreatures
        {
            get => _hasCreatures;
            set
            {
                RefreshTypesList(value, DataType.AttackModes);
                _hasCreatures = value;
            }
        }
        public bool HasCreatureSources
        {
            get => _hasCreatureSources;
            set
            {
                RefreshTypesList(value, DataType.CreatureSources);
                _hasCreatureSources = value;
            }
        }
        public bool HasDataFiles
        {
            get => _hasDataFiles;
            set
            {
                RefreshTypesList(value, DataType.DataFiles);
                _hasDataFiles = value;
            }
        }
        public bool HasDmcPlaces
        {
            get => _hasDmcPlaces;
            set
            {
                RefreshTypesList(value, DataType.DmcPlaces);
                _hasDmcPlaces = value;
            }
        }
        public bool HasEncounters
        {
            get => _hasEncounters;
            set
            {
                RefreshTypesList(value, DataType.Encounters);
                _hasEncounters = value;
            }
        }
        public bool HasEncounterTriggers
        {
            get => _hasEncounterTriggers;
            set
            {
                RefreshTypesList(value, DataType.EncounterTriggers);
                _hasEncounterTriggers = value;
            }
        }
        public bool HasFactions
        {
            get => _hasFactions;
            set
            {
                RefreshTypesList(value, DataType.Factions);
                _hasFactions = value;
            }
        }
        public bool HasForbiddenHexes
        {
            get => _hasForbiddenHexes;
            set
            {
                RefreshTypesList(value, DataType.ForbiddenHexes);
                _hasForbiddenHexes = value;
            }
        }
        public bool HasGameVars
        {
            get => _hasGameVars;
            set
            {
                RefreshTypesList(value, DataType.GameVars);
                _hasGameVars = value;
            }
        }
        public bool HasHeadlines
        {
            get => _hasHeadlines;
            set
            {
                RefreshTypesList(value, DataType.Headlines);
                _hasHeadlines = value;
            }
        }
        public bool HasHexTypes
        {
            get => _hasHexTypes;
            set
            {
                RefreshTypesList(value, DataType.HexTypes);
                _hasHexTypes = value;
            }
        }
        public bool HasImages
        {
            get => _hasImages;
            set
            {
                RefreshTypesList(value, DataType.Images);
                _hasImages = value;
            }
        }
        public bool HasIngredients
        {
            get => _hasIngredients;
            set
            {
                RefreshTypesList(value, DataType.Ingredients);
                _hasIngredients = value;
            }
        }
        public bool HasItemProps
        {
            get => _hasItemProps;
            set
            {
                RefreshTypesList(value, DataType.ItemProps);
                _hasItemProps = value;
            }
        }
        public bool HasItemTypes
        {
            get => _hasItemTypes;
            set
            {
                RefreshTypesList(value, DataType.ItemTypes);
                _hasItemTypes = value;
            }
        }
        public bool HasMaps
        {
            get => _hasMaps;
            set
            {
                RefreshTypesList(value, DataType.Maps);
                _hasMaps = value;
            }
        }
        public bool HasRecipes
        {
            get => _hasRecipes;
            set
            {
                RefreshTypesList(value, DataType.Recipes);
                _hasRecipes = value;
            }
        }
        public bool HasTreasureTable
        {
            get => _hasTreasureTable;
            set
            {
                RefreshTypesList(value, DataType.TreasureTable);
                _hasTreasureTable = value;
            }
        }

        private void RefreshTypesList(bool hasType, DataType dataType)
        {
            string table = dataType.GetTable();
            if (hasType)
                _types.Add(table);
            else
                _types.Remove(table);
        }
    }
}
