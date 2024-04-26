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

        private readonly ILocationService _locationService;
        private readonly IWeatherService _weatherService;
        private readonly IWallpaperService _wallpaperService;
        private bool _isGenerating;
        private string _backgroundImagePath;
        public string BackgroundImagePath
        {
            get => _backgroundImagePath;
            set
            {
                if (_backgroundImagePath != value)
                {
                    _backgroundImagePath = value;
                    OnPropertyChanged(nameof(BackgroundImagePath));
                }
            }
        }

        private string _currentWeatherCondition;
        public string CurrentWeatherCondition
        {
            get => _currentWeatherCondition;
            set
            {
                if (_currentWeatherCondition != value)
                {
                    _currentWeatherCondition = value;
                    OnPropertyChanged(nameof(CurrentWeatherCondition));
                    UpdateBackgroundImage();
                }
            }
        }
        private void UpdateBackgroundImage()
        {
            string imagePath = string.Empty;

            switch (CurrentWeatherCondition.ToLower())
            {
                case "clear":
                    imagePath = "pack://application:,,,/Views/Images/ClearSky.jpg";
                    break;
                case "cloudy":
                    imagePath = "pack://application:,,,/Views/Images/CloudySky.jpg";
                    break;
                case "rainy":
                    imagePath = "pack://application:,,,/Views/Images/RainySky.jpg";
                    break;
                case "snowy":
                    imagePath = "pack://application:,,,/Views/Images/SnowySky.jpg";
                    break;
                default:
                    imagePath = "pack://application:,,,/Views/Images/DefaultSky.jpg";
                    break;
            }

            BackgroundImagePath = imagePath;
        }

        public bool IsGenerating
        {
            get => _isGenerating;
            set
            {
                _isGenerating = value;
                OnPropertyChanged(nameof(IsGenerating));
            }
        }

        private double _progressValue;
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public HomeViewModel(ILocationService locationService, IWeatherService weatherService, IWallpaperService wallpaperService)
        {
            _locationService = locationService;
            _weatherService = weatherService;
            _wallpaperService = wallpaperService;
            GenerateWallpaperCommand = new AsyncRelayCommand(GenerateWallpaper);
            BackgroundImagePath = "/Views/Images/DefaultSky.jpg";
        }

        private async Task GenerateWallpaper()
        {
            IsGenerating = true;
            ProgressValue = 0;

            await Task.Run(async () =>
            {
                while (ProgressValue < 90)
                {
                    await Task.Delay(100);
                    ProgressValue += 1;
                }
            });

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

                CurrentWeatherCondition = weatherData.Condition;
                await _wallpaperService.SetWallpaperAsync(wallpaperUrl);
                Console.WriteLine("Wallpaper set successfully.");

                await Task.Run(async () =>
                {
                    while (ProgressValue < 100)
                    {
                        await Task.Delay(50);
                        ProgressValue += 1;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsGenerating = false;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
