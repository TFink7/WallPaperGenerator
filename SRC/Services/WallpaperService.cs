using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;
using System.IO;
using OpenAI_API.Images;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public class WallpaperService : IWallpaperService
    {
        private readonly OpenAIAPI _api;
        private readonly IWallpaperInfoService _infoService;


        public WallpaperService(IWallpaperInfoService wallpaperInfoService)
        {
            _api = new OpenAIAPI(APIAuthentication.Default);
            _infoService = wallpaperInfoService;
        }

        public async Task<string> GenerateWallpaperAsync(string city, string country, string condition, double temperatureCelsius)
        {

            var prompt = $"An imaginative wallpaper background showcasing elements of {city} in {country} with {condition} weather and {temperatureCelsius}°C displayed in the top left corner of the image";
            var request = new ImageGenerationRequest(prompt, OpenAI_API.Models.Model.DALLE3, ImageSize._1024);

            var result = await _api.ImageGenerations.CreateImageAsync(request);

            if (result.Data != null && result.Data.Count > 0)
            {
                var imageUrl = result.Data[0].Url;

                string filename = $"{Guid.NewGuid()}.png";
                string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Wallpapers", filename);

                using (var client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                    await File.WriteAllBytesAsync(localPath, imageBytes);
                }

                var weatherData = new WeatherData
                {
                    Condition = condition,
                    TemperatureCelsius = temperatureCelsius,
                    DataCapturedDate = DateTime.Now
                };

                var wallpaperInfo = new WallpaperInfo
                {
                    ImagePath = localPath, 
                    Location = $"{city}, {country}",
                    WeatherData = weatherData
                };

                await _infoService.AddOrUpdateWallpaperInfoAsync(wallpaperInfo);
                return localPath;
            }
            else
            {
                throw new InvalidOperationException("Failed to generate image URL.");
            }
        }


        public async Task SetWallpaperAsync(string imagePath)
        {
            var uri = new Uri(imagePath, UriKind.Absolute);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}