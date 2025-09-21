using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Services.DataTypeHandlers;

namespace NeoScavHelperTool.Models
{
    public enum DataType
    {
        [DataType(
            File = "attackmodes.xml",
            Type = ModType.New,
            Table = AttackModesHandlerService.TABLE,
            Handler = typeof(AttackModesHandlerService)
        )]
        AttackModes,

        [DataType(File = "barterhexes.xml", Type = ModType.New)]
        BarterHexes,

        [DataType(File = "battlemoves.xml", Type = ModType.New)]
        BattleMoves,

        [DataType(File = "camptypes.xml", Type = ModType.New)]
        CampTypes,

        [DataType(File = "chargeprofiles.xml", Type = ModType.New)]
        ChargeProfiles,

        [DataType(File = "conditions.xml", Type = ModType.New)]
        Conditions,

        [DataType(File = "containertypes.xml", Type = ModType.New)]
        ContainerTypes,

        [DataType(File = "creatures.xml", Type = ModType.New)]
        Creatures,

        [DataType(File = "creaturesources.xml", Type = ModType.New)]
        CreaturesSources,

        [DataType(File = "datafiles.xml", Type = ModType.New)]
        DataFiles,

        [DataType(File = "dmcplaces.xml", Type = ModType.New)]
        DmcPlaces,

        [DataType(File = "encounters.xml", Type = ModType.New)]
        Encounters,

        [DataType(File = "encountertriggers.xml", Type = ModType.New)]
        EncounterTriggers,

        [DataType(File = "factions.xml", Type = ModType.New)]
        Factions,

        [DataType(File = "forbiddenhexes.xml", Type = ModType.New)]
        ForbiddenHexes,

        [DataType(File = "gamevars.xml", Type = ModType.New)]
        GameVars,

        [DataType(File = "headlines.xml", Type = ModType.New)]
        Headlines,

        [DataType(File = "hextypes.xml", Type = ModType.New)]
        HexTypes,

        [DataType(File = "ingredients.xml", Type = ModType.New)]
        Ingredients,

        [DataType(File = "itemprops.xml", Type = ModType.New)]
        ItemProps,

        [DataType(File = "itemtypes.xml", Type = ModType.New)]
        ItemTypes,

        [DataType(File = "maps.xml", Type = ModType.New)]
        Maps,

        [DataType(File = "recipes.xml", Type = ModType.New)]
        Recipes,

        [DataType(File = "treasuretable.xml", Type = ModType.New)]
        TreasureTable,

        [DataType(File = "neogame.xml", Type = ModType.Old)]
        Neogame,
    }
}
