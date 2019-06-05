using Kurdspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            var spellChecker = SpellChecker.FromFile("ckb-IQ.txt");

            var entries = new List<Entry>();

            var words = spellChecker.GetWordList().ToList();

            var patternChars = spellChecker.GetPatterns().Select(p => p.Template).Sum(t => t.Count());
            var variantChars = words.Sum(w => w.Count());

            for (int i = 0; i < words.Count; i++)
            {
                var word = words[i];
                var entry = new Entry();
                entry.Correct = word;

                for (int m = 0; m < 3; m++)
                {
                    var misspelled = Misspell(word);
                    if (entry.Misspellings.Contains(misspelled))
                    {
                        m--;
                        continue;
                    }

                    entry.Misspellings.Add(misspelled);
                }

                entries.Add(entry);
            }

            double correct = 0;
            double correct3 = 0;
            double correct5 = 0;
            double total = 0;

            int count = 0;
            foreach (var entry in entries)
            {
                foreach (var misspelling in entry.Misspellings)
                {
                    total++;

                    var suggestions = spellChecker.Suggest(misspelling, 5);
                    if (suggestions.Count > 0 && suggestions[0] == entry.Correct)
                        correct++;
                    if (suggestions.Take(3).Any(s => s == entry.Correct))
                        correct3++;
                    if (suggestions.Take(5).Any(s => s == entry.Correct))
                        correct5++;
                }

                count++;
                if (count % 100 == 0)
                    Console.WriteLine(count);
            }

            Console.WriteLine($"Correct1: {correct / total * 100:N2}% of {total:N0} words.");
            Console.WriteLine($"Correct3: {correct3 / total * 100:N2}% of {total:N0} words.");
            Console.WriteLine($"Correct5: {correct5 / total * 100:N2}% of {total:N0} words.");
            Console.ReadLine();
        }

        private readonly static Random _random = new Random();
        private readonly static string _alphabet = "قوەرتیئحۆپاسدفگهژکلزخجڤبنمممڕشڵچ";
        private static string Misspell(string word)
        {
            var index = _random.Next(word.Length - 1) + 1;
            var randomLetter = _alphabet[_random.Next(_alphabet.Length)];

            var operation = _random.Next(3);
            if (operation == 0) // insert
            {
                return word.Insert(index, randomLetter.ToString());
            }
            else if (operation == 1) // delete
            {
                return word.Remove(index, 1);
            }
            else // replace
            {
                var builder = new StringBuilder(word);
                builder[index] = randomLetter;
                return builder.ToString();
            }
        }
    }

    class Entry
    {
        public string Correct { get; set; }
        public HashSet<string> Misspellings { get; set; } = new HashSet<string>();
    }
}
