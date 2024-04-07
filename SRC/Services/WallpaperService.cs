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

namespace WallPaperGenerator.Services
{
    public class WallpaperService : IWallpaperService
    {
        private readonly OpenAIAPI _api;

        public WallpaperService()
        {
            _api = new OpenAIAPI(APIAuthentication.Default);
        }

        public async Task<string> GenerateWallpaperAsync(string city, string country, string condition, double temperatureCelsius)
        {
            var prompt = $"An imaginative landscape of {city} in {country} showcasing {condition} weather at {temperatureCelsius}°C";
            var request = new ImageGenerationRequest(prompt, OpenAI_API.Models.Model.DALLE3); // Adapt parameters as needed

            var result = await _api.ImageGenerations.CreateImageAsync(request);

            if (result.Data != null && result.Data.Count > 0)
            {
                return result.Data[0].Url;
            }
            else 
            {
                throw new InvalidOperationException("Failed to generate image URL.");
            }
        }
        
        
        public async Task SetWallpaperAsync(string imagePath)
        {
            string filename = Path.GetFileName(imagePath);
            string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), filename);

            using (var client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(imagePath);
                await File.WriteAllBytesAsync(localPath, imageBytes);
            }

            SystemParametersInfo(20, 0, localPath, 1 | 2);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}