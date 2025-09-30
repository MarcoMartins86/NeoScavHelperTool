using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Awesome.Net.WritableOptions.Extensions;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoScavHelperTool.Models;
using NeoScavHelperTool.Models.DataTypeModels;
using NeoScavHelperTool.Services;
using NeoScavHelperTool.Services.DataTypeHandlers;
using NeoScavHelperTool.ViewModels;
using NeoScavHelperTool.Views;
using Serilog;
using Forms = System.Windows.Forms;
#if NET462
using NeoScavHelperTool.Framework;
using SQLitePCL;
#endif

namespace NeoScavHelperTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILogger<App> _logger;
        private DialogService _dialogService;

        public static new App Current => (App)Application.Current;

        public App()
        {
            // subscribe to catch all unhandled exception (show message)
            DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;

            // start a service collection
            IServiceCollection serviceCollection = new ServiceCollection();

            // setup the service collection
            IConfiguration configuration = SetupConfiguration(serviceCollection);
            SetupLogging(serviceCollection, configuration);
            SetupServices(serviceCollection);

            // build the service provider and register it in the Ioc.Default
            Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());

            // assign the internal properties from the service provider
            _logger = Ioc.Default.GetRequiredService<ILogger<App>>();
            _dialogService = Ioc.Default.GetRequiredService<DialogService>();

#if NET462
            InitSQLiteProvider();
#endif

            InitializeComponent();
        }

        private IConfiguration SetupConfiguration(IServiceCollection services)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot config = builder.Build();

            services
                .AddOptions<AppOptions>()
                .Bind(config.GetSection(AppOptions.Section))
                .ValidateDataAnnotations()
                .Validate(
                    options => File.Exists(options.NeoScavExePath),
                    "Neo Scavenger executable not found!"
                );

            services.ConfigureWritableOptions<AppOptions>(config, AppOptions.Section);

            services.AddSingleton<IConfiguration>(config);

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
            services.AddSingleton<Dispatcher>(Dispatcher);
            services.AddSingleton<LoadingService>();
            services.AddSingleton<NeoScavFolderPathResolverService>();
            services.AddSingleton<DialogService>();
            services.AddSingleton<DatabaseService>();
            services.AddSingleton<NeoScavPhpParserService>();
            services.AddSingleton<AttackModesHandlerService>();
            services.AddSingleton<BarterHexesHandlerService>();
            services.AddSingleton<BattleMovesHandlerService>();
            services.AddSingleton<CampTypesHandlerService>();
            services.AddSingleton<ChargeProfilesHandlerService>();
            services.AddSingleton<ConditionsHandlerService>();
            services.AddSingleton<ContainerTypesHandlerService>();
            services.AddSingleton<CreaturesHandlerService>();
            services.AddSingleton<CreatureSourcesHandlerService>();
            services.AddSingleton<DataFilesHandlerService>();
            services.AddSingleton<DmcPlacesHandlerService>();
            services.AddSingleton<EncountersHandlerService>();
            services.AddSingleton<EncounterTriggersHandlerService>();
            services.AddSingleton<FactionsHandlerService>();
            services.AddSingleton<ForbiddenHexesHandlerService>();
            services.AddSingleton<GameVarsHandlerService>();
            services.AddSingleton<HeadlinesHandlerService>();
            services.AddSingleton<HexTypesHandlerService>();
            services.AddSingleton<IngredientsHandlerService>();
            services.AddSingleton<ItemPropsHandlerService>();
            services.AddSingleton<ItemTypesHandlerService>();
            services.AddSingleton<MapsHandlerService>();
            services.AddSingleton<RecipesHandlerService>();
            services.AddSingleton<TreasureTableHandlerService>();
            services.AddSingleton<NeogameHandlerService>();

            // Register ViewModels
            services.AddTransient<SplashScreenViewModel>();

            // Register Views
            services.AddSingleton<SplashScreenWindow>();
            services.AddSingleton<MainWindow>();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            _logger.LogInformation("Successful exit the application");
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

#if NET462
        private void InitSQLiteProvider()
        {
            const string name = "e_sqlite3";
            SQLite3Provider_dynamic_cdecl.Setup(name, new ModuleGetFunctionPointer(name));
            raw.SetProvider(new SQLite3Provider_dynamic_cdecl());
            string nativeLibaryName = raw.GetNativeLibraryName();
            _logger.LogTrace(
                "SQLite provider manually set to use {nativeLibaryName}",
                nativeLibaryName
            );
        }
#endif
    }
}
