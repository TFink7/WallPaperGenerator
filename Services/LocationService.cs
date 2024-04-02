using System.Net.Http;
using System.Threading.Tasks;
using WallPaperGenerator.Models;
using Newtonsoft.Json; 

namespace WallPaperGenerator.Services
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;

        public LocationService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<LocationData> GetCurrentLocationAsync()
        {
            // Example API call to a geolocation service
            string apiUrl = "https://api.ipgeolocation.io/ipgeo?apiKey=YOUR_API_KEY";
            var response = await _httpClient.GetStringAsync(apiUrl);
            var locationData = JsonConvert.DeserializeObject<LocationData>(response);

            return locationData;
        }
    }
}
