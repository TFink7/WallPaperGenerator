using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallPaperGenerator.Models
{
    public class LocationData
    {
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country_name")]
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

    }
}
