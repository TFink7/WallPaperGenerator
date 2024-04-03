using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WeatherData?> GetWeatherAsync(string location)
        {
            var apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={location}&aqi=no");
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                dynamic? data = JsonConvert.DeserializeObject<dynamic>(body);
                if (data == null) return null;

                var weatherData = new WeatherData
                {
                    Condition = data.current.condition.text,
                    TemperatureCelsius = data.current.temp_c,
                    DataCapturedDate = DateTime.Now
                };

                return weatherData;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nException caught: {e.Message}");
                return null;
            }
        }
    }
}
