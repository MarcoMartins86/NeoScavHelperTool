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
            set
            {
                switch ((HamburgerDisplayMode)value)
                {
                    case HamburgerDisplayMode.ByMod:
                        FirstExpanderHeaderChooseText = "Choose a mod";
                        break;
                    case HamburgerDisplayMode.ByType:
                        break;
                    case HamburgerDisplayMode.Consolidated:
                        break;
                }
                IsFirstExpanderExtended = true;
                IsSecondExpanderExtended = false;
                ChosenFirstExpanderIndex = -1;
                SetProperty(ref _displayMode, (HamburgerDisplayMode)value);
            }
        }

        public bool _isFirstExpanderExtended = true;
        public bool IsFirstExpanderExtended
        {
            get => _isFirstExpanderExtended;
            set => SetProperty(ref _isFirstExpanderExtended, value);
        }

        public string _firstExpanderHeaderChooseText = "Choose a mod";
        public string FirstExpanderHeaderChooseText
        {
            get => _firstExpanderHeaderChooseText;
            set => SetProperty(ref _firstExpanderHeaderChooseText, value);
        }

        public string _firstExpanderHeaderShowText = string.Empty;
        public string FirstExpanderHeaderShowText
        {
            get => _firstExpanderHeaderShowText;
            set => SetProperty(ref _firstExpanderHeaderShowText, value);
        }

        public bool _isSecondExpanderExtended = false;
        public bool IsSecondExpanderExtended
        {
            get => _isSecondExpanderExtended;
            set => SetProperty(ref _isSecondExpanderExtended, value);
        }

        public ICollection<string> Mods => _modsMetadata.Mods;

        public int _chosenFirstExpanderIndex = -1;
        public int ChosenFirstExpanderIndex
        {
            get => _chosenFirstExpanderIndex;
            set
            {
                switch (_displayMode)
                {
                    case HamburgerDisplayMode.ByMod:
                        if (value != -1)
                        {
                            string modName = Mods.ElementAt(value);
                            FirstExpanderHeaderShowText = $"\"{modName}\" types";
                            SelectedModTypes = _modsMetadata.GetModTypes(modName);
                        }
                        else
                        {
                            SelectedModTypes = null;
                        }
                        IsFirstExpanderExtended = false;
                        IsSecondExpanderExtended = true;
                        break;
                    case HamburgerDisplayMode.ByType:
                        IsFirstExpanderExtended = true;
                        IsSecondExpanderExtended = false;
                        break;
                    case HamburgerDisplayMode.Consolidated:
                        // TODO
                        break;
                }

                SetProperty(ref _chosenFirstExpanderIndex, value);
            }
        }

        private ICollection<string> _selectedModTypes;
        public ICollection<string> SelectedModTypes
        {
            get => _selectedModTypes;
            set => SetProperty(ref _selectedModTypes, value);
        }

        public int _chosenSecondExpanderIndex = -1;
        public int ChosenSecondExpanderIndex
        {
            get => _chosenSecondExpanderIndex;
            set
            {
                /*
                switch (_displayMode)
                {
                    case HamburgerDisplayMode.ByMod:
                        if (value != -1)
                        {
                            FirstExpanderHeaderShowText = $"\"{Mods[value]}\" types";
                        }
                        IsFirstExpanderExtended = false;
                        IsSecondExpanderExtended = true;
                        break;
                    case HamburgerDisplayMode.ByType:
                        IsFirstExpanderExtended = true;
                        IsSecondExpanderExtended = false;
                        break;
                    case HamburgerDisplayMode.Consolidated:
                        // TODO
                        break;
                }*/

                SetProperty(ref _chosenSecondExpanderIndex, value);
            }
        }

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
