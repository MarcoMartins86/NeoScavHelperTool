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
        private static int GET_MODS_PHP_DATA_OFFSET = 1;
        private static string GET_IMAGES_PHP_NAME = "getimages.php";
        private static int GET_IMAGES_PHP_DATA_OFFSET = 2;
        private static string GET_IMAGES_PHP_BIG_IMAGE_TYPE_PREFIX = "x2_";
        private static char[] LINE_SEPARATOR = { '&' };
        private static char[] KEY_VALUE_SEPARATOR = { '=' };
        private static string N_ROWS = "nRows";
        private static string N_COLS = "nCols";

        private readonly ILogger<NeoScavPhpParserService> _logger;

        public NeoScavPhpParserService(ILogger<NeoScavPhpParserService> logger)
        {
            _logger = logger;
        }

        public IList<ModInfo> GetModsInfo(string rootFolder)
        {
            // Start by parsing the getmods.php file
            string[] getModsPhpContent = ParseGetModsPhp(rootFolder);
            // Parse the number of entries and +1 to address vanilla game content
            int nEntries = ParseModsNumberEntries(getModsPhpContent) + 1;
            var mods = new List<ModInfo>(nEntries);
            // let's start by adding the vanilla game files
            mods.Add(GetNewModInfo(ModInfo.VANILLA_MOD_NAME, rootFolder));
            // TODO: add code to parse the other mods besides vanilla
            return mods;
        }

        private ModInfo GetNewModInfo(string name, string folder)
        {
            _logger.LogInformation(
                "Gathering mod \"{name}\" info from folder \"{folder}\"",
                name,
                folder
            );

            ModInfo modInfo = new ModInfo() { Name = name, Folder = folder };

            IEnumerable<string> dirFiles = Directory.EnumerateFiles(
                Path.Combine(folder, ModInfo.NEW_MOD_TYPE_DATA_FOLDER)
            );

            ISet<DataType> modFiles = new HashSet<DataType>();

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
            modInfo.Images = GetImagesInfo(folder);

            return modInfo;
        }

        private IList<ImageInfo> GetImagesInfo(string folder)
        {
            // Start by parsing the getimages.php file
            string[] getImagesPhpContent = ParseGetImagesPhp(folder);
            // Parse the number of entries and columns
            ParseImagesNumberEntriesAndColumns(
                getImagesPhpContent,
                out var nEntries,
                out var nColumns
            );
            // verify that the available data matches nEntries
            if (nEntries != Math.Max(0, getImagesPhpContent.Length - GET_IMAGES_PHP_DATA_OFFSET))
            {
                throw new Exception(
                    $"\"{GET_IMAGES_PHP_NAME}\" contains invalid \"{N_ROWS}\" value. Indicated \"{nEntries}\" but found \"{Math.Max(0, getImagesPhpContent.Length - GET_IMAGES_PHP_DATA_OFFSET)}\""
                );
            }
            // only continue if there's any entry
            if (nEntries == 0)
            {
                return Array.Empty<ImageInfo>();
            }
            // verify that the available columns is not greater than the expected one
            if (nColumns > (int)ImageType.Total)
            {
                throw new Exception(
                    $"\"{GET_IMAGES_PHP_NAME}\" contains unexpected \"{N_COLS}\" value. Indicated \"{nColumns}\" but expected at most \"{(int)ImageType.Total}\""
                );
            }

            return ParseGetImagesPhpContent(getImagesPhpContent);
        }

        private string[] ParseGetModsPhp(string folder)
        {
            _logger.LogTrace($"Starting to parse the \"{GET_MODS_PHP_NAME}\" file");
            string fileContent = File.ReadAllText(Path.Combine(folder, GET_MODS_PHP_NAME));
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

        private string[] ParseGetImagesPhp(string folder)
        {
            _logger.LogTrace($"Starting to parse the \"{GET_IMAGES_PHP_NAME}\" file");
            string fileContent = File.ReadAllText(Path.Combine(folder, GET_IMAGES_PHP_NAME));
            string[] splitGetImagesPhp = fileContent.Split(
                LINE_SEPARATOR,
                StringSplitOptions.RemoveEmptyEntries
            );

            if (splitGetImagesPhp.Length == 0)
            {
                throw new Exception($"\"{GET_IMAGES_PHP_NAME}\" cannot be empty");
            }

            return splitGetImagesPhp;
        }

        private IList<ImageInfo> ParseGetImagesPhpContent(string[] content)
        {
            var dictionary = new Dictionary<string, ImageInfo>(
                content.Length - GET_IMAGES_PHP_DATA_OFFSET
            );

            for (int i = GET_IMAGES_PHP_DATA_OFFSET; i < content.Length; i++)
            {
                KeyValuePair<string, string> image = ParseRow<string>(
                    content[i],
                    GET_IMAGES_PHP_NAME
                );

                ImageType type = image.Value.StartsWith(GET_IMAGES_PHP_BIG_IMAGE_TYPE_PREFIX)
                    ? ImageType.Big
                    : ImageType.Small;

                string name = Path.GetFileNameWithoutExtension(image.Value)
                    .TrimStart(GET_IMAGES_PHP_BIG_IMAGE_TYPE_PREFIX.ToCharArray());

                _logger.LogTrace(
                    "{type} image \"{name}\": \"{image.Value}\" found",
                    type,
                    name,
                    image.Value
                );
                if (dictionary.TryGetValue(name, out var imageInfo))
                {
                    imageInfo.Files[(int)type] = image.Value;
                }
                else
                {
                    var newImageInfo = new ImageInfo()
                    {
                        Name = name,
                        Files = new string[(int)ImageType.Total],
                    };
                    newImageInfo.Files[(int)type] = image.Value;
                    dictionary[name] = newImageInfo;
                }
            }

            return dictionary.Values.ToList();
        }

        private int ParseModsNumberEntries(string[] getModsPhpContent)
        {
            _logger.LogTrace($"Starting to parse the \"{N_ROWS}\"");
            KeyValuePair<string, int> nRows = ParseRow<int>(
                getModsPhpContent[0],
                GET_MODS_PHP_NAME
            );

            if (!N_ROWS.Equals(nRows.Key, StringComparison.InvariantCulture))
            {
                throw new Exception($"\"{GET_MODS_PHP_NAME}\" doesn't start with \"{N_ROWS}\"");
            }

            _logger.LogTrace($"\"{N_ROWS}\" contains {{nRows.Value}} entries", nRows.Value);

            return nRows.Value;
        }

        private void ParseImagesNumberEntriesAndColumns(
            string[] getImagesPhpContent,
            out int nEntries,
            out int nColumns
        )
        {
            _logger.LogTrace($"Starting to parse the \"{N_ROWS}\"");
            KeyValuePair<string, int> nRow = ParseRow<int>(
                getImagesPhpContent[0],
                GET_IMAGES_PHP_NAME
            );

            if (!N_ROWS.Equals(nRow.Key, StringComparison.InvariantCulture))
            {
                throw new Exception($"\"{GET_IMAGES_PHP_NAME}\" doesn't start with \"{N_ROWS}\"");
            }

            nEntries = nRow.Value;
            _logger.LogTrace($"\"{N_ROWS}\" contains {{nEntries}} entries", nEntries);

            if (nEntries > 0)
            {
                _logger.LogTrace($"Starting to parse the \"{N_COLS}\"");
                KeyValuePair<string, int> nCols = ParseRow<int>(
                    getImagesPhpContent[1],
                    GET_IMAGES_PHP_NAME
                );

                if (!N_COLS.Equals(nCols.Key, StringComparison.InvariantCulture))
                {
                    throw new Exception(
                        $"\"{GET_IMAGES_PHP_NAME}\" second line isn't \"{N_COLS}\""
                    );
                }

                nColumns = nCols.Value;
                _logger.LogTrace($"\"{N_COLS}\" contains {{nColumns}} entries", nColumns);
            }
            else
            {
                nColumns = 0;
            }
        }

        private KeyValuePair<string, T> ParseRow<T>(string entry, string file)
        {
            _logger.LogTrace("Parsing \"{file}\" entry: \"{entry}\"", file, entry);
            string[] splitEntry = entry.Split(
                KEY_VALUE_SEPARATOR,
                StringSplitOptions.RemoveEmptyEntries
            );
            if (splitEntry.Length != 2)
            {
                throw new Exception($"\"{file}\" contains invalid entry: \"{entry}\"");
            }
            return new KeyValuePair<string, T>(
                splitEntry[0].Trim(),
                (T)Convert.ChangeType(splitEntry[1].Trim(), typeof(T))
            );
        }
    }
}
