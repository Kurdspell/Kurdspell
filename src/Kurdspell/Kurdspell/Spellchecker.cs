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
            _patterns = PatternsRepository.GetPatterns();
            _rules = PatternsRepository.GetRules();
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

            if (_dictionary.TryGetValue(word[0], out var patterns))
            {
                return Pattern.GetTop(patterns, word, count, _rules);
            }

            return new List<string>();
        }
    }
}
