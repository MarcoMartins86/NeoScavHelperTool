using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services
{
    public class NeoScavPhpParserService
    {
        private static string GET_MODS_PHP_NAME = "getmods.php";
        private static string GET_IMAGES_PHP_NAME = "getimages.php";
        private static char[] LINE_SEPARATOR = { '&' };
        private static char[] KEY_VALUE_SEPARATOR = { '=' };
        private static string N_ROWS = "nRows";

        private readonly ILogger<NeoScavPhpParserService> _logger;

        public NeoScavPhpParserService(ILogger<NeoScavPhpParserService> logger)
        {
            _logger = logger;
        }

        public List<ModInfo> GetMods(string rootFolder)
        {
            // Start by parsing the getmods.php file
            string[] getModsPhpContent = ParseGetModsPhp(rootFolder);
            // Parse the number of entries and +1 to address vanilla game content
            int nEntries = ParseNumberEntries(getModsPhpContent) + 1;
            var mods = new List<ModInfo>(nEntries);
            // let's start by adding the vanilla game files
            mods.Add(GetMod(ModInfo.VANILLA_MOD_NAME, rootFolder));

            return null;
        }

        private ModInfo GetMod(string name, string folder)
        {
            _logger.LogInformation(
                "Gathering mod \"{name}\" info from folder \"{folder}\"",
                name,
                folder
            );

            ModInfo modInfo = new ModInfo() { Name = name, Url = folder };

            IEnumerable<string> dirFiles = Directory.EnumerateFiles(
                Path.Combine(folder, ModInfo.DATA_FOLDER)
            );

            List<DataType> modFiles = new List<DataType>(dirFiles.Count());

            foreach (string file in dirFiles)
            {
                if (DataTypeHelper.TryGetDataTypeFromFile(file, out var dataType))
                {
                    _logger.LogTrace(
                        "\"{name}\": \"{dataType}\" found",
                        name,
                        Enum.GetName(typeof(DataType), dataType)
                    );
                    modFiles.Add(dataType);
                }
                else
                {
                    _logger.LogTrace("\"{name}\": \"{file}\" ignored", name, file);
                }
            }

            modInfo.Files = modFiles;

            return modInfo;
        }

        private string[] ParseGetModsPhp(string rootFolder)
        {
            _logger.LogTrace($"Starting to parse the \"{GET_MODS_PHP_NAME}\" file");
            string fileContent = File.ReadAllText(Path.Combine(rootFolder, GET_MODS_PHP_NAME));
            string[] splitGetModsPhp = fileContent.Split(
                LINE_SEPARATOR,
                StringSplitOptions.RemoveEmptyEntries
            );

            if (splitGetModsPhp.Length == 0)
            {
                throw new Exception($"\"{GET_MODS_PHP_NAME}\" cannot be empty");
            }

            return splitGetModsPhp;
        }

        private int ParseNumberEntries(string[] getModsPhpContent)
        {
            _logger.LogTrace($"Starting to parse the \"{N_ROWS}\"");
            KeyValuePair<string, int> nRows = ParseRow<int>(getModsPhpContent[0]);

            if (!N_ROWS.Equals(nRows.Key, StringComparison.InvariantCulture))
            {
                throw new Exception($"\"{GET_MODS_PHP_NAME}\" doesn't start with \"{N_ROWS}\"");
            }

            _logger.LogTrace($"\"{N_ROWS}\" contains {{nRows.Value}} entries", nRows.Value);

            return nRows.Value;
        }

        private KeyValuePair<string, T> ParseRow<T>(string entry)
        {
            _logger.LogTrace($"Parsing \"{GET_MODS_PHP_NAME}\" entry: \"{{entry}}\"", entry);
            string[] splitEntry = entry.Split(
                KEY_VALUE_SEPARATOR,
                StringSplitOptions.RemoveEmptyEntries
            );
            if (splitEntry.Length != 2)
            {
                throw new Exception($"\"{GET_MODS_PHP_NAME}\" contains invalid entry: \"{entry}\"");
            }
            return new KeyValuePair<string, T>(
                splitEntry[0],
                (T)Convert.ChangeType(splitEntry[1], typeof(T))
            );
        }
    }
}
