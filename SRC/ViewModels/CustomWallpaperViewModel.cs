using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using WallPaperGenerator.Commands;
using WallPaperGenerator.Services;
using System.Windows;

namespace WallPaperGenerator.ViewModels
{
    public class CustomWallpaperViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IWallpaperService _wallpaperService;
        private readonly MainViewModel _mainViewModel;

        public ICommand GenerateCustomWallpaperCommand { get; private set; }
        public ICommand NavigateHomeCommand { get; private set; }

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
                    UpdateSkyBackground();
                }
            }
        }

        private bool _isGenerating;
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
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public bool HasErrors
        {
            get => _errors.Any();
            private set
            {
                if (value != _errors.Any())
                {
                    OnPropertyChanged(nameof(HasErrors));
                    CanGenerateWallpaper = !value;
                }
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }
            return null;
        }

        private void ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);

            if (propertyName == nameof(City) || propertyName == nameof(Country) || propertyName == nameof(Condition))
            {
                var value = (string)GetType().GetProperty(propertyName).GetValue(this);
                if (string.IsNullOrWhiteSpace(value))
                {
                    AddError(propertyName, $"{propertyName} is required.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z\s]+$"))
                {
                    AddError(propertyName, $"{propertyName} must contain only words.");
                }
            }
            else if (propertyName == nameof(TemperatureCelsius))
            {
                if (TemperatureCelsius < -273.15)
                {
                    AddError(propertyName, "Temperature cannot be below absolute zero (-273.15°C).");
                }
            }

            CanGenerateWallpaper = !HasErrors;
        }

        private void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            _errors[propertyName].Add(errorMessage);
            OnErrorsChanged(propertyName);
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private bool _canGenerateWallpaper;
        public bool CanGenerateWallpaper
        {
            get => _canGenerateWallpaper;
            set
            {
                if (_canGenerateWallpaper != value)
                {
                    _canGenerateWallpaper = value;
                    OnPropertyChanged(nameof(CanGenerateWallpaper));
                }
            }
        }

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


        private void UpdateSkyBackground()
        {
            string imagePath = string.Empty;

            switch (CurrentWeatherCondition.ToLower())
            {
                case "clear":
                    imagePath = "/Views/Images/ClearSky.jpg";
                    break;
                case "cloudy":
                    imagePath = "/Views/Images/CloudySky.jpg";
                    break;
                case "rainy":
                    imagePath = "/Views/Images/RainySky.jpg";
                    break;
                case "snowy":
                    imagePath = "/Views/Images/SnowySky.jpg";
                    break;
                default:
                    imagePath = "/Views/Images/DefaultSky.jpg";
                    break;
            }

            BackgroundImagePath = imagePath;
        }



        private string _city;
        public string City
        {
            get => _city;
            set
            {
                if (_city != value)
                {
                    _city = value;
                    OnPropertyChanged(nameof(City));
                    ValidateProperty(nameof(City));
                }
            }
        }
        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                if (_country != value)
                {
                    _country = value;
                    OnPropertyChanged(nameof(Country));
                    ValidateProperty(nameof(Country));
                }
            }
        }
        private string _condition;
        public string Condition
        {
            get => _condition;
            set
            {
                if (_condition != value)
                {
                    _condition = value;
                    OnPropertyChanged(nameof(Condition));
                    ValidateProperty(nameof(Condition));
                }
            }
        }
        private double _temperatureCelsius;
        public double TemperatureCelsius
        {
            get => _temperatureCelsius;
            set
            {
                if (_temperatureCelsius != value)
                {
                    _temperatureCelsius = value;
                    OnPropertyChanged(nameof(TemperatureCelsius));
                    ValidateProperty(nameof(TemperatureCelsius));
                }
            }
        }

        public CustomWallpaperViewModel(IWallpaperService wallpaperService, MainViewModel mainViewModel)
        {
            _wallpaperService = wallpaperService;
            _mainViewModel = mainViewModel;
            BackgroundImagePath = "/Views/Images/DefaultSky.jpg";

            NavigateHomeCommand = new RelayCommand(NavigateHome);
            GenerateCustomWallpaperCommand = new AsyncRelayCommand(GenerateCustomWallpaper);
        }

        private async Task GenerateCustomWallpaper()
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
                var wallpaperUrl = await _wallpaperService.GenerateWallpaperAsync(City, Country, Condition, TemperatureCelsius);
                if (string.IsNullOrEmpty(wallpaperUrl))
                {
                    Console.WriteLine("Failed to generate custom wallpaper.");
                    return;
                }

                await _wallpaperService.SetWallpaperAsync(wallpaperUrl);
                Console.WriteLine("Custom wallpaper set successfully.");

                CurrentWeatherCondition = Condition;

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
        private void NavigateHome(object parameter)
        {
            _mainViewModel.NavigateToHomeView();
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}