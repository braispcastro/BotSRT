using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BotSRT.Services
{
    public class SetupsService
    {
        private const int ITEMS_TO_SHOW = 4;

        public int ItemsToShow
        {
            get { return ITEMS_TO_SHOW; }
        }

        public IEnumerable<string> GetNextDirectories(string currentPath)
        {
            if (Directory.Exists(currentPath))
            {
                var directories = Directory.GetDirectories(currentPath);
                foreach (var directory in directories)
                {
                    yield return Path.GetFileName(directory);
                }
            }
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
    }
}
