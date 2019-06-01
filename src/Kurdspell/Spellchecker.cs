using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kurdspell
{
    public class SpellChecker
    {
        private const string Information = "information";
        private const string Affixes = "affixes";
        private const string Patterns = "patterns";

        private const string SectionPrefix = "~ ";
        private const string InformationSectionName = SectionPrefix + Information;
        private const string AffixesSectionName = SectionPrefix + Affixes;
        private const string PatternsSectionName = SectionPrefix + Patterns;

        private enum ParseSection
        {
            None,
            Information,
            Affixes,
            Patterns,
        }

        private readonly List<Pattern> _patterns;
        private readonly Dictionary<string, Affix> _affixes;
        private readonly Dictionary<char, List<Pattern>> _dictionary;

        public SpellChecker(List<Pattern> patterns, List<Affix> affixes, Dictionary<string, string> properties = null)
        {
            _patterns = patterns;
            _affixes = affixes.ToDictionary(a => a.Name);
            _dictionary = new Dictionary<char, List<Pattern>>();
            Properties = properties ?? new Dictionary<string, string>();

            foreach (var pattern in _patterns)
            {
                AddPattern(pattern);
            }
        }

        public IReadOnlyList<Pattern> GetPatterns() => _patterns;

        public IReadOnlyDictionary<string, Affix> GetAffixes() => _affixes;

        public IEnumerable<string> GetWordList()
        {
            return _patterns.SelectMany(p => p.GetVariants(_affixes));
        }

        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public bool Check(string word)
        {
            if (word == string.Empty)
                return true;

            var length = word.Length;
            var secondChar = length >= 2 ? word[1] : '\0';
            var thirdChar = length >= 3 ? word[2] : '\0';
            var fourthChar = length >= 4 ? word[3] : '\0';
            var fifthChar = length >= 5 ? word[4] : '\0';

            if (_dictionary.TryGetValue(word[0], out var patterns))
            {
                bool found = false;

                Parallel.For(0, patterns.Count, (i, state) =>
                {
                    if (patterns[i].IsExactly(word, length, secondChar, thirdChar, fourthChar, fifthChar, _affixes))
                    {
                        found = true;
                        state.Break();
                    }
                });

                return found;
            }

            return false;
        }

        public List<string> Suggest(string word, int count)
        {
            if (word == string.Empty)
                return new List<string>();

            word = word.ToLower();

            if (_dictionary.TryGetValue(word[0], out var patterns))
            {
                return Pattern.GetTop(patterns, word, count, _affixes);
            }

            return new List<string>();
        }

        public void AddToDictionary(string word)
        {
            var pattern = new Pattern(word);
            _patterns.Add(pattern);
            AddPattern(pattern);
        }

        private void AddPattern(Pattern pattern)
        {
            if (!_dictionary.ContainsKey(pattern.Template[0]))
            {
                _dictionary[pattern.Template[0]] = new List<Pattern>();
            }

            _dictionary[pattern.Template[0]].Add(pattern);
        }

        #region Parsing Dictionary
        // Async
        public static async Task<SpellChecker> FromFileAsync(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return await FromStreamAsync(stream);
            }
        }

        public static async Task<SpellChecker> FromStreamAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await FromReaderAsync(reader);
            }
        }

        public static async Task<SpellChecker> FromReaderAsync(TextReader reader)
        {
            var patterns = new List<Pattern>();
            var affixes = new List<Affix>();

            var properties = new Dictionary<string, string>();

            var section = ParseSection.None;

            while (reader.Peek() != -1)
            {
                var current = await reader.ReadLineAsync();
                section = ParseLine(current, section, properties, affixes, patterns);
            }

            return new SpellChecker(patterns, affixes, properties);
        }

        // Sync
        public static SpellChecker FromFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return FromStream(stream);
            }
        }

        public static SpellChecker FromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return FromReader(reader);
            }
        }

        public static SpellChecker FromString(string content)
        {
            using (var reader = new StringReader(content))
            {
                return FromReader(reader);
            }
        }

        public static SpellChecker FromReader(TextReader reader)
        {
            var patterns = new List<Pattern>();
            var affixes = new List<Affix>();

            var properties = new Dictionary<string, string>();

            var section = ParseSection.None;

            while (reader.Peek() != -1)
            {
                var current = reader.ReadLine();
                section = ParseLine(current, section, properties, affixes, patterns);
            }

            return new SpellChecker(patterns, affixes, properties);
        }

        private static ParseSection ParseLine(
            string current,
            ParseSection section,
            Dictionary<string, string> properties,
            List<Affix> affixes,
            List<Pattern> patterns)
        {
            if (current.StartsWith("~"))
            {
                var index = 1;
                while (index > current.Length)
                {
                    index++;

                    if (current[index] != ' ')
                        break;
                }

                var sectionName = current.Substring(index);
                switch (sectionName.Trim().ToLowerInvariant())
                {
                    case Information:
                        section = ParseSection.Information;
                        break;
                    case Affixes:
                        section = ParseSection.Affixes;
                        break;
                    case Patterns:
                        section = ParseSection.Patterns;
                        break;
                }
            }
            else
            {
                switch (section)
                {
                    case ParseSection.Information:
                        {
                            // Sample Property:
                            // Name : Value

                            var parts = current.Split(':');
                            if (parts.Length == 2)
                            {
                                properties.Add(parts[0].Trim(), parts[1].Trim());
                            }
                        }
                        break;
                    case ParseSection.Affixes:
                        {
                            var parts = current.Split(':');
                            if (parts.Length == 2)
                            {
                                var name = parts[0].Trim().ToLowerInvariant(); // Affix names are case-insensitive
                                var variants = parts[1].Split(',').Select(v => v.Trim()).ToArray();
                                affixes.Add(new Affix(name, variants));
                            }
                        }
                        break;
                    case ParseSection.Patterns:
                        // TODO: Make sure all of the affixes are loaded before the patterns
                        patterns.Add(new Pattern(current));
                        break;
                }
            }

            return section;
        }
        #endregion

        #region Persisting Dictionary
        // Async
        public async Task SaveAsync(string path)
        {
            using (var stream = File.OpenWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                stream.SetLength(0);

                await SaveAsync(writer);
            }
        }

        public async Task SaveAsync(TextWriter writer)
        {
            await writer.WriteLineAsync(InformationSectionName);
            foreach (var prop in Properties)
            {
                await writer.WriteAsync(prop.Key);
                await writer.WriteAsync(": ");
                await writer.WriteLineAsync(prop.Value);
            }

            await writer.WriteLineAsync();
            await writer.WriteLineAsync(AffixesSectionName);

            foreach (var affix in _affixes.Values)
            {
                await writer.WriteAsync(affix.Name);
                await writer.WriteAsync(": ");
                await writer.WriteLineAsync(string.Join(",", affix.Values));
            }

            await writer.WriteLineAsync();
            await writer.WriteLineAsync(PatternsSectionName);

            foreach (var pattern in _patterns)
            {
                await writer.WriteLineAsync(pattern.Template);
            }
        }

        // Sync
        public void Save(string path)
        {
            using (var stream = File.OpenWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                stream.SetLength(0);

                Save(writer);
            }
        }
        public void Save(TextWriter writer)
        {
            writer.WriteLine(InformationSectionName);
            foreach (var prop in Properties)
            {
                writer.Write(prop.Key);
                writer.Write(": ");
                writer.WriteLine(prop.Value);
            }

            writer.WriteLine();
            writer.WriteLine(AffixesSectionName);

            foreach (var affix in _affixes.Values)
            {
                writer.Write(affix.Name);
                writer.Write(": ");
                writer.WriteLine(string.Join(",", affix.Values));
            }

            writer.WriteLine();
            writer.WriteLine(PatternsSectionName);

            foreach (var pattern in _patterns)
            {
                writer.WriteLine(pattern.Template);
            }
        }
        #endregion
    }
}
