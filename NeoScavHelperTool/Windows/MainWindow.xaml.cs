using System;
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
using MahApps.Metro.Controls;
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
    }
}
