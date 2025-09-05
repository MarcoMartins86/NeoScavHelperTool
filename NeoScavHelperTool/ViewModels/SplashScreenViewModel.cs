using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Services;
using NeoScavHelperTool.Views;

namespace NeoScavHelperTool.ViewModels
{
    public class SplashScreenViewModel : ObservableObject
    {
        private readonly ILogger<SplashScreenViewModel> _logger;
        private readonly MainWindow _mainWindow;
        private readonly Dispatcher _dispatcher;
        private readonly LoadingService _loadingService;

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public SplashScreenViewModel(
            ILogger<SplashScreenViewModel> logger,
            MainWindow mainWindow,
            Dispatcher dispatcher,
            LoadingService loadingService
        )
        {
            _logger = logger;
            _mainWindow = mainWindow;
            _dispatcher = dispatcher;
            _loadingService = loadingService;
        }

        public async Task StartLoadingService(Window window)
        {
            window.Closing += (sender, e) =>
            {
                _logger.LogInformation($"{nameof(MainWindow)} launching");
                _mainWindow.Show();
            };

            await Task.Run(() =>
                {
                    Debug.Assert(
                        Thread.CurrentThread != _dispatcher.Thread,
                        "Loading work must not run in the UI thread!"
                    );

                    _logger.LogInformation($"{nameof(LoadingService)} starting");
                    _loadingService.Start(this);
                    _logger.LogInformation($"{nameof(LoadingService)} finished");
                })
                .ConfigureAwait(false);

            _logger.LogInformation($"{nameof(SplashScreenWindow)} closing");
            _dispatcher.Invoke(window.Close);
        }
    }
}
