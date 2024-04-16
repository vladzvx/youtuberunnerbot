using Microsoft.EntityFrameworkCore;
using YRB.Lib.Storage;

namespace YRB.Lib.Services
{
    public class ChromePathSource
    {
        private readonly IDbContextFactory<YrbDbContext> _dbContextFactory;
        public ChromePathSource(IDbContextFactory<YrbDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            using var context = _dbContextFactory.CreateDbContext();
            context.Database.EnsureCreated();
        }

        public async Task UpsertPath(string path)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var p = await context.ChromePathes.FirstOrDefaultAsync();
            var filename = Path.GetFileName(path);
            var directory = Path.GetDirectoryName(path);

            if (p == null)
            {
                await context.ChromePathes.AddAsync(new Storage.Entities.ChromePath()
                {
                    Filename = filename,
                    Directory = directory,
                    Id = 1,
                });
            }
            else
            {
                p.Filename = filename;
                p.Directory = directory;
                context.Update(p);
            }
            await context.SaveChangesAsync();
        }


        public async Task<string?> GetChromeDirectory()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var path = await context.ChromePathes.FirstOrDefaultAsync();
            return path?.Directory;
        }

        public async Task<string?> GetChromeExe()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var path = await context.ChromePathes.FirstOrDefaultAsync();
            return path?.Filename;
        }
    }
}
