﻿using BenchmarkDotNet.Attributes;
using Kurdspell;
using System.Collections.Generic;

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
            _kurdspell = new SpellChecker(PatternsRepository.GetPatterns(), PatternsRepository.GetAffixes());
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
            var kurdspell = new SpellChecker(PatternsRepository.GetPatterns(), PatternsRepository.GetAffixes());
        }
    }
}
