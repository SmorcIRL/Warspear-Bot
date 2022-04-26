using System.Collections.Generic;
using System.Drawing;

namespace WarspearBot.Contracts
{
    public interface IFileStorage
    {
        List<(Bitmap Image, string Name)> LoadImages(string directory);
    }
}