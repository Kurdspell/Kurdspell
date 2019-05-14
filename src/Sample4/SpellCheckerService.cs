using Kurdspell;
using Sample4.Controllers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sample4
{
    public class SpellCheckerService
    {
        private readonly ConcurrentDictionary<string, List<string>> _suggestionsCache
                 = new ConcurrentDictionary<string, List<string>>();

        private readonly ConcurrentDictionary<string, bool> _isvalidCache
            = new ConcurrentDictionary<string, bool>();

        private SpellChecker _spellchecker;
        private const string Path = "ckb-IQ.txt";

        public SpellCheckerService()
        {
            Dictionary = File.ReadAllText(Path);
            _spellchecker = new SpellChecker(Path);
        }

        public bool Check(string word)
            => _isvalidCache.GetOrAdd(word, w => _spellchecker.Check(w));

        public List<string> Suggest(string word)
            => _suggestionsCache.GetOrAdd(word, w => _spellchecker.Suggest(w, 3));

        public string Dictionary { get; }

        public IReadOnlyList<Word> Tokenize(string text)
        {
            var list = new List<Word>();

            int i = 0;
            int start = 0;

            var breakpoints = new char[] { ' ', '\n', '!', '?', ',', '؟', '،' };

            while (i < text.Length)
            {
                if (breakpoints.Contains(text[i]))
                {
                    var word = new Word(text.Substring(start, i - start), new Range(start, i - 1));
                    list.Add(word);
                    start = i + 1;
                }

                i++;
            }

            if (start < text.Length)
            {
                var word = new Word(text.Substring(start, text.Length - start), new Range(start, text.Length - 1));
                list.Add(word);
            }

            return list;
        }
    }

    public struct Word
    {
        public Word(string value, Range range) : this()
        {
            Value = value;
            Range = range;
        }

        public string Value { get; }
        public Range Range { get; }
    }
}
