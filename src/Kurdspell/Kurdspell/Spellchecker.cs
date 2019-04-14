using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurdspell
{
    public class SpellChecker
    {
        private enum ParseSection
        {
            None,
            Information,
            Rules,
            Patterns,
        }

        private readonly List<Pattern> _patterns;
        private readonly List<Rule> _rules;
        private readonly Dictionary<char, List<Pattern>> _dictionary;

        public SpellChecker(List<Pattern> patterns, List<Rule> rules)
        {
            _patterns = patterns;
            _rules = rules;
            _dictionary = new Dictionary<char, List<Pattern>>();

            foreach (var pattern in _patterns)
            {
                if (!_dictionary.ContainsKey(pattern.Template[0]))
                {
                    _dictionary[pattern.Template[0]] = new List<Pattern>();
                }

                _dictionary[pattern.Template[0]].Add(pattern);
            }
        }

        public SpellChecker(string dictionaryPath)
        {
            _patterns = new List<Pattern>();
            _rules = new List<Rule>();

            var rules = new List<Tuple<int, Rule>>();

            using (var stream = File.OpenRead(dictionaryPath))
            using (var reader = new StreamReader(stream, Encoding.Unicode, true))
            {
                var section = ParseSection.None;

                while (!reader.EndOfStream)
                {
                    var current = reader.ReadLine();
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
                            case "information":
                                section = ParseSection.Information;
                                break;
                            case "rules":
                                section = ParseSection.Rules;
                                break;
                            case "patterns":
                                section = ParseSection.Patterns;
                                break;

                        }
                    }
                    else
                    {
                        switch (section)
                        {
                            case ParseSection.Information:
                                // Ignore for now
                                break;
                            case ParseSection.Rules:
                                var parts = current.Split(':');
                                if (parts.Length == 2 && int.TryParse(parts[0], out var number))
                                {
                                    var variants = parts[1].Split(',').Select(v => v.Trim()).ToArray();
                                    rules.Add(new Tuple<int, Rule>(number, new Rule(variants)));
                                }

                                break;
                            case ParseSection.Patterns:
                                // TODO: Make sure all of the rules are loaded before the patterns
                                _patterns.Add(new Pattern(current));
                                break;
                        }
                    }
                }
            }

            _rules = rules.OrderBy(t => t.Item1).Select(t => t.Item2).ToList();

            _dictionary = new Dictionary<char, List<Pattern>>();

            foreach (var pattern in _patterns)
            {
                if (string.IsNullOrWhiteSpace(pattern.Template))
                    continue;

                if (!_dictionary.ContainsKey(pattern.Template[0]))
                {
                    _dictionary[pattern.Template[0]] = new List<Pattern>();
                }

                _dictionary[pattern.Template[0]].Add(pattern);
            }
        }

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
                    if (patterns[i].IsExactly(word, length, secondChar, thirdChar, fourthChar, fifthChar, _rules))
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
                return Pattern.GetTop(patterns, word, count, _rules);
            }

            return new List<string>();
        }
    }
}
