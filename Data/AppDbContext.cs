using HaberPortali2.Models;
using Microsoft.EntityFrameworkCore;

namespace HaberPortali2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }          // Kullanıcılar
        public DbSet<Category> Categories { get; set; } // Kategoriler
        public DbSet<News> News { get; set; }           // Haberler
        public DbSet<Comment> Comments { get; set; }     // Yorumlar
    }
}
