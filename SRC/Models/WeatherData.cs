using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace WallPaperGenerator.Models
{
    public class WeatherData
    {
        public int Id { get; set; }
        public string Condition { get; set; }
        public double TemperatureCelsius { get; set; }
        public DateTime DataCapturedDate { get; set; }
    }
}
