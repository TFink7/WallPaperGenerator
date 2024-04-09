using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WallPaperGenerator.Services;
using WallPaperGenerator.ViewModels;

namespace WallPaperGenerator
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            // Initialize ServiceCollection
            var services = new ServiceCollection();
            ConfigureServices(services);

            // Build the ServiceProvider
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddSingleton<IWallpaperService, WallpaperService>();

            services.AddSingleton<MainViewModel>();

            services.AddHttpClient();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetService<MainViewModel>()
            };
            mainWindow.Show();
        }
    }
}