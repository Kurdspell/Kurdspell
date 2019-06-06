using System.Collections.Generic;

namespace DictionaryEditor.Models
{
    public class Preferences
    {
        public List<string> RecentFiles { get; set; }
        public bool RightToLeft { get; set; }

        public void AddPathToRecentFiles(string path)
        {
            RecentFiles.Remove(path);
            RecentFiles.Insert(0, path);

            if (RecentFiles.Count > 9)
            {
                RecentFiles.RemoveRange(9, RecentFiles.Count - 9);
            }
        }
    }
}
