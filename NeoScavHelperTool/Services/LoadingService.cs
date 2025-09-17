using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Extension;
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Services.DataTypeHandler;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Services
{
    public class LoadingService
    {
        private readonly ILogger<LoadingService> _logger;
        private readonly NeoScavFolderPathResolverService _gamePathResolverService;
        private readonly DatabaseService _dbService;
        private readonly NeoScavPhpParserService _phpParserService;

        //private readonly AtackModesService _neoScavModXmlParser;

        public LoadingService(
            ILogger<LoadingService> logger,
            NeoScavFolderPathResolverService folderPathResolverService,
            DatabaseService dbService,
            NeoScavPhpParserService phpParserService,
            Dispatcher dispatcher
        )
        {
            _logger = logger;
            _gamePathResolverService = folderPathResolverService;
            _dbService = dbService;
            _phpParserService = phpParserService;
        }

        public void Start(ISplashScreen splashScreen)
        {
            // Resolve game folder path
            string gamePath = _gamePathResolverService.NeoScavFolderPath;
            // Get the list of mods info
            IList<ModInfo> mods = _phpParserService.GetModsInfo(gamePath);
            // Do a pre computation of total files we have to parse
            int totalFilesToParse = mods.Select(mod => mod.Files.Count).Sum();
            _logger.LogDebug("\"{totalFilesToParse}\" mod files to parse", totalFilesToParse);

            for (int i = 0, j = 0; i < mods.Count; i++)
            {
                ModInfo mod = mods[i];
                for (int x = 0; x < mod.Files.Count; x++, j++)
                {
                    splashScreen.SetProgress(
                        (j * 100) / totalFilesToParse,
                        $"Validating {mod.Name}_{mod.Files.ElementAt(x)}"
                    );

                    for (int y = 0; y < 100000000; y++)
                        ;

                    /*xml.Load(
                        Path.Combine(
                            mod.Folder,
                            ModInfo.NEW_MOD_TYPE_DATA_FOLDER,
                            mod.Files.ElementAt(x).ToString() + ".xml"
                        )
                    );*/

                    DataTypeBaseService.LoadDocumentWithValidation(
                        Path.Combine(
                            mod.Folder,
                            ModInfo.NEW_MOD_TYPE_DATA_FOLDER,
                            mod.Files.ElementAt(x).ToString() + ".xml"
                        )
                    );
                }
            }

            splashScreen.SetProgress(100, "Finished loading");

            Thread.Sleep(5000);
        }
    }
}
