using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using MahApps.Metro.Controls;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Services;
using NeoScavHelperTool.ViewModels;

namespace NeoScavHelperTool.Views
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        private SplashScreenViewModel ViewModel => DataContext as SplashScreenViewModel;

        public SplashScreenWindow()
        {
            InitializeComponent();

            this.ExecuteWhenLoaded(async () => await ViewModel.StartLoadingService(this));
        }
    }
}
