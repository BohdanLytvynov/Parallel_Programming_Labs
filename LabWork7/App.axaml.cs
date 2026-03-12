using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LabWork7.BusinessLayer.PICalculators;
using LabWork7.DialogManagers;
using LabWork7.DialogManagers.Base;
using LabWork7.Settings;
using LabWork7.ViewModels;
using LabWork7.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace LabWork7
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                ServiceCollection services = new ServiceCollection();
                ConfigureServices(services);
                ServiceProvider = services.BuildServiceProvider();

                AppSettings appSettings = ServiceProvider.GetRequiredService<AppSettings>();

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(appSettings.lang);

                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                var mainWindow = ServiceProvider.GetService<MainWindow>();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<MainWindow>(c =>
            { 
                var vm = c.GetRequiredService<MainWindowViewModel>();
                MainWindow main = new MainWindow();
                main.DataContext = vm;

                return main;
            });

            services.AddSingleton<IAsyncPICalculator, AsyncPIParallelCalculator>();
            services.AddSingleton<IDialogManager, DialogManager>();
            services.AddTransient<MessageBox>();
            services.AddTransient<ProgressMessageBox>();

            services.AddSingleton<AppSettings>(c =>
            {
                string basePath = Directory.GetCurrentDirectory();
                string jsonConfig = basePath + Path.DirectorySeparatorChar + "appsettings.json";

                if (!File.Exists(jsonConfig))
                {
                    string content = "{\r\n\t\"global\":\r\n\t{\r\n\t\t\"lang\":\"English\"\r\n\t}\r\n}";
                    File.Create(jsonConfig);
                    File.WriteAllText(jsonConfig, content);
                }

                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration configuration = builder.Build();

                AppSettings appSettings = new AppSettings();
                configuration.GetSection("global").Bind(appSettings);

                return appSettings;
            });
        }
    }
}