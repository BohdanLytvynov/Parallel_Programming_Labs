using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LabWork5.MatrixCalculators;
using LabWork5.RandomMatrixFillers;
using LabWork5_AvaloniaUI.Managers;
using LabWork5_AvaloniaUI.ViewModels;
using LabWork5_AvaloniaUI.ViewModels.Dialogs;
using LabWork5_AvaloniaUI.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace LabWork5_AvaloniaUI
{
    //Here I try to implement the pattern Entry Point
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }//Service Container

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                Services = serviceCollection.BuildServiceProvider();

                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = Services.GetRequiredService<MainWindow>();
            }

            base.OnFrameworkInitializationCompleted();
        }
        /// <summary>
        /// Main service container configuration 
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureServices(IServiceCollection services)
        {
            //We use it for Windows and its View models, after closing of it - resources will be cleared
            services.AddTransient<MainWindowViewModel>();

            services.AddTransient<MainWindow>(c =>
            {
                var vm = c.GetRequiredService<MainWindowViewModel>();
                var v = new MainWindow() { DataContext = vm };
                v.Closed += (o, e) => 
                {
                    (vm as IDisposable)?.Dispose(); 
                };
                return v;
            });

            services.AddSingleton<IWindowManager, WindowManager>();

            services.AddTransient<DialogViewModel>();
            services.AddTransient<MatrixCalcDialog>(c =>
            {
                var vm = c.GetRequiredService<DialogViewModel>();
                var v = new MatrixCalcDialog() { DataContext = vm };
                vm.OnOkPressed += (o, e) =>
                {
                    v.Close();
                };
                v.Closing += (o, e) =>
                {
                    vm.OnOkPressed -= (o, e) => { };
                    (vm as IDisposable)?.Dispose();
                };

                return v;
            });

            services.AddTransient<ProgressDialogViewModel>();
            services.AddTransient<ProgressDialog>(c =>
            {
                var vm = c.GetRequiredService<ProgressDialogViewModel>();
                var v = new ProgressDialog() { DataContext = vm };
                vm.ProgressCanceled += (o, e) =>
                {
                    v.Close();
                };
                v.Closed += (o, e) =>
                {
                    vm.ProgressCanceled -= (o, e) => { };
                    (vm as IDisposable)?.Dispose();
                };
                return v;
            });

            services.AddTransient<ResultViewModel>();
            services.AddTransient<ResultWindow>(c =>
            {
                var vm = c.GetRequiredService<ResultViewModel>();
                var v = new ResultWindow() { DataContext = vm };
                vm.OnOkPressed += (o, e) =>
                {
                    v.Close();
                };
                v.Closed += (o, e) =>
                {
                    vm.OnOkPressed -= (o, e) => { };
                    (vm as IDisposable)?.Dispose();
                };
                return v;
            });
            //Here we add new services that must exist during all the app lifetime
            services.AddSingleton<IRandomMatrixFiller, RandomMatrixFiller>();
            services.AddSingleton<IMatrixCalculator, MatrixCalculator>();
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
    }
}