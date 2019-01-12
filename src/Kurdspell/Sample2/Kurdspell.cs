using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kurdspell2;

namespace Sample2
{
    public class Kurdspell
    {
        private readonly List<Pattern> _patterns;
        private readonly Dictionary<char, List<Pattern>> _dictionary;

        public Kurdspell(string dictionaryPath)
        {
            _patterns = PatternsRepository.GetPatterns(dictionaryPath);
            _dictionary = new Dictionary<char, List<Pattern>>();

            foreach (var pattern in _patterns)
            {
                if (!_dictionary.ContainsKey(pattern.Template[0]))
                {
                    _dictionary[pattern.Template[0]] = new List<Kurdspell2.Pattern>();
                }

                _dictionary[pattern.Template[0]].Add(pattern);
            }
        }

        public bool Check(string word)
        {
            if (word == string.Empty)
                return true;

            var length = word.Length;
            var firstChar = length >= 1 ? word[0] : '\0';
            var secondChar = length >= 2 ? word[1] : '\0';
            var thirdChar = length >= 3 ? word[2] : '\0';
            var fourthChar = length >= 4 ? word[3] : '\0';
            var fifthChar = length >= 5 ? word[4] : '\0';

            if (_dictionary.TryGetValue(firstChar, out var patterns))
            {
                for (int i = 0; i < patterns.Count; i++)
                {
                    if (patterns[i].IsExactly(word, length, firstChar, secondChar, thirdChar, fourthChar, firstChar))
                        return true;
                }
            }

            return false;
        }

        public List<string> Suggest(string word, int count)
        {
            if (word == string.Empty)
                return new List<string>();

            if (_dictionary.TryGetValue(word[0], out var patterns))
            {
                return Pattern.GetTop(patterns, word, count);
            }

            return new List<string>();
        }
    }
}
