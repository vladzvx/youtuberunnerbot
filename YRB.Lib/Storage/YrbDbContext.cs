using Microsoft.EntityFrameworkCore;
using YRB.Lib.Storage.Entities;

namespace YRB.Lib.Storage
{
    public class YrbDbContext : DbContext
    {
        public static string GetDbPath()
        {
            var path = Environment.ProcessPath;
            if (path == null)
            {
                path = Environment.CurrentDirectory;
            }
            else
            {
                path = Path.GetDirectoryName(path);
            }
            path = Path.Combine(path ?? string.Empty, "yrb");
            return path;
        }

        public string DbPath { get; } = string.Empty;
        public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
        public DbSet<ChromePath> ChromePathes { get; set; }

        public YrbDbContext()
        {
            var path = GetDbPath();
            Directory.CreateDirectory(path);
            DbPath = System.IO.Path.Join(path, "yrb.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorizedUser>().HasKey(au => au.Id);
            modelBuilder.Entity<ChromePath>().HasKey(cp => cp.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}
