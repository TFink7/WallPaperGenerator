using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WallPaperGenerator.Commands;
using WallPaperGenerator.Models;
using WallPaperGenerator.Services;

namespace WallPaperGenerator.ViewModels
{
    // ViewModel to display and possibly set previous wallpapers
    public class PastImagesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WallpaperInfo> Images { get; private set; }
        private readonly IWallpaperInfoService _wallpaperInfoService;
        private readonly MainViewModel _mainViewModel;
        private readonly IWallpaperService _wallpaperService; 

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand NavigateHomeCommand { get; private set; }
        public ICommand SetWallpaperCommand { get; private set; }

        public PastImagesViewModel(IWallpaperInfoService wallpaperInfoService, MainViewModel mainViewModel, IWallpaperService wallpaperService) // Change to IWallpaperService
        {
            _wallpaperInfoService = wallpaperInfoService;
            _mainViewModel = mainViewModel;
            _wallpaperService = wallpaperService;
            Images = new ObservableCollection<WallpaperInfo>();
            NavigateHomeCommand = new RelayCommand(NavigateHome);
            SetWallpaperCommand = new RelayCommand(SetWallpaper);
        }

        // Method to load all images from the database asynchronously
        public async Task LoadImagesAsync()
        {
            try
            {
                var wallpapers = await _wallpaperInfoService.GetAllWallpapersAsync();
                App.Current.Dispatcher.Invoke(() => {
                    Images.Clear();
                    foreach (var wallpaper in wallpapers)
                    {
                        Images.Add(wallpaper);
                    }
                });
            }
            catch (Exception ex)
            {
                ReportError(ex.Message);
            }
        }

        private void NavigateHome(object parameter)
        {
            _mainViewModel.NavigateToHomeView();
        }

        // Wallpaper setting method to allow the user to set older wallpapers as their current wallpaper
        private void SetWallpaper(object parameter)
        {
            if (parameter is WallpaperInfo wallpaperInfo)
            {
                _wallpaperService.SetWallpaper(wallpaperInfo.ImagePath);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ReportError(string message)
        {
            Console.WriteLine($"An error occurred while loading images: {message}");
        }
    }
}
