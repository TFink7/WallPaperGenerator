using Newtonsoft.Json;


namespace WallPaperGenerator.Models
{
    public class LocationData
    {
        //maps "city" property from JSON API response
        [JsonProperty("city")]
        public string City { get; private set; }
        //maps "country_name" property from JSON API response 
        [JsonProperty("country_name")]
        public string Country { get; private set; }
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }

        public LocationData(string city, string country, string latitude, string longitude)
        {
            City = city;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
