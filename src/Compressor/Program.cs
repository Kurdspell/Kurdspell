using Kurdspell;
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

            var affixGroups = File.ReadAllLines("affixes.txt").Select(w => w.Split(',')).ToList();
            var cleanAffixes = affixGroups.Select(r =>
                r.Where(c => !string.IsNullOrWhiteSpace(c))
                 .Select(i => i.Trim())
                 .ToList()
            ).ToList();

            var affixes = cleanAffixes.SelectMany(r => r)
                                    .Where(r => r.Length > 1)
                                    .OrderByDescending(r => r.Length)
                                    .Distinct()
                                    .ToList();

            var acutalAffixes = affixGroups.Select((items, i) => new Affix(items)).ToList();

            foreach (var word in words)
            {
                word.Compressed = word.Text;
                foreach (var affix in affixes)
                {
                    var changed = word.Compressed.Replace(affix, string.Empty);
                    if (changed != word.Compressed)
                    {
                        word.Compressed = changed;
                        int index = 0;
                        while (index < word.Text.Length &&
                              (index = word.Text.IndexOf(affix, index)) > 0)
                        {
                            word.Affixes.Add(new AffixPossibility(index, affix));
                            index += affix.Length;
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

            var patternList = new List<string>();
            foreach (var group in groups)
            {
                //var builder = new StringBuilder(group.Key);
                var variants = group.ToList();
                var longest = variants.OrderByDescending(v => v.Affixes.Count).First();

                longest.Affixes = longest.Affixes.OrderBy(a => a.Index).ToList();

                var previous = longest.Affixes.FirstOrDefault();
                for (int i = 1; i < longest.Affixes.Count; i++)
                {
                    var current = longest.Affixes[i];
                    if (current.Index < previous.Index + previous.Affix.Length)
                    {
                        longest.Affixes.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        previous = current;
                    }
                }

                var list = new List<List<string>>();
                int start = 0;
                for (int i = 0; i < longest.Affixes.Count; i++)
                {
                    var count = variants.SkipWhile(w => w == longest)
                                        .Sum(w => w.Affixes.Count(a => a.Affix == longest.Affixes[i].Affix));

                    var significatOccurance = variants.Count * 0.8;

                    if (count > significatOccurance)
                    {
                        var index = longest.Affixes[i].Index + longest.Affixes[i].Affix.Length;
                        list.Add(new List<string> { longest.Text.Substring(start, index - start) });
                        start = index;
                    }
                    else
                    {
                        var index = longest.Affixes[i].Index;

                        if (index > start)
                        {
                            list.Add(new List<string> { longest.Text.Substring(start, index - start) });
                            start = index + longest.Affixes[i].Affix.Length;
                        }

                        var matchingAffixes = affixGroups
                            .Select((g, x) => new { Affixes = g, Index = x })
                            .Where(g => g.Affixes.Contains(longest.Affixes[i].Affix))
                            .Select(g => $"{{{g.Index}}}")
                            .ToList();

                        list.Add(matchingAffixes);
                    }
                }

                var patterns = Kurdspell.CartesianProduct.Linq(list);

                Pattern p = default(Pattern);
                int max = 0;
                foreach (var parts in patterns)
                {
                    var pattern = new Pattern(string.Join("", parts));
                    int count = 0;
                    foreach (var variant in variants)
                    {
                        if (pattern.IsExactly(variant.Text, acutalAffixes))
                            count++;
                    }

                    if (max == 0 || count > max)
                    {
                        max = count;
                        p = pattern;
                    }
                }

                if (max > 0)
                {
                    patternList.Add(p.Template);
                }
            }

            Console.WriteLine($"Patterns: {patternList.Count:N0}");

            using (var file = File.OpenWrite("patterns.txt"))
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                file.SetLength(0);

                foreach (var pattern in patternList)
                {
                    writer.WriteLine(pattern);
                }
            }
        }
    }

    class Word
    {
        public string Text { get; set; }
        public int Frequency { get; set; }
        public string Compressed { get; set; }
        public List<AffixPossibility> Affixes { get; set; } = new List<AffixPossibility>();
    }

    public class AffixPossibility
    {
        public AffixPossibility(int index, string affix)
        {
            Index = index;
            Affix = affix;
        }

        public int Index { get; }
        public string Affix { get; }
    }
}