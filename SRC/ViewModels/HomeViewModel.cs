using System.Windows.Input;
using System.ComponentModel;
using WallPaperGenerator.Commands;
using WallPaperGenerator.Services;
using System;
using WallPaperGenerator.Models;
using System.Threading.Tasks;

namespace WallPaperGenerator.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand GenerateWallpaperCommand { get; private set; }
        public ICommand CustomCommand { get; private set; }

        private readonly ILocationService _locationService;
        private readonly IWeatherService _weatherService;
        private readonly IWallpaperService _wallpaperService;

        public HomeViewModel(ILocationService locationService, IWeatherService weatherService, IWallpaperService wallpaperService)
        {
            _locationService = locationService;
            _weatherService = weatherService;
            _wallpaperService = wallpaperService;
            GenerateWallpaperCommand = new RelayCommand(GenerateWallpaper);
            CustomCommand = new RelayCommand(CustomOperation);
        }

        private async void GenerateWallpaper(object parameter)
        {
            try
            {
                LocationData locationData = await _locationService.GetCurrentLocationAsync();
                if (string.IsNullOrWhiteSpace(locationData.City) || string.IsNullOrEmpty(locationData.Country) || locationData.Latitude == null || locationData.Longitude == null)
                {
                    Console.WriteLine("Location data incomplete.");
                    return;
                }

                var weatherData = await _weatherService.GetWeatherAsync(locationData);
                if (weatherData == null)
                {
                    Console.WriteLine("Failed to get weather data.");
                    return;
                }

                var wallpaperUrl = await _wallpaperService.GenerateWallpaperAsync(locationData.City, locationData.Country, weatherData.Condition, weatherData.TemperatureCelsius);
                if (string.IsNullOrEmpty(wallpaperUrl))
                {
                    Console.WriteLine("Failed to generate wallpaper.");
                    return;
                }

                await _wallpaperService.SetWallpaperAsync(wallpaperUrl);
                Console.WriteLine("Wallpaper set successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void CustomOperation(object parameter)
        {
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
