using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeoScavHelperTool.Models;

namespace NeoScavHelperTool.Services
{
    public class NeoScavFolderPathResolverService
    {
        private readonly ILogger<NeoScavFolderPathResolverService> _logger;
        private readonly IOptionsMonitor<AppOptions> _optionsDelegate;
        private readonly DialogService _dialogService;

        private string _neoScavFolderPath = string.Empty;
        public string NeoScavFolderPath
        {
            get => GetNeoScavFolderPath();
            private set => _neoScavFolderPath = value;
        }

        public NeoScavFolderPathResolverService(
            ILogger<NeoScavFolderPathResolverService> logger,
            IOptionsMonitor<AppOptions> optionsDelegate,
            DialogService dialogService
        )
        {
            _logger = logger;
            _optionsDelegate = optionsDelegate;
            _dialogService = dialogService;
        }

        private string GetNeoScavFolderPath()
        {
            try
            {
                if (string.IsNullOrEmpty(_neoScavFolderPath))
                {
                    _neoScavFolderPath = Path.GetDirectoryName(
                        _optionsDelegate.CurrentValue.NeoScavExePath
                    );
                }
            }
            catch (OptionsValidationException ex)
            {
                _logger.LogDebug(ex, $"Failed to get {nameof(_neoScavFolderPath)}");
                if (!ResolveNeoScavFolderPath())
                {
                    throw new Exception("Cannot continue without the game folder!");
                }
            }

            return _neoScavFolderPath;
        }

        private bool ResolveNeoScavFolderPath()
        {
            string neoScavExePath = _dialogService.OpenFileDialog(
                "Select your NEO Scavenger game folder",
                "NEOScavenger",
                "|NEOScavenger.exe",
                "exe",
                true
            );
            if (!string.IsNullOrEmpty(neoScavExePath))
            {
                _logger.LogDebug(
                    "Successfully resolved Neo Scavenger Exe directory: \"{neoScavExePath}\"",
                    neoScavExePath
                );
                _neoScavFolderPath = Path.GetDirectoryName(neoScavExePath);
                return true;
            }
            return false;
        }
    }
}
