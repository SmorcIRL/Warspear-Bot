using System.Collections.Generic;
using System.Drawing;

namespace WarspearBot.Contracts
{
    public interface IImageProcessor
    {
        IEnumerable<(int X, int Y, float Rate)> GetMatches(Bitmap image, Bitmap template, float threshold);
    }
}