using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WallPaperGenerator.Data;
using WallPaperGenerator.Models;
using WallPaperGenerator.Services;

namespace WallPaperGenerator.Services.Tests
{
    [TestClass]
    public class WallpaperInfoServiceTests
    {
        private AppDbContext _dbContext;
        private WallpaperInfoService _service;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:") 
                .Options;

            _dbContext = new AppDbContext(options);
            _dbContext.Database.OpenConnection();
            _dbContext.Database.EnsureCreated();

            _service = new WallpaperInfoService(_dbContext);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task AddOrUpdateWallpaperInfoAsync_ShouldAddNewWallpaperInfo()
        {
            var wallpaperInfo = new WallpaperInfo
            {
                ImagePath = "path/to/image",
                Location = "Test Location",
                WeatherData = new WeatherData { Condition = "Sunny", TemperatureCelsius = 25 }
            };

            await _service.AddOrUpdateWallpaperInfoAsync(wallpaperInfo);

            var wallpapers = await _dbContext.Wallpapers.ToListAsync();
            Assert.AreEqual(1, wallpapers.Count);
            Assert.AreEqual("path/to/image", wallpapers[0].ImagePath);
        }

        [TestMethod]
        public async Task AddOrUpdateWallpaperInfoAsync_ShouldRemoveOldestWallpaperIfMoreThan100()
        {
            for (int i = 1; i <= 101; i++)
            {
                var wallpaperInfo = new WallpaperInfo
                {
                    ImagePath = $"path/to/image{i}",
                    Location = "Test Location",
                    WeatherData = new WeatherData { Condition = "Sunny", TemperatureCelsius = 25 }
                };
                await _service.AddOrUpdateWallpaperInfoAsync(wallpaperInfo);
            }

            var wallpapers = await _dbContext.Wallpapers.ToListAsync();

            Assert.AreEqual(100, wallpapers.Count);
            Assert.IsFalse(wallpapers.Any(w => w.ImagePath == "path/to/image1"));
        }

        [TestMethod]
        public async Task GetAllWallpapersAsync_ShouldReturnAllWallpapers()
        {
            var wallpapers = new List<WallpaperInfo>
            {
                new WallpaperInfo
                {
                    ImagePath = "path/to/image1",
                    Location = "Test Location 1",
                    WeatherData = new WeatherData { Condition = "Sunny", TemperatureCelsius = 25 }
                },
                new WallpaperInfo
                {
                    ImagePath = "path/to/image2",
                    Location = "Test Location 2",
                    WeatherData = new WeatherData { Condition = "Cloudy", TemperatureCelsius = 20 }
                },
                new WallpaperInfo
                {
                    ImagePath = "path/to/image3",
                    Location = "Test Location 3",
                    WeatherData = new WeatherData { Condition = "Rainy", TemperatureCelsius = 15 }
                }
            };

            await _dbContext.Wallpapers.AddRangeAsync(wallpapers);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetAllWallpapersAsync();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("path/to/image1", result[0].ImagePath);
            Assert.AreEqual("path/to/image2", result[1].ImagePath);
            Assert.AreEqual("path/to/image3", result[2].ImagePath);
            Assert.AreEqual("Test Location 1", result[0].Location);
            Assert.AreEqual("Test Location 2", result[1].Location);
            Assert.AreEqual("Test Location 3", result[2].Location);
            Assert.AreEqual("Sunny", result[0].WeatherData.Condition);
            Assert.AreEqual("Cloudy", result[1].WeatherData.Condition);
            Assert.AreEqual("Rainy", result[2].WeatherData.Condition);
        }

        [TestMethod]
        public async Task GetWeatherDataByIdAsync_ShouldReturnWeatherData()
        {
            var weatherData = new WeatherData { Condition = "Sunny", TemperatureCelsius = 25, WeatherID = 1 };
            var weatherData2 = new WeatherData { Condition = "Cloudy", TemperatureCelsius = 20, WeatherID = 2 };
            var weatherData3 = new WeatherData { Condition = "Rainy", TemperatureCelsius = 15, WeatherID = 3 };
            _dbContext.WeatherData.Add(weatherData);
            _dbContext.WeatherData.Add(weatherData2);
            _dbContext.WeatherData.Add(weatherData3);
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetWeatherDataByIdAsync(weatherData.WeatherID);
            var result2 = await _service.GetWeatherDataByIdAsync(weatherData2.WeatherID);
            var result3 = await _service.GetWeatherDataByIdAsync(weatherData3.WeatherID);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);
            Assert.AreEqual("Sunny", result.Condition);
            Assert.AreEqual("Cloudy", result2.Condition);
            Assert.AreEqual("Rainy", result3.Condition);
        }
    }
}
