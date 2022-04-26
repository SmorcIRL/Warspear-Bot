using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WarspearBot.Contracts;
using WarspearBot.Models;
using WarspearBot.Sceneries;
using WarspearBot.Services;

namespace WarspearBot
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new NotSupportedException("OS other then Windows is not supported");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            var configuration = config.Get<AppConfiguration>();

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(configuration);
                    services.AddLogging(x => x.AddNLog("NLog.config"));
                    services.AddSingleton<IFileStorage, FileStorage>();
                    services.AddSingleton<IImageProcessor, ImageProcessor>();
                    services.AddSingleton<IWindowAgent, WindowAgent>();
                    services.AddSingleton<IBotEngine, BotEngine>();
                    services.AddHostedService<TestScenario>();
                    services.AddHostedService<PerformanceLogger>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}