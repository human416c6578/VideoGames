using Microsoft.EntityFrameworkCore;
using VideoGames.Models;

namespace VideoGames.data
{
    public class MvcGamesDbContext : DbContext
    {
        public MvcGamesDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
