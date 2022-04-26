using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using WarspearBot.Contracts;
using WarspearBot.Helpers;
using WarspearBot.Models;
using Microsoft.Extensions.Logging;

namespace WarspearBot.Services
{
    public class BotEngine : IBotEngine
    {
        private readonly IWindowAgent _windowAgent;
        private readonly IFileStorage _storage;
        private readonly IImageProcessor _imageProcessor;
        private readonly ILogger<BotEngine> _logger;

        private EngineInitConfiguration _config;
        private IntPtr _windowHandle;

        private Dictionary<string, Bitmap> _scenarioImages;

        public BotEngine(IWindowAgent windowAgent, IFileStorage storage, IImageProcessor imageProcessor, ILogger<BotEngine> logger)
        {
            _windowAgent = windowAgent ?? throw new ArgumentNullException(nameof(windowAgent));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Init(EngineInitConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _windowHandle = _windowAgent.GetWindowHandleByName(configuration.WindowName);

            var (windowW, windowH) = _windowAgent.GetWindowSize(_windowHandle);

            if (windowW != _config.WindowExpectedSize.W || windowH != _config.WindowExpectedSize.H)
            {
                throw new Exception("Unexpected window size");
            }

            _windowAgent.MoveWindow(_windowHandle, configuration.WindowInitialLocation);

            if (configuration.PinWindowToTop)
            {
                _windowAgent.PinToTop(_windowHandle);
            }

            _scenarioImages = _storage
                .LoadImages(configuration.ScenarioName)
                .ToDictionary(x => x.Name, x => x.Image);

            return Task.CompletedTask;
        }

        public Bitmap MakeScreenshot((int X, int Y) location, (int W, int H) size)
        {
            var bitmap = new Bitmap(size.W, size.H);
            var (windowX, windowY) = _windowAgent.GetWindowLocation(_windowHandle);

            var x = windowX + _config.GameScreenLeftTop.X + location.X;
            var y = windowY + _config.GameScreenLeftTop.Y + location.Y;

            return _windowAgent.MakeScreenshot(bitmap, (x, y));
        }

        public List<TemplateMatchInfo> GetTemplateMatches(Bitmap image, params (string TemplateName, float Threshold)[] templateInfos)
        {
            var matches = templateInfos
                .SelectMany(info =>
                {
                    var template = _scenarioImages[info.TemplateName];
                    var matches = _imageProcessor.GetMatches(image, template, info.Threshold);

                    return matches.Select(x => new TemplateMatchInfo
                    {
                        TemplateName = info.TemplateName,
                        Threshold = info.Threshold,
                        MatchingRate = x.Rate,
                        Location = (x.X, x.Y),
                        TemplateSize = (template.Width, template.Height),
                    });
                })
                .ToList();

            _logger.LogInformation($"Matching [{templateInfos.Select(x => x.TemplateName).Join()}]. " +
                                   $"Total: {matches.Count}. " +
                                   $"Best: {(matches.Count > 0 ? matches.OrderByDescending(x => x.MatchingRate).Select(x => $"({x.TemplateName}: {x.MatchingRate})").First() : "-")}");
            return matches;
        }

        public void Click((int X, int Y) location, MouseClick mouseClick)
        {
            var (windowX, windowY) = _windowAgent.GetWindowLocation(_windowHandle);
            var x = windowX + _config.GameScreenLeftTop.X + location.X;
            var y = windowY + _config.GameScreenLeftTop.Y + location.Y;
            _windowAgent.MouseClick((x, y), mouseClick);
        }

        public Bitmap MakeGameScreenshot()
        {
            return MakeScreenshot((0, 0), _config.GameScreenSize);
        }
    }
}