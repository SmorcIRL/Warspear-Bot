using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WarspearBot.Contracts;
using WarspearBot.Models;

namespace WarspearBot.Sceneries
{
    public abstract class ScenarioBase : BackgroundService
    {
        protected IFileStorage Storage { get; }

        protected IImageProcessor ImageProcessor { get; }

        protected IBotEngine Engine { get; }

        protected ILogger Logger { get; }

        protected EngineInitConfiguration Configuration { get; private set; }

        protected ScenarioBase(IFileStorage fileStorage, IImageProcessor imageProcessor, IBotEngine engine, ILogger logger)
        {
            Storage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            ImageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
            Engine = engine ?? throw new ArgumentNullException(nameof(engine));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Configuration = GetConfiguration();

                await Engine.Init(Configuration);

                await RunScenario(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        protected abstract EngineInitConfiguration GetConfiguration();

        protected abstract Task RunScenario(CancellationToken token);

        protected static Task Delay(int delay)
        {
            return Task.Delay(delay);
        }
    }
}