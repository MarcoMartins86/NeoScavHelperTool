using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace NeoScavHelperTool.Models
{
    [Table("mods_metadata", WithoutRowId = true)]
    public class ModMetadata
    {
        [PrimaryKey]
        public string Name { get; set; }
        public int Order { get; set; }
        public string Folder { get; set; }
        public bool HasAttackModes { get; set; }
        public bool HasBarterHexes { get; set; }
        public bool HasBattleMoves { get; set; }
        public bool HasCampTypes { get; set; }
        public bool HasChargeProfiles { get; set; }
        public bool HasConditions { get; set; }
        public bool HasContainerTypes { get; set; }
        public bool HasCreatures { get; set; }
        public bool HasCreatureSources { get; set; }
        public bool HasDataFiles { get; set; }
        public bool HasDmcPlaces { get; set; }
        public bool HasEncounters { get; set; }
        public bool HasEncounterTriggers { get; set; }
        public bool HasFactions { get; set; }
        public bool HasForbiddenHexes { get; set; }
        public bool HasGameVars { get; set; }
        public bool HasHeadlines { get; set; }
        public bool HasHexTypes { get; set; }
        public bool HasImages { get; set; }
        public bool HasIngredients { get; set; }
        public bool HasItemProps { get; set; }
        public bool HasItemTypes { get; set; }
        public bool HasMaps { get; set; }
        public bool HasRecipes { get; set; }
        public bool HasTreasureTable { get; set; }
    }
}
