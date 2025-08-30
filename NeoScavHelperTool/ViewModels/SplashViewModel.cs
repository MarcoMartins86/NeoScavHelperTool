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
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Services;
using NeoScavHelperTool.Views;

namespace NeoScavHelperTool.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        private readonly ILogger<SplashViewModel> _logger;
        private readonly MainWindow _mainWindow;
        private readonly Dispatcher _dispatcher;
        private readonly LoadingService _loadingService;

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set
            {
                if (!Equals(_message, value))
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        public SplashViewModel(
            ILogger<SplashViewModel> logger,
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
                _logger.LogTrace("SplashWindow is closing, launching MainWindow");
                _mainWindow.Show();
            };

            await Task.Run(() =>
                {
                    Debug.Assert(
                        Thread.CurrentThread != _dispatcher.Thread,
                        "Loading work must not run in the UI thread!"
                    );

                    _logger.LogTrace("Asking LoadingService to start");

                    _loadingService.Start(this);
                })
                .ConfigureAwait(false);

            _logger.LogTrace("LoadingService finished, asking SplashWindow to close");
            _dispatcher.Invoke(window.Close);
        }
    }
}
