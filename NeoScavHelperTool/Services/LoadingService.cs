using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Attributes;
using NeoScavHelperTool.Extension;
using NeoScavHelperTool.Helper;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Services
{
    public class LoadingService
    {
        private readonly ILogger<LoadingService> _logger;
        private readonly NeoScavFolderPathResolverService _gamePathResolverService;
        private readonly DatabaseService _dbService;
        private readonly NeoScavPhpParserService _phpParserService;

        public LoadingService(
            ILogger<LoadingService> logger,
            NeoScavFolderPathResolverService folderPathResolverService,
            DatabaseService dbService,
            NeoScavPhpParserService phpParserService
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

            splashScreen.SetProgress(50, gamePath);
            Thread.Sleep(5000);
        }
    }
}
