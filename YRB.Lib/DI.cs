using Microsoft.Extensions.DependencyInjection;
using YRB.Lib.Services;
using YRB.Lib.Storage;

namespace YRB.Lib
{
    public static class DI
    {
        public static IServiceCollection AddBot(this IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<ChromePathSource>();
            services.AddSingleton<UsersRepository>();
            services.AddDbContextFactory<YrbDbContext>();
            services.AddHostedService<BotWorker>();
            return services;
        }
    }
}
