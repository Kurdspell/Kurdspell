using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunspellTest
{
    public static class SettingsHelper
    {
        private const string FilePath = "settings.txt";

        public static Settings GetSettings()
        {
            if (!File.Exists(FilePath))
                return null;

            var setting = new Settings();
            var lines = File.ReadAllLines(FilePath);

            foreach(var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    parts[0] = parts[0].Trim();

                    if (parts[0].Equals("Affix", StringComparison.OrdinalIgnoreCase))
                        setting.AffixFilePath = parts[1].Trim();
                    else if (parts[0].Equals("Dictionary", StringComparison.OrdinalIgnoreCase))
                        setting.DictionaryFilePath = parts[1].Trim();
                }
            }

            return setting;
        }

        public static void SaveSettings(Settings setting)
        {
            var lines = $"Affix = {setting.AffixFilePath}\nDictionary = {setting.DictionaryFilePath}";
            File.WriteAllText(FilePath, lines);
        }
    }

    public class Settings
    {
        public string AffixFilePath { get; set; }
        public string DictionaryFilePath { get; set; }
    }
}
