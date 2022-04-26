using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WarspearBot.Contracts;
using WarspearBot.Models;

namespace WarspearBot.Sceneries
{
    public class TestScenario : ScenarioBase
    {
        private readonly (string TemplateName, float Threshold)[] _fairyTemplates =
        {
            ("fairy_up.bmp", 0.6f),
            ("fairy_right.bmp", 0.6f),
            ("fairy_down.bmp", 0.6f),
            ("fairy_left.bmp", 0.6f),
        };

        private readonly (string TemplateName, float Threshold)[] _fairyAttackedTemplates =
        {
            ("fairy_attacked_1.bmp", 0.7f),
            ("fairy_attacked_2.bmp", 0.7f),
        };

        private readonly (string TemplateName, float Threshold)[] _lootTemplates =
        {
            ("loot_1.bmp", 0.8f),
            ("loot_2.bmp", 0.8f),
            ("loot_3.bmp", 0.8f),
            ("loot_4.bmp", 0.8f),
            ("loot_5.bmp", 0.8f),
        };

        private readonly (string TemplateName, float Threshold)[] _lootHandTemplates =
        {
            ("loot_hand_1.bmp", 0.8f),
            ("loot_hand_2.bmp", 0.8f),
        };

        public TestScenario(IFileStorage fileStorage, IImageProcessor imageProcessor, IBotEngine engine, ILogger<TestScenario> logger)
            : base(fileStorage, imageProcessor, engine, logger)
        {
        }

        protected override EngineInitConfiguration GetConfiguration()
        {
            return new EngineInitConfiguration
            {
                WindowName = "VirtualBoxVM",
                MoveWindowOnStart = false,
                WindowInitialLocation = (0, 0),
                WindowExpectedSize = (816, 639),
                PinWindowToTop = true,
                GameScreenLeftTopOffset = (8, 31),
                GameScreenSize = (800, 600),
                ScenarioName = "TestScenario",
                //=========================================//
                CollectLootClickLocation = (300, 140),
                Skill2Location = (180, 580),
                CloseEnemySelectionClickLocation = (300, 530),
                DeclineInventoryExtendingClickLocation = (310, 370),
                CloseLootWindowClickLocation = (300, 515),
            };
        }

        protected override async Task RunScenario(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await FarmCycle();

                await Delay(1000);
            }
        }

        private async Task FarmCycle()
        {
            TemplateMatchInfo fairyMatch = null;

            while (fairyMatch == null)
            {
                using var screen = Engine.MakeGameScreenshot();

                var matchesFairy = Engine.GetTemplateMatches(screen, _fairyTemplates);
                if (matchesFairy.Count != 0)
                {
                    fairyMatch = matchesFairy.FirstOrDefault();
                    continue;
                }

                await Delay(200);
            }

            Engine.Click((fairyMatch.Location.X + fairyMatch.TemplateSize.W / 2, fairyMatch.Location.Y + fairyMatch.TemplateSize.H - 10));
            await Delay(200);

            using var screenAfterFairyClick = Engine.MakeGameScreenshot();

            var matchesEnemySelection = Engine.GetTemplateMatches(screenAfterFairyClick, ("enemy_selection.bmp", 0.9f));
            if (matchesEnemySelection.Count != 0)
            {
                var matchesFairySelection = Engine.GetTemplateMatches(screenAfterFairyClick, ("fairy_selection.bmp", 0.9f));
                if (matchesFairySelection.Count != 0)
                {
                    ClickOnMatchCenter(matchesFairySelection[0]);
                }
                else
                {
                    Engine.Click(Configuration.CloseEnemySelectionClickLocation);
                    return;
                }
            }

            await Delay(5000);

            TemplateMatchInfo lootMatch = null;
            TemplateMatchInfo redAreaMatch = null;

            for (var i = 0; i < 10; i++)
            {
                using var screen = Engine.MakeGameScreenshot();

                var matchesRedArea = Engine.GetTemplateMatches(screen, _fairyAttackedTemplates);
                if (matchesRedArea.Count != 0)
                {
                    redAreaMatch = matchesRedArea[0];
                }

                var matchesHand = Engine.GetTemplateMatches(screen, _lootHandTemplates);
                if (matchesHand.Count != 0)
                {
                    lootMatch = matchesHand[0];
                    break;
                }

                var matchesLoot = Engine.GetTemplateMatches(screen, _lootTemplates);
                if (matchesLoot.Count != 0)
                {
                    lootMatch = matchesLoot
                        .Select(x => (Match: x, Distance: GetDistance(redAreaMatch?.Location ?? fairyMatch.Location, x.Location)))
                        .OrderBy(x => x.Distance)
                        .Where(x => x.Distance < (redAreaMatch == null ? 200 : 50))
                        .Select(x => x.Match)
                        .FirstOrDefault();

                    if (lootMatch != null)
                    {
                        break;
                    }
                }

                await Delay(500);
            }

            if (lootMatch != null)
            {
                Logger.LogInformation($"Clicking on loot match: {lootMatch.Location}");
                ClickOnMatchCenter(lootMatch);
                await Delay(200);
            }
            else if (redAreaMatch != null)
            {
                Logger.LogInformation($"Clicking on last red area match: {redAreaMatch.Location}");
                ClickOnMatchCenter(redAreaMatch);
                await Delay(200);
            }
            else
            {
                Logger.LogInformation("Loot not found, repeating farm cycle");
                return;
            }

            using var screenAfterLootClick = Engine.MakeGameScreenshot();

            var lootSelection = Engine.GetTemplateMatches(screenAfterLootClick, ("loot_selection.bmp", 0.9f));
            if (lootSelection.Count != 0)
            {
                ClickOnMatchCenter(lootSelection[0]);
            }

            await Delay(1000);

            for (var i = 0; i < 10; i++)
            {
                using var screen = Engine.MakeGameScreenshot();

                var matchesLootBag = Engine.GetTemplateMatches(screen, ("loot_bag.bmp", 0.9f));
                if (matchesLootBag.Count != 0)
                {
                    await Delay(300);

                    Engine.Click(Configuration.CollectLootClickLocation);
                    await Delay(300);

                    using var screenAfterCollectLootClick = Engine.MakeGameScreenshot();

                    var matchesNotification = Engine.GetTemplateMatches(screenAfterCollectLootClick, ("inventory_full_notification.bmp", 0.9f));
                    if (matchesNotification.Count != 0)
                    {
                        Logger.LogWarning("Inventory is full");

                        Engine.Click(Configuration.DeclineInventoryExtendingClickLocation);
                        await Delay(300);

                        Engine.Click(Configuration.CloseLootWindowClickLocation);
                        await Delay(300);
                    }
                    else
                    {
                        Logger.LogInformation("Loot collected");
                    }

                    break;
                }

                await Delay(300);
            }

            var hpPercent = GetHealthPercent();
            Logger.LogInformation($"HP percent: {hpPercent}");
            if (hpPercent < 80)
            {
                Logger.LogInformation("Healing");
                Engine.Click(Configuration.Skill2Location, MouseClick.Right);
                await Delay(500);
            }
        }

        private int GetHealthPercent()
        {
            using var hpScreen = Engine.MakeScreenshot((38, 34), (134, 1));
            var count = CountPixels(hpScreen, Color.FromArgb(148, 0, 0));

            return (int)(count / 134.0 * 100);
        }

        private double GetDistance((int X, int Y) point1, (int X, int Y) point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        private int CountPixels(Bitmap image, Color color)
        {
            var matches = 0;

            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    if (image.GetPixel(x, y) == color)
                    {
                        matches++;
                    }
                }
            }

            return matches;
        }

        private void ClickOnMatchCenter(TemplateMatchInfo match)
        {
            Engine.Click((match.Location.X + match.TemplateSize.W / 2, match.Location.Y + match.TemplateSize.H / 2));
        }
    }
}