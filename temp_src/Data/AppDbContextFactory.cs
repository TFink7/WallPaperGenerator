using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WallPaperGenerator.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = "Data Source=WallPaperGenerator.db"; 

            optionsBuilder.UseSqlite(connectionString); 

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
