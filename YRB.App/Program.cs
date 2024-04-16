using Microsoft.Extensions.Hosting;
using YRB.Lib;

namespace YRB.App
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            var builder = Host.CreateApplicationBuilder();
            builder.Services.AddBot();
            var host = builder.Build();
            host.RunAsync();
            ApplicationConfiguration.Initialize();
            Application.Run(new YoutubeRunner());
        }
    }
}