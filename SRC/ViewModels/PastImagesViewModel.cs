using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WallPaperGenerator.Models;
using WallPaperGenerator.Services;

namespace WallPaperGenerator.ViewModels
{
    public class PastImagesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WallpaperInfo> Images { get; private set; }
        private readonly IWallpaperInfoService _wallpaperInfoService;

        public event PropertyChangedEventHandler PropertyChanged;

        public PastImagesViewModel(IWallpaperInfoService wallpaperInfoService)
        {
            _wallpaperInfoService = wallpaperInfoService;
            Images = new ObservableCollection<WallpaperInfo>();
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
