using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using NeoScavHelperTool.Services;
using NeoScavHelperTool.Services.DataTypeHandlers.Base;

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
                switch (value)
                {
                    case (int)HamburgerDisplayMode.ByMod:
                        FirstExpanderHeaderChooseText = "Choose a mod";
                        SecondExpanderHeaderChooseText = "Choose a type";
                        break;
                    case (int)HamburgerDisplayMode.ByType:
                        FirstExpanderHeaderChooseText = "Choose a type";
                        SecondExpanderHeaderChooseText = "Choose a mod";
                        break;
                    case (int)HamburgerDisplayMode.Consolidated:
                        break;
                }
                _firstExpanderMaxWidth = 0;
                _secondExpanderMaxWidth = 0;
                IsFirstExpanderExtended = true;
                IsSecondExpanderExtended = false;
                ChosenFirstExpanderIndex = -1;
                ChosenSecondExpanderIndex = -1;
                HamburgerItems = new List<DataTypeModelBase>();
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

        public string _secondExpanderHeaderChooseText = "Choose a type";
        public string SecondExpanderHeaderChooseText
        {
            get => _secondExpanderHeaderChooseText;
            set => SetProperty(ref _secondExpanderHeaderChooseText, value);
        }

        public string _secondExpanderHeaderShowText = string.Empty;
        public string SecondExpanderHeaderShowText
        {
            get => _secondExpanderHeaderShowText;
            set => SetProperty(ref _secondExpanderHeaderShowText, value);
        }

        public ICollection<string> Mods => _modsMetadata.Mods;

        public int _chosenFirstExpanderIndex = -1;
        public int ChosenFirstExpanderIndex
        {
            get => _chosenFirstExpanderIndex;
            set
            {
                if (value == -1)
                {
                    SelectedModTypes = null;
                    IsFirstExpanderExtended = true;
                }
                else
                {
                    switch (_displayMode)
                    {
                        case HamburgerDisplayMode.ByMod:
                            string modName = Mods.ElementAt(value);
                            FirstExpanderHeaderShowText = $"\"{modName}\" types";
                            SelectedModTypes = _modsMetadata.GetModTypes(modName);
                            break;
                        case HamburgerDisplayMode.ByType:
                            break;
                        case HamburgerDisplayMode.Consolidated:
                            // TODO
                            break;
                    }
                    ChosenSecondExpanderIndex = -1;
                    IsFirstExpanderExtended = false;
                    IsSecondExpanderExtended = true;
                    HamburgerItems = new List<DataTypeModelBase>();
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
                if (value == -1)
                {
                    IsSecondExpanderExtended = true;
                }
                else
                {
                    switch (_displayMode)
                    {
                        case HamburgerDisplayMode.ByMod:
                            string typeName = SelectedModTypes.ElementAt(value);
                            SecondExpanderHeaderShowText = $"\"{typeName}\" data";
                            break;
                        case HamburgerDisplayMode.ByType:
                            break;
                        case HamburgerDisplayMode.Consolidated:
                            // TODO
                            break;
                    }
                    IsFirstExpanderExtended = false;
                    IsSecondExpanderExtended = false;
                }
                SetProperty(ref _chosenSecondExpanderIndex, value);
            }
        }

        private double _firstExpanderMaxWidth = 0;
        public double FirstExpanderMaxWidth
        {
            get => _firstExpanderMaxWidth;
            set => _firstExpanderMaxWidth = Math.Max(_firstExpanderMaxWidth, value);
        }

        private double _secondExpanderMaxWidth = 0;
        public double SecondExpanderMaxWidth
        {
            get => _secondExpanderMaxWidth;
            set => _secondExpanderMaxWidth = Math.Max(_secondExpanderMaxWidth, value);
        }

        private List<DataTypeModelBase> _hamburgerItems = new List<DataTypeModelBase>();
        public List<DataTypeModelBase> HamburgerItems
        {
            get => _hamburgerItems;
            set
            {
                if (value != null && value.Count > 0)
                {
                    switch (_displayMode)
                    {
                        case HamburgerDisplayMode.ByMod:
                            HamburgerItemsContentName = SelectedModTypes.ElementAt(
                                ChosenSecondExpanderIndex
                            );
                            break;
                        case HamburgerDisplayMode.ByType:
                            break;
                        case HamburgerDisplayMode.Consolidated:
                            break;
                    }
                }
                else
                {
                    HamburgerItemsContentName = string.Empty;
                }

                SetProperty(ref _hamburgerItems, value);
            }
        }

        public string HamburgerItemsContentName { get; private set; } = string.Empty;

        public HamburgerPaneViewModel(
            ILogger<HamburgerPaneViewModel> logger,
            ModsMetadataService modsMetadata
        )
        {
            _logger = logger;
            _modsMetadata = modsMetadata;
        }

        public (string mod, string table) GetSelectedModAndTable()
        {
            switch (_displayMode)
            {
                case HamburgerDisplayMode.ByMod:
                    return (
                        Mods.ElementAt(ChosenFirstExpanderIndex),
                        SelectedModTypes.ElementAt(ChosenSecondExpanderIndex)
                    );
                case HamburgerDisplayMode.ByType:
                    return (string.Empty, string.Empty);
                case HamburgerDisplayMode.Consolidated:
                    return (string.Empty, string.Empty);
            }
            throw new NotImplementedException();
        }
    }

    public enum HamburgerDisplayMode
    {
        ByMod,
        ByType,
        Consolidated,
    }
}
