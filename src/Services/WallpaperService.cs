using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Images;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public class WallpaperService : IWallpaperService
    {
        private readonly OpenAIAPI _api;
        private readonly IWallpaperInfoService _infoService;
        private readonly IHttpClientFactory _httpClientFactory;

        public WallpaperService(IWallpaperInfoService wallpaperInfoService, IHttpClientFactory httpClientFactory)
        {
            _api = new OpenAIAPI(APIAuthentication.Default);
            _infoService = wallpaperInfoService;
            _httpClientFactory = httpClientFactory;
        }
        
        // Generates a wallpaper based on the provided city, country, weather condition, and temperature
        public async Task<string> GenerateWallpaperAsync(string city, string country, string condition, double temperatureCelsius)
        {
            var prompt = $"A non-distorted semi-realistic wallpaper background showcasing elements of {city} in {country} with {condition} weather and {temperatureCelsius}°C always displayed in the right corner of the image";
            var request = new ImageGenerationRequest(prompt, OpenAI_API.Models.Model.DALLE3, ImageSize._1024);

            try
            {
                // Request image generation from the OpenAI API
                var result = await _api.ImageGenerations.CreateImageAsync(request);

                if (result.Data != null && result.Data.Count > 0)
                {
                    var imageUrl = result.Data[0].Url;
                    string filename = $"{Guid.NewGuid()}.png";
                    string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Wallpapers", filename);

                    // Downloading generated image locally
                    var client = _httpClientFactory.CreateClient();
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                    await File.WriteAllBytesAsync(localPath, imageBytes);

                    var weatherData = new WeatherData(condition, temperatureCelsius, DateTime.Now);
                    var wallpaperInfo = new WallpaperInfo(localPath, $"{city}, {country}", weatherData);

                    await _infoService.AddOrUpdateWallpaperInfoAsync(wallpaperInfo);
                    return localPath;
                }
                else
                {
                    throw new InvalidOperationException("Failed to generate image URL.");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"HttpRequestException: {e.Message}");
                throw;
            }
            catch (IOException e)
            {
                Console.WriteLine($"IOException: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                throw;
            }
        }

        // Set wallpaper using given image
        public void SetWallpaper(string imagePath)
        {
            try
            {
                var uri = new Uri(imagePath, UriKind.Absolute);
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                throw;
            }
        }

        // Constants for system call to set wallpaper
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}
