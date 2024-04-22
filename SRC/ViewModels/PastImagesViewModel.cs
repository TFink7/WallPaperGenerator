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
    public class PastImagesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WallpaperInfo> Images { get; private set; }
        private readonly IWallpaperInfoService _wallpaperInfoService;
        private readonly MainViewModel _mainViewModel;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand NavigateHomeCommand { get; private set; }

        public PastImagesViewModel(IWallpaperInfoService wallpaperInfoService, MainViewModel mainViewModel)
        {
            _wallpaperInfoService = wallpaperInfoService;
            Images = new ObservableCollection<WallpaperInfo>();
            _mainViewModel = mainViewModel;
            NavigateHomeCommand = new RelayCommand(NavigateHome);
        }

        public async Task InitializeAsync()
        {
            await LoadImagesAsync();
        }

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
