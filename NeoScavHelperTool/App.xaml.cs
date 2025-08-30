using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NeoScavHelperTool.Services;
using NeoScavHelperTool.ViewModels;
using NeoScavHelperTool.Views;

namespace NeoScavHelperTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _serviceProvider.GetRequiredService<SplashWindow>().Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Configure Logging
            services.AddLogging();

            // Register Services
            services.AddSingleton(Current.Dispatcher);
            services.AddSingleton<LoadingService>();

            // Register ViewModels
            services.AddSingleton<SplashViewModel>();

            // Register Views
            services.AddSingleton<SplashWindow>();
            services.AddSingleton<MainWindow>();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            // Dispose of services if needed
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
