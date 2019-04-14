using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kurdspell2
{
    public class PatternsRepository
    {
        public static List<Pattern> GetPatterns(string path)
        {
            var rules = new List<Rule>
            {
                new Rule(new string[] { "m", "man", "t", "tan", "y", "yan" }),
                new Rule(new string[] { "m", "in", "a", "n", "et", "n" }),
                new Rule(new string[] { "om", "oyn", "oyt", "on", "wat", "on" }),
                new Rule(new string[] { "m", "in", "t", "n", "et", "n" }),
                new Rule(new string[] { "yaka", "yek", "akan", "yan", "" }),
                new Rule(new string[] { "ish", "" }),
                new Rule(new string[] { "m", "yn", "at", "an", "an", "an" }),
            };

            return File.ReadAllLines(path)
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .Select(l => new Pattern(l, rules))
                       .ToList();
        }

        private static readonly Random _random = new Random(42);
        private static string GetRandomPattern(int maxRule)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var length = _random.Next(5, 12);
            var stringChars = new char[8];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }

            var word = new String(stringChars);
            var times = _random.Next(1, 3);

            for (int i = 0; i < times; i++)
            {
                int index;
                do
                {
                    index = _random.Next(0, word.Length - 1);
                }
                while (word[GetPreviousOrFirst(index)] == '{' || word[GetPreviousOrFirst(index)] == '}' || char.IsNumber(word[GetPreviousOrFirst(index)]));

                var rule = _random.Next(0, maxRule + 1);
                word = word.Substring(0, GetPreviousOrFirst(index)) + "{" + rule + "}" + word.Substring(index + 1, word.Length - index - 1);
            }

            return word;

            int GetPreviousOrFirst(int index)
            {
                if (index == 0)
                    return index;
                return index - 1;
            }
        }
    }
}
