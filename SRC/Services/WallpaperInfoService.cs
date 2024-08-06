using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallPaperGenerator.Data;
using WallPaperGenerator.Models;

namespace WallPaperGenerator.Services
{
    public class WallpaperInfoService : IWallpaperInfoService
    {
        private readonly AppDbContext _context;

        public WallpaperInfoService(AppDbContext context)
        {
            _context = context;
        }

        // Adds or updates WallpaperInfo asynchronously
        public async Task AddOrUpdateWallpaperInfoAsync(WallpaperInfo wallpaperInfo)
        {
            try
            {
                var entry = _context.Entry(wallpaperInfo);
                if (entry.State == EntityState.Detached)
                {
                    // If the entity is not tracked by the context, add it to the context
                    _context.Wallpapers.Add(wallpaperInfo);
                }
                await _context.SaveChangesAsync();

                // Ensure that the number of wallpapers does not exceed 100 to manage storage and performance
                if (await _context.Wallpapers.CountAsync() > 100)
                {
                    var oldest = await _context.Wallpapers.OrderBy(w => w.WallPaperInfoID).FirstAsync();
                    _context.Wallpapers.Remove(oldest);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred when saving to the database: {ex.Message}");

                throw;
            }
        }

        // Retrieves all wallpapers including related WeatherData asynchronously
        public async Task<List<WallpaperInfo>> GetAllWallpapersAsync()
        {
            return await _context.Wallpapers
                .Include(w => w.WeatherData) 
                .ToListAsync();
        }

        // Retrieves WeatherData by its ID asynchronously
        public async Task<WeatherData> GetWeatherDataByIdAsync(int id)
        {
            return await _context.WeatherData.FindAsync(id);
        }
    }
}
