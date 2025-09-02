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

        public LoadingService(
            ILogger<LoadingService> logger,
            NeoScavFolderPathResolverService folderPathResolverService
        )
        {
            _logger = logger;
            _gamePathResolverService = folderPathResolverService;
        }

        public void Start(SplashScreenViewModel splashScreenViewModel)
        {
            string gamePath = _gamePathResolverService.NeoScavFolderPath;

            Thread.Sleep(5000);
        }
    }
}
