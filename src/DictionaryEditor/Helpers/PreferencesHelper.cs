using DictionaryEditor.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DictionaryEditor.Helpers
{
    static class PreferencesHelper
    {
        const string EditorPath = "Kurdspell\\Editor";
        const string FileName = "preferences.json";

        private static string GetLocalFolder()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, EditorPath);
        }

        private static string GetPreferencesFile() => Path.Combine(GetLocalFolder(), FileName);

        public static Preferences GetPreferences()
        {
            var file = GetPreferencesFile();

            if (File.Exists(file))
            {
                return JsonConvert.DeserializeObject<Preferences>(File.ReadAllText(file));
            }

            return new Preferences
            {
                RecentFiles = new List<string>(),
                RightToLeft = true,
            };
        }

        public static void SavePreferences(Preferences preferences)
        {
            if (!Directory.Exists(GetLocalFolder()))
            {
                Directory.CreateDirectory(GetLocalFolder());
            }

            var json = JsonConvert.SerializeObject(preferences);
            File.WriteAllText(GetPreferencesFile(), json);
        }
    }
}
