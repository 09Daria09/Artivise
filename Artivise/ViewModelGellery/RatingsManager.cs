using Artivise.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Artivise.ViewModelGellery
{
    public static class RatingsManager
    {
        private static string GetRatingsFilePath(string category)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appName = "Artivise";
            string categoryFolder = Path.Combine(appDataFolder, appName, category);

            if (!Directory.Exists(categoryFolder))
            {
                Directory.CreateDirectory(categoryFolder);
            }

            return Path.Combine(categoryFolder, "ratings.json");
        }

        public static void SaveRatingsToFile(List<ImageData> images, string category)
        {
            var filePath = GetRatingsFilePath(category);
            var ratingsToSave = images.Select(img => new
            {
                img.ImagePath,
                Ratings = img.Ratings
            }).ToList();

            var json = JsonConvert.SerializeObject(ratingsToSave, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static void LoadRatingsFromFile(List<ImageData> images, string category)
        {
            var filePath = GetRatingsFilePath(category);
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var loadedRatings = JsonConvert.DeserializeObject<List<ImageData>>(json);

                foreach (var loadedImage in loadedRatings)
                {
                    var image = images.FirstOrDefault(i => i.ImagePath == loadedImage.ImagePath);
                    if (image != null)
                    {
                        image.Ratings = loadedImage.Ratings;
                    }
                }
            }
        }
    }
}
