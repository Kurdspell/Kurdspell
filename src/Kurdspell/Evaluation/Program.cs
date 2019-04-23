using Kurdspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            var spellChecker = new SpellChecker("ckb-IQ.txt");
            var lines = File.ReadAllLines("test-1.txt");

            var entries = new List<Entry>();

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                var entry = new Entry();
                entry.Correct = parts[0];
                entry.Misspellings = parts[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                entries.Add(entry);
            }

            double correct = 0;
            double correct3 = 0;
            double correct5 = 0;
            double total = 0;

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
            }

            Console.WriteLine($"Correct1: {correct / total * 100:N2}% of {total:N0} words.");
            Console.WriteLine($"Correct3: {correct3 / total * 100:N2}% of {total:N0} words.");
            Console.WriteLine($"Correct5: {correct5 / total * 100:N2}% of {total:N0} words.");
        }
    }

    class Entry
    {
        public string Correct { get; set; }
        public string[] Misspellings { get; set; }
    }
}
