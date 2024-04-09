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

        public async Task<WeatherData?> GetWeatherAsync(LocationData locationData)
        {
            string query = $"{locationData.Latitude},{locationData.Longitude}";
            var apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={Uri.EscapeDataString(query)}&aqi=no");
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                try
                {
                    dynamic? data = JsonConvert.DeserializeObject<dynamic>(body) ?? throw new JsonSerializationException();
                    var weatherData = new WeatherData
                    {
                        Condition = data.current.condition.text,
                        TemperatureCelsius = data.current.temp_c,
                        DataCapturedDate = DateTime.Now
                    };

                    return weatherData;
                }
                catch (JsonReaderException e)
                {
                    //handle invalid JSON format
                    Console.WriteLine($"\nInvalid JSON format: {e.Message}");
                    return null;
                }
                catch (JsonSerializationException e)
                {
                    //handle invalid JSON data
                    Console.WriteLine($"\nInvalid JSON data: {e.Message}");
                    return null;
                }
            }

            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nException caught: {e.Message}");
                return null;
            }
        }
    }
}
