using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WallPaperGenerator.Data;
using WallPaperGenerator.Services;
using WallPaperGenerator.ViewModels;

namespace WallPaperGenerator
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private MainViewModel _mainViewModel;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=wallpaperGenerator.db");
            });


            services.AddHttpClient();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddSingleton<IWallpaperService, WallpaperService>();
            services.AddSingleton<IWallpaperInfoService, WallpaperInfoService>();

            services.AddSingleton<MainViewModel>();
            services.AddTransient<PastImagesViewModel>();
            services.AddTransient<MainWindow>();

            services.AddTransient<CustomWallpaperViewModel>(provider =>
    new CustomWallpaperViewModel(provider.GetRequiredService<IWallpaperService>(), provider.GetRequiredService<MainViewModel>()));

            services.AddTransient<PastImagesViewModel>(provider =>
    new PastImagesViewModel(provider.GetRequiredService<IWallpaperInfoService>(), provider.GetRequiredService<MainViewModel>()));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();

                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                _mainViewModel = scope.ServiceProvider.GetRequiredService<MainViewModel>();

                mainWindow.DataContext = _mainViewModel;

                mainWindow.Show();
            }
        }
        public MainViewModel MainViewModel => _mainViewModel;
    }
}