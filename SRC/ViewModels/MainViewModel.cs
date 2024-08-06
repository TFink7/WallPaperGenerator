using System.Windows.Input;
using System.ComponentModel;
using WallPaperGenerator.Commands;
using WallPaperGenerator.Services;
using System;
using WallPaperGenerator.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WallPaperGenerator.ViewModels
{
    // Central ViewModel that manages the navigation between different views
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ILocationService _locationService;
        private readonly IWeatherService _weatherService;
        private readonly IWallpaperService _wallpaperService;
        private readonly IWallpaperInfoService _wallpaperInfoService;

        public ICommand ViewPastImagesCommand { get; private set; }
        public ICommand NavigateToCustomWallpaperViewCommand { get; private set; }

        private INotifyPropertyChanged _currentViewModel;
        public INotifyPropertyChanged CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        // Injecting all dependencies for navigation
        public MainViewModel(ILocationService locationService, IWeatherService weatherService, IWallpaperService wallpaperService, IWallpaperInfoService infoService)
        {
            _locationService = locationService;
            _weatherService = weatherService;
            _wallpaperService = wallpaperService;
            _wallpaperInfoService = infoService;

            ViewPastImagesCommand = new AsyncRelayCommand(NavigateToPastWallPaperView);
            NavigateToCustomWallpaperViewCommand = new RelayCommand(obj => NavigateToCustomWallpaperView());
            CurrentViewModel = new HomeViewModel(_locationService, _weatherService, _wallpaperService);
        }

        private async Task NavigateToPastWallPaperView()
        {
            var viewModel = new PastImagesViewModel(_wallpaperInfoService, this, _wallpaperService); 
            await viewModel.LoadImagesAsync();
            CurrentViewModel = viewModel;
        }

        public void NavigateToHomeView()
        {
            CurrentViewModel = new HomeViewModel(_locationService, _weatherService, _wallpaperService);
        }

        private void NavigateToCustomWallpaperView()
        {
            CurrentViewModel = new CustomWallpaperViewModel(_wallpaperService, this);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
