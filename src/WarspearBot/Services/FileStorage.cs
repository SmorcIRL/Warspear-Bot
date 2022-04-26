using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using WarspearBot.Contracts;
using WarspearBot.Models;

namespace WarspearBot.Services
{
    public class FileStorage : IFileStorage
    {
        private readonly string _imagesPath;

        public FileStorage(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration.FileStorage == null)
            {
                throw new ArgumentException(nameof(configuration.FileStorage));
            }

            _imagesPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuration.FileStorage.ImagesPath));
        }

        public List<(Bitmap Image, string Name)> LoadImages(string directory)
        {
            var path = directory == null
                ? _imagesPath
                : Path.Combine(_imagesPath, directory);

            return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(x => x.EndsWith(".bmp"))
                .Select(x => (new Bitmap(x), Path.GetFileName(x)))
                .ToList();
        }
    }
}