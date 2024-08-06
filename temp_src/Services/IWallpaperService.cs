using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public interface IWallpaperService
    {
        Task<string> GenerateWallpaperAsync(string city, string country, string condition, double temperatureCelsius);
        void SetWallpaper(string imagePath);
    }
}