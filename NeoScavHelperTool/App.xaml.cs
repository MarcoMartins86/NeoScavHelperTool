using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Models;
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
        private ILogger<App> _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;

            IServiceCollection serviceCollection = new ServiceCollection();

            IConfiguration configuration = ConfigureConfiguration(serviceCollection);
            ConfigureLogging(serviceCollection, configuration);
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = _serviceProvider.GetService<ILogger<App>>();

            _serviceProvider.GetRequiredService<SplashWindow>().Show();
        }

        private IConfiguration ConfigureConfiguration(IServiceCollection services)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            //string conn = config.GetConnectionString("Neenah_SFC_ConnectionString");

            services
                .AddOptions<AppOptions>()
                .Bind(config.GetSection(AppOptions.Section))
                .ValidateDataAnnotations()
                .Validate(
                    options => File.Exists(options.NeoScavExePath),
                    "Neoscavenger executable not found!"
                );
            //services.Configure<AppOptions>(config.GetSection(AppOptions.Section));

            services.AddSingleton(config);

            return config;
        }

        private void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            File.Delete("./last_run.txt");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    "./last_run.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .Enrich.WithThreadId()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddLogging(builder =>
                builder
                    .ClearProviders()
                    .AddSerilog(dispose: true)
                    .AddConfiguration(configuration.GetSection("Logging"))
            );
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register Services
            services.AddSingleton(Current.Dispatcher);
            services.AddSingleton<Application>(this);
            services.AddSingleton<LoadingService>();
            services.AddSingleton<UnauthorizedAccessException>();

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

        public void DispatcherUnhandledExceptionHandler(
            object sender,
            DispatcherUnhandledExceptionEventArgs e
        )
        {
            if (!e.Handled)
            {
                _logger.LogCritical(e.Exception, e.Exception.Message);
                Dispatcher.Invoke(() =>
                    MessageBox.Show(
                        e.Exception.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    )
                );
            }
        }
    }
}
