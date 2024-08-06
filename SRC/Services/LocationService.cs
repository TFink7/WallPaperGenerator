using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public class LocationService : ILocationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LocationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Asynchronously retrieves current location data for use in image generation and fetching weather data
        public async Task<LocationData> GetCurrentLocationAsync()
        {
            var apiKey = Environment.GetEnvironmentVariable("IPGEOLOCATION_API_KEY");
            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.GetAsync($"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}");

                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();

                try
                {
                    var locationData = JsonConvert.DeserializeObject<LocationData>(body) ?? throw new JsonSerializationException();
                    return locationData;
                }
                catch (JsonReaderException e)
                {
                    Console.WriteLine($"\nInvalid JSON format: {e.Message}");
                    return null;
                }
                catch (JsonSerializationException e)
                {
                    Console.WriteLine($"\nInvalid JSON content: {e.Message}");
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nHTTP Request Exception caught: {e.Message}");
                return null;
            }
        }
    }
}
