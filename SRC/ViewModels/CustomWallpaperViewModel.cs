using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WallPaperGenerator.Commands;
using WallPaperGenerator.Services;

namespace WallPaperGenerator.ViewModels
{
    public class CustomWallpaperViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IWallpaperService _wallpaperService;
        private readonly MainViewModel _mainViewModel;

        public ICommand GenerateCustomWallpaperCommand { get; private set; }
        public ICommand NavigateHomeCommand { get; private set; }

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
                }
            }
        }

        public CustomWallpaperViewModel(IWallpaperService wallpaperService, MainViewModel mainViewModel)
        {
            _wallpaperService = wallpaperService;
            _mainViewModel = mainViewModel;

            NavigateHomeCommand = new RelayCommand(NavigateHome);
            GenerateCustomWallpaperCommand = new AsyncRelayCommand(GenerateCustomWallpaper);
        }

        private async Task GenerateCustomWallpaper()
        {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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