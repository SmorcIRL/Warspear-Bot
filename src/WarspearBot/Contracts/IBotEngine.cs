using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using WarspearBot.Models;

namespace WarspearBot.Contracts
{
    public interface IBotEngine
    {
        Task Init(EngineInitConfiguration configuration);

        Bitmap MakeScreenshot((int X, int Y) location, (int W, int H) size);

        List<TemplateMatchInfo> GetTemplateMatches(Bitmap image, params (string TemplateName, float Threshold)[] templateInfos);

        void Click((int X, int Y) location, MouseClick mouseClick = MouseClick.Left);

        Bitmap MakeGameScreenshot();
    }
}