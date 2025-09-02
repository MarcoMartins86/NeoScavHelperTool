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
using Forms = System.Windows.Forms;

namespace NeoScavHelperTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        private ILogger<App> _logger;
        private DialogService _dialogService;

        protected override void OnStartup(StartupEventArgs e)
        {
            // subscribe to catch all unhandled exception (show message)
            DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;

            // start a service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // setup the service collection
            IConfiguration configuration = SetupConfiguration(serviceCollection);
            SetupLogging(serviceCollection, configuration);
            SetupServices(serviceCollection);

            // build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // assign the internal properties from the service provider
            _logger = _serviceProvider.GetRequiredService<ILogger<App>>();
            _dialogService = _serviceProvider.GetRequiredService<DialogService>();

            // show the splash screen
            _serviceProvider.GetRequiredService<SplashScreenWindow>().Show();
        }

        private IConfiguration SetupConfiguration(IServiceCollection services)
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

        private void SetupLogging(IServiceCollection services, IConfiguration configuration)
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

        private void SetupServices(IServiceCollection services)
        {
            // Register Services
            services.AddSingleton<Dispatcher>(Current.Dispatcher);
            services.AddSingleton<LoadingService>();
            services.AddSingleton<NeoScavFolderPathResolverService>();
            services.AddSingleton<DialogService>();

            // Register ViewModels
            services.AddSingleton<SplashScreenViewModel>();

            // Register Views
            services.AddSingleton<SplashScreenWindow>();
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

        private void DispatcherUnhandledExceptionHandler(
            object sender,
            DispatcherUnhandledExceptionEventArgs e
        )
        {
            if (!e.Handled)
            {
                _logger.LogCritical(e.Exception, e.Exception.Message);
                _dialogService.MessageBox(
                    e.Exception.Message,
                    "Error",
                    Forms.MessageBoxButtons.OK,
                    Forms.MessageBoxIcon.Error
                );
            }
        }
    }
}
