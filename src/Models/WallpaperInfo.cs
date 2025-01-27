using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WallPaperGenerator.Models
{
    public class WallpaperInfo
    {
        [Key]
        public int WallPaperInfoID { get; set; }

        [Required]
        public string ImagePath { get; set; }

        public string Location { get; set; }

        public int WeatherDataId { get; set; }

        [ForeignKey("WeatherDataId")]
        public WeatherData WeatherData { get; set; }
        
        // Parameterless constructor for EF Core
        public WallpaperInfo() { }

        // Regular constructor for normal operation
        public WallpaperInfo(string imagePath, string location, WeatherData weatherData)
        {
            ImagePath = imagePath;
            Location = location;
            WeatherData = weatherData;
        }
    }
}
