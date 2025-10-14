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
    public class SplashScreenViewModel : ObservableObject, ISplashScreen
    {
        private readonly ILogger<SplashScreenViewModel> _logger;
        private readonly Lazy<MainWindow> _mainWindow;
        private readonly Dispatcher _dispatcher;
        private readonly LoadingService _loadingService;

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private int _progress = 0;
        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private bool _isIndeterminate = true;
        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => SetProperty(ref _isIndeterminate, value);
        }

        public SplashScreenViewModel(
            ILogger<SplashScreenViewModel> logger,
            Lazy<MainWindow> mainWindow,
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
                GC.Collect();
                _logger.LogInformation($"{nameof(MainWindow)} launching");
                _mainWindow.Value.Show();
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

        public void SetProgress(int percentage, string message)
        {
            // run this in the UI thread so that it can be refreshed
            _dispatcher.Invoke(() =>
            {
                IsIndeterminate = false;
                Progress = percentage;
                Message = message;
            });
        }
    }

    public interface ISplashScreen
    {
        void SetProgress(int percentage, string message);
    }
}
