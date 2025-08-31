using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Services;
using NeoScavHelperTool.ViewModels;
using NeoScavHelperTool.Views;
using Serilog;

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
            File.Delete("./last_run.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Async(a =>
                    a.File(
                        "./last_run.txt",
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                )
                .Enrich.WithThreadId()
                .CreateLogger();

            services.AddLogging(builder =>
                builder.SetMinimumLevel(LogLevel.Trace).ClearProviders().AddSerilog(dispose: true)
            );

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
