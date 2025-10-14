using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Services;

namespace NeoScavHelperTool.ViewModels
{
    public class HamburgerPaneViewModel : ObservableObject
    {
        private readonly ILogger<HamburgerPaneViewModel> _logger;
        private readonly ModsMetadataService _modsMetadata;

        private HamburgerDisplayMode _displayMode = 0;
        public int DisplayMode
        {
            get => (int)_displayMode;
            set => SetProperty(ref _displayMode, (HamburgerDisplayMode)value);
        }

        public List<string> Mods => _modsMetadata.Mods;

        public HamburgerPaneViewModel(
            ILogger<HamburgerPaneViewModel> logger,
            ModsMetadataService modsMetadata
        )
        {
            _logger = logger;
            _modsMetadata = modsMetadata;
        }
    }

    enum HamburgerDisplayMode
    {
        ByMod,
        ByType,
        Consolidated,
    }
}
