using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Compresser
{
    class Program
    {
        static void Main(string[] args)
        {
            var words = File.ReadAllLines("words.txt").Select(w => w.Split('\t')).Select(parts =>
                new Word
                {
                    Text = parts[0],
                    Frequency = int.Parse(parts[1])
                }).ToList();

            var affixGroups = File.ReadAllLines("rules.txt").Select(w => w.Split(',')).ToList();
            var cleanRules = affixGroups.Select(r =>
                r.Where(c => !string.IsNullOrWhiteSpace(c))
                 .Select(i => i.Trim())
                 .ToList()
            ).ToList();

            var affixes = cleanRules.SelectMany(r => r)
                                    .Where(r => r.Length > 1)
                                    .OrderByDescending(r => r.Length)
                                    .Distinct()
                                    .ToList();

            foreach (var word in words)
            {
                word.Compressed = word.Text;
                foreach (var rule in affixes)
                {
                    var changed = word.Compressed.Replace(rule, string.Empty);
                    if (changed != word.Compressed)
                    {
                        word.Compressed = changed;
                        int index = 0;
                        while (index < word.Text.Length &&
                              (index = word.Text.IndexOf(rule, index)) > 0)
                        {
                            word.Affixes.Add(index);
                            index++;
                        }
                    }
                }
            }

            var groups = words.GroupBy(w => w.Compressed)
                              .Where(g => g.Count() > 1)
                              .ToList();

            Console.WriteLine($"Groups: {groups.Count:N0}");

            using (var file = File.OpenWrite("output.txt"))
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                file.SetLength(0);

                foreach (var group in groups)
                {
                    writer.WriteLine($"{group.Key}: {group.Sum(w => w.Frequency):N0}");
                    writer.WriteLine("-------------");
                    foreach (var item in group)
                    {
                        writer.WriteLine(item.Text);
                    }

                    writer.WriteLine();
                }
            }

            foreach (var group in groups)
            {
                var builder = new StringBuilder(group.Key);
                var variants = group.ToList();
                var longest = variants.OrderByDescending(v => v.Affixes.Count).First();

                var parts = new List<string>();
                int start = 0;
                foreach(var affix in longest.Affixes)
                {
                    parts.Add(longest.Text.Substring(start, affix));
                }

                //for (int i = 0; i < builder.Length; i++)
                //{
                //    foreach (var variant in variants)
                //    {
                //        if (builder[i] != variant.Text[i])
                //        {

                //        }
                //    }
                //}
            }
        }
    }

    class Word
    {
        public string Text { get; set; }
        public int Frequency { get; set; }
        public string Compressed { get; set; }
        public List<int> Affixes { get; set; } = new List<int>();
    }

    public class AffixPossibility
    {
        public int Index { get; set; }
        public int Rule { get; set; }
    }
}