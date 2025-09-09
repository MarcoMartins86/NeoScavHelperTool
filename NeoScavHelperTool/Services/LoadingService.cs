using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Services
{
    public class LoadingService
    {
        private readonly ILogger<LoadingService> _logger;
        private readonly NeoScavFolderPathResolverService _gamePathResolverService;
        private readonly DatabaseService _dbService;

        public LoadingService(
            ILogger<LoadingService> logger,
            NeoScavFolderPathResolverService folderPathResolverService,
            DatabaseService dbService
        )
        {
            _logger = logger;
            _gamePathResolverService = folderPathResolverService;
            _dbService = dbService;
        }

        public void Start(SplashScreenViewModel splashScreenViewModel)
        {
            // Resolve game folder path
            string gamePath = _gamePathResolverService.NeoScavFolderPath;

            _dbService.Query<int>();

            splashScreenViewModel.Message = gamePath;
            Thread.Sleep(5000);
        }
    }
}
