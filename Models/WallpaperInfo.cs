using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallPaperGenerator.Models
{
    class WallpaperInfo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ImagePath { get; set; }
        public string Location { get; set; }
        public int WeatherDataId { get; set; }
        public WeatherData WeatherData { get; set; }
    }
}
