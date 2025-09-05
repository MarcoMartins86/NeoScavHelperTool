using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Awesome.Net.WritableOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services
{
    public class NeoScavFolderPathResolverService
    {
        private readonly ILogger<NeoScavFolderPathResolverService> _logger;
        IWritableOptions<AppOptions> _appOptions;
        private readonly DialogService _dialogService;

        private string _neoScavFolderPath = string.Empty;
        public string NeoScavFolderPath
        {
            get => GetNeoScavFolderPath();
            private set => _neoScavFolderPath = value;
        }

        public NeoScavFolderPathResolverService(
            ILogger<NeoScavFolderPathResolverService> logger,
            IWritableOptions<AppOptions> appOptions,
            DialogService dialogService
        )
        {
            _logger = logger;
            _appOptions = appOptions;
            _dialogService = dialogService;
        }

        private string GetNeoScavFolderPath()
        {
            try
            {
                if (string.IsNullOrEmpty(_neoScavFolderPath))
                {
                    // Try to read executable path from appsettings.json
                    _neoScavFolderPath = Path.GetDirectoryName(_appOptions.Value.NeoScavExePath);
                }
            }
            catch (OptionsValidationException ex)
            {
                _logger.LogDebug(ex, $"Failed to get {nameof(_neoScavFolderPath)}");
                // Resolve the game folder path in other way
                _neoScavFolderPath = ResolveNeoScavFolderPath();
            }

            _logger.LogTrace(
                "Neo Scavenger game folder: \"{_neoScavFolderPath}\"",
                _neoScavFolderPath
            );

            return _neoScavFolderPath;
        }

        private string ResolveNeoScavFolderPath()
        {
            // Ask user to point to the game executable
            string neoScavExePath = _dialogService.OpenFileDialog(
                "Select your NEO Scavenger game folder",
                "NEOScavenger",
                "|NEOScavenger.exe",
                "exe",
                true
            );

            if (string.IsNullOrEmpty(neoScavExePath))
            {
                throw new Exception("Cannot continue without the game folder!");
            }

            // Refresh appsettings.json with the new value
            _appOptions.Update(opt => opt.NeoScavExePath = neoScavExePath);

            // Extract the game directory from the executable path
            string neoScavFolderPath = Path.GetDirectoryName(neoScavExePath);
            _logger.LogDebug(
                "Successfully resolved Neo Scavenger Exe directory: \"{neoScavExePath}\"",
                neoScavExePath
            );

            return neoScavFolderPath;
        }
    }
}
