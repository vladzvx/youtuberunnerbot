using Microsoft.EntityFrameworkCore;
using YRB.Lib.Storage;
using YRB.Lib.Storage.Entities;

namespace YRB.Lib.Services
{
    public class UsersRepository
    {
        private readonly IDbContextFactory<YrbDbContext> _dbContextFactory;
        public UsersRepository(IDbContextFactory<YrbDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            using var context = _dbContextFactory.CreateDbContext();
            context.Database.EnsureCreated();
        }

        public async Task<bool> CheckUserExistance(long userId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var res = await context.AuthorizedUsers.CountAsync(au => au.Id == userId);
            return res > 0;
        }

        public async Task UpsertUser(long userId, string? username, string firstName)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await context.AuthorizedUsers.FirstOrDefaultAsync(au => au.Id == userId);
            if (user == null)
            {
                await context.AddAsync(new AuthorizedUser()
                {
                    FirstName = firstName,
                    Id = userId,
                    Username = username,
                });
            }
            else
            {
                user.Username = username;
                user.FirstName = firstName;
                context.Update(user);
            }
            await context.SaveChangesAsync();
        }

    }
}
