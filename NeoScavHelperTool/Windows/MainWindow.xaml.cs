using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using CommunityToolkit.Mvvm.DependencyInjection;
using MahApps.Metro.Controls;
using NeoScavHelperTool.Helpers;
using NeoScavHelperTool.Models.DataTypeModels.Base;
using NeoScavHelperTool.Services.DataTypeHandlers.Base;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private HamburgerPaneViewModel ViewModel => DataContext as HamburgerPaneViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExpanderElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Defer the measurement to ensure layout has completed after expansion
            Dispatcher.InvokeAsync(
                () =>
                {
                    Expander expander = sender as Expander;
                    switch (expander.Name)
                    {
                        case "FirstExpander":
                            ViewModel.FirstExpanderMaxWidth = expander.ActualWidth;
                            break;
                        case "SecondExpander":
                            ViewModel.SecondExpanderMaxWidth = expander.ActualWidth;
                            break;
                        default:
                            throw new NotImplementedException(
                                $"Unexpected expander: \"{expander.Name}\""
                            );
                    }
                },
                DispatcherPriority.Render
            ); // Use Render priority to ensure layout is done
        }

        private void Expanders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                ViewModel.HamburgerItems = new List<DataTypeModelBase>();
                return;
            }

            // Ignore, already listing this content
            if (e.AddedItems.Contains(ViewModel.HamburgerItemsContentName))
            {
                return;
            }

            (string mod, string table) = ViewModel.GetSelectedModAndTable();

            IRepositoryDataTypeHandlerService repository =
                DataTypeHelper.TryGetHandlerFromTable<IRepositoryDataTypeHandlerService>(table);

            ViewModel.HamburgerItems = repository.FindAll(mod);
        }
    }
}
