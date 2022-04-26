using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using WarspearBot.Contracts;

namespace WarspearBot.Services
{
    public class ImageProcessor : IImageProcessor
    {
        public IEnumerable<(int X, int Y, float Rate)> GetMatches(Bitmap image, Bitmap template, float threshold)
        {
            using var imageToScan = image.ToImage<Bgr, byte>();
            using var templateImage = template.ToImage<Bgr, byte>();

            using var matches = imageToScan.MatchTemplate(templateImage, TemplateMatchingType.CcoeffNormed);

            for (var y = 0; y < matches.Data.GetLength(0); y++)
            {
                for (var x = 0; x < matches.Data.GetLength(1); x++)
                {
                    var probability = matches.Data[y, x, 0];

                    if (probability >= threshold)
                    {
                        yield return (x, y, probability);
                    }
                }
            }
        }
    }
}