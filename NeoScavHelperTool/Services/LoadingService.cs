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
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Extension;
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Services.DataTypeHandlers;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Services
{
    public class LoadingService
    {
        private readonly ILogger<LoadingService> _logger;
        private readonly NeoScavFolderPathResolverService _folderPathResolverService;
        private readonly NeoScavPhpParserService _phpParserService;

        public LoadingService(
            ILogger<LoadingService> logger,
            NeoScavFolderPathResolverService folderPathResolverService,
            NeoScavPhpParserService phpParserService
        )
        {
            _logger = logger;
            _folderPathResolverService = folderPathResolverService;
            _phpParserService = phpParserService;
        }

        public void Start(ISplashScreen splashScreen)
        {
            // Resolve game folder path
            string gamePath = _folderPathResolverService.NeoScavFolderPath;
            // Get the list of mods info
            IList<ModInfo> mods = _phpParserService.GetModsInfo(gamePath);
            // Load the mods data into db
            LoadModsDataIntoDb(splashScreen, gamePath, mods);
            // Update progress stating it has finished
            splashScreen.SetProgress(100, "Finished loading");
        }

        private void LoadModsDataIntoDb(
            ISplashScreen splashScreen,
            string gamePath,
            IList<ModInfo> mods
        )
        {
            // Do a pre computation of total files we have to parse (helps to define progress)
            int totalFilesToParse = mods.Select(mod => mod.Files.Count).Sum();
            _logger.LogDebug("\"{totalFilesToParse}\" mod files to parse", totalFilesToParse);

            for (int i = 0, j = 0; i < mods.Count; i++)
            {
                ModInfo mod = mods[i];
                for (int x = 0; x < mod.Files.Count; x++, j++)
                {
                    DataType type = mod.Files.ElementAt(x);

                    splashScreen.SetProgress(
                        (j * 100) / totalFilesToParse,
                        $"Loading {mod.Name}_{type}"
                    );

                    if (!DataTypeHelper.TryGetAttributeFromDataType(type, out var attribute))
                    {
                        throw new Exception($"Unexpected error, unknown DataType: \"{type}\"");
                    }

                    if (
                        Ioc.Default.GetService(attribute.Handler) is IDataTypeHandlerService handler
                    )
                    {
                        handler.LoadModIntoDb(gamePath, mod, attribute);
                    }
                    else
                    {
                        throw new Exception(
                            "Unexpected error, check the Handler attribute assignement in DataType enum"
                        );
                    }
                }
            }
        }
    }
}
