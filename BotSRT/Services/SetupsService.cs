using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BotSRT.Services
{
    public class SetupsService
    {
        private const int ITEMS_TO_SHOW = 4;

        public int ItemsToShow
        {
            get { return ITEMS_TO_SHOW; }
        }

        public List<string> GetNextDirectories(string currentPath)
        {
            var result = new List<string>();
            if (Directory.Exists(currentPath))
            {
                var directories = Directory.GetDirectories(currentPath);
                foreach (var directory in directories)
                {
                    result.Add(Path.GetFileName(directory));
                }
            }

            return result.OrderBy(x => x).ToList();
        }

        public string GetDirectoriesToShow(List<string> nextDirectories, int startIndex)
        {
            int index = 0;
            var result = "";
            var parseItemsToShow = startIndex + ITEMS_TO_SHOW >= nextDirectories.Count
                ? nextDirectories.Count - startIndex
                : ITEMS_TO_SHOW;
            foreach (var directory in nextDirectories.GetRange(startIndex, parseItemsToShow))
            {
                result = string.Concat(result, $"{++index}.- {directory.ToUpperInvariant()}\n");
            }
            return result;
        }

        public string CompleteSetupPath(string path)
        {
            // Windows
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            // Unix
            path = path.Replace('/', Path.DirectorySeparatorChar);

            path = Path.Combine(@"Setups", path);
            var lenght = path.Split(Path.DirectorySeparatorChar).Length;
            if (lenght < 4)
            {
                path = Path.Combine(path, Environment.GetEnvironmentVariable("CurrentSeason"));
            }

            return path;
        }
    }
}
