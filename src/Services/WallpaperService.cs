using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using OpenAI;
using OpenAI.Images;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public class WallpaperService : IWallpaperService
    {
        private readonly OpenAIClient _client;
        private readonly IWallpaperInfoService _infoService;
        private readonly IHttpClientFactory _httpClientFactory;

        public WallpaperService(IWallpaperInfoService wallpaperInfoService, IHttpClientFactory httpClientFactory)
        {
            _client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            _infoService = wallpaperInfoService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GenerateWallpaperAsync(string city, string country, string condition, double temperatureCelsius)
        {
            var prompt = $"A non-distorted semi-realistic wallpaper background showcasing elements of {city} in {country} with {condition} weather and {temperatureCelsius}°C always displayed in the right corner of the image";
            var imageClient = _client.GetImageClient("gpt-image-2");
            var options = new ImageGenerationOptions { Size = GeneratedImageSize.W1024xH1024 };

            try
            {
                var result = await imageClient.GenerateImageAsync(prompt, options);
                var image = result.Value;

                string filename = $"{Guid.NewGuid()}.png";
                string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Wallpapers", filename);
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));

                if (image.ImageBytes != null)
                {
                    await File.WriteAllBytesAsync(localPath, image.ImageBytes.ToArray());
                }
                else
                {
                    var client = _httpClientFactory.CreateClient();
                    var imageBytes = await client.GetByteArrayAsync(image.ImageUri);
                    await File.WriteAllBytesAsync(localPath, imageBytes);
                }

                var weatherData = new WeatherData(condition, temperatureCelsius, DateTime.Now);
                var wallpaperInfo = new WallpaperInfo(localPath, $"{city}, {country}", weatherData);

                await _infoService.AddOrUpdateWallpaperInfoAsync(wallpaperInfo);
                return localPath;
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

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}
