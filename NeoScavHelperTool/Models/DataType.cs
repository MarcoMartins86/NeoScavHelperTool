using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Attributes;

namespace NeoScavHelperTool.Models
{
    public enum DataType
    {
        [DataType(File = "attackmodes.xml")]
        AtackModes,

        [DataType(File = "barterhexes.xml")]
        BarterHexes,

        [DataType(File = "battlemoves.xml")]
        BattleMoves,

        [DataType(File = "camptypes.xml")]
        CampTypes,

        [DataType(File = "chargeprofiles.xml")]
        ChargeProfiles,

        [DataType(File = "conditions.xml")]
        Conditions,

        [DataType(File = "containertypes.xml")]
        ContainerTypes,

        [DataType(File = "creatures.xml")]
        Creatures,

        [DataType(File = "creaturesources.xml")]
        CreaturesSources,

        [DataType(File = "datafiles.xml")]
        DataFiles,

        [DataType(File = "dmcplaces.xml")]
        DmcPlaces,

        [DataType(File = "encounters.xml")]
        Encounters,

        [DataType(File = "encountertriggers.xml")]
        EncounterTriggers,

        [DataType(File = "factions.xml")]
        Factions,

        [DataType(File = "forbiddenhexes.xml")]
        ForbiddenHexes,

        [DataType(File = "gamevars.xml")]
        GameVars,

        [DataType(File = "headlines.xml")]
        Headlines,

        [DataType(File = "hextypes.xml")]
        HexTypes,

        [DataType(File = "ingredients.xml")]
        Ingredients,

        [DataType(File = "itemprops.xml")]
        ItemProps,

        [DataType(File = "itemtypes.xml")]
        ItemTypes,

        [DataType(File = "maps.xml")]
        Maps,

        [DataType(File = "recipes.xml")]
        Recipes,

        [DataType(File = "treasuretable.xml")]
        TreasureTable,
    }
}
