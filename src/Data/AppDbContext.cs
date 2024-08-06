﻿using Microsoft.EntityFrameworkCore;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<WallpaperInfo> Wallpapers { get; set; }
        public DbSet<WeatherData> WeatherData { get; set; }
    }
}
