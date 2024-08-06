using System;
using System.ComponentModel.DataAnnotations;

namespace WallPaperGenerator.Models
{
    public class WeatherData
    {
        [Key]
        public int WeatherID { get; set; }

        [Required]
        public string Condition { get; set; }

        [Required]
        public double TemperatureCelsius { get; set; }

        [Required]
        public DateTime DataCapturedDate { get; set; }

        public WeatherData() { }

        public WeatherData(string condition, double temperatureCelsius, DateTime dataCapturedDate)
        {
            Condition = condition;
            TemperatureCelsius = temperatureCelsius;
            DataCapturedDate = dataCapturedDate;
        }
    }
}
