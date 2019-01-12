using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using Kurdspell;

namespace Sample
{
    public class Benchmarks
    {
        private static readonly SpellChecker _kurdspell;
        private static readonly List<string> _words = new List<string>
        {
            "dexom",
            "bimbexshe",
            "mamostakanm",
            "sldfjksj",
            "sdkflj",
        };

        static Benchmarks()
        {
            _kurdspell = new SpellChecker("");
        }

        [Benchmark]
        public void Check()
        {
            for (int i = 0; i < _words.Count; i++)
            {
                _kurdspell.Check(_words[i]);
            }
        }

        [Benchmark]
        public void Suggest()
        {
            for (int i = 0; i < _words.Count; i++)
            {
                _kurdspell.Suggest(_words[i], 5);
            }
        }

        [Benchmark]
        public void Construct()
        {
            var kurdspell = new SpellChecker("");
        }
    }
}
