using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurdspell
{
    public struct Pattern
    {
        public Pattern(string template)
        {
            Template = template.ToLower();

            var dictionary = new Dictionary<int, int>();
            var parts = new List<object>();

            int index = 0;
            int start = 0;

            while (index < Template.Length)
            {
                if (Template[index] == '{')
                {
                    if (start != index)
                    {
                        var current = Template.Substring(start, index - start);
                        parts.Add(current);
                    }

                    start = index;
                    while (Template[++index] != '}')
                    {
                        if (index == Template.Length - 1)
                            throw new ArgumentException($"Expected '}}'. Template: {Template}");
                    }

                    var text = Template.Substring(start, index - start + 1);
                    if (!int.TryParse(text.Substring(1, text.Length - 2), out var number))
                    {
                        throw new ArgumentException("Rule names must be numbers");
                    }

                    parts.Add(number);

                    start = index + 1;
                }
                else if (index == template.Length - 1 && Template[index] != '}')
                {
                    parts.Add(Template.Substring(start, index + 1 - start));
                }

                index++;
            }

            Parts = parts;

            _firstChar = Template.Length >= 1 ? Template[0] : '\0';
            _secondChar = Template.Length >= 2 ? Template[1] : '\0';
            _thirdChar = Template.Length >= 3 ? Template[2] : '\0';
            _fourthChar = Template.Length >= 4 ? Template[3] : '\0';
            _fifthChar = Template.Length >= 5 ? Template[4] : '\0';

            _length = (byte)Template.Length;
        }

        public readonly string Template;

        public IReadOnlyList<object> Parts { get; }

        private readonly char _firstChar;
        private readonly char _secondChar;
        private readonly char _thirdChar;
        private readonly char _fourthChar;
        private readonly char _fifthChar;
        private readonly byte _length;

        public bool IsExactly(string word, int wLength, char wSecondChar, char wThirdChar, char wFourthChar, char wFifthChar, IReadOnlyList<Rule> rules, int partIndex = 1, int charIndex = 0)
        {
            if (charIndex == 0)
            {
                bool cont = true;
                if (cont)
                {
                    if (_secondChar == '\0')
                    {
                        return wLength == 1;
                    }
                    else if (_secondChar == '{')
                    {
                        cont = false;
                    }
                    else
                    {
                        if (wSecondChar != _secondChar)
                            return false;
                    }

                    charIndex = 1;
                }

                if (cont)
                {
                    if (_thirdChar == '\0')
                    {
                        return wLength == 2;
                    }
                    else if (_thirdChar == '{')
                    {
                        charIndex = 2;
                        cont = false;
                    }
                    else
                    {
                        if (wThirdChar != _thirdChar)
                            return false;
                    }
                    charIndex = 2;
                }

                if (cont)
                {
                    if (_fourthChar == '\0')
                    {
                        return wLength == 3;
                    }
                    else if (_fourthChar == '{')
                    {
                        charIndex = 3;
                        cont = false;
                    }
                    else
                    {
                        if (wFourthChar != _fourthChar)
                            return false;
                    }
                    charIndex = 3;
                }

                if (cont)
                {
                    if (_fifthChar == '\0')
                    {
                        return wLength == 4;
                    }
                    else if (_fifthChar == '{')
                    {
                        cont = false;
                    }
                    else
                    {
                        if (wFifthChar != _fifthChar)
                            return false;
                    }

                    charIndex = 4;
                }

                if (cont)
                {
                    for (; charIndex < Template.Length; charIndex++)
                    {
                        var current = Template[charIndex];
                        if (current == '{')
                        {
                            break;
                        }

                        if (charIndex >= wLength)
                            return false;

                        if (current != word[charIndex])
                        {
                            return false;
                        }
                    }
                }
            }

            List<int> matches = null;

            for (; partIndex < Parts.Count; partIndex++)
            {
                var p = Parts[partIndex];

                if (p is int rule)
                {
                    if (matches == null)
                        matches = new List<int>();
                    else
                        matches.Clear();

                    var variants = rules[rule].Values;
                    var shouldGoOn = false;

                    int goodLength = 0;

                    for (int j = 0; j < variants.Length; j++)
                    {
                        var variant = variants[j];

                        if (CanBeTheSame(variant, variant.Length, word, wLength, charIndex))
                        {
                            shouldGoOn = true;
                            if (goodLength < variant.Length)
                                goodLength = variant.Length;
                            matches.Add(j);
                        }
                    }

                    if (matches.Count > 1 && partIndex != Parts.Count - 1)
                    {
                        shouldGoOn = false;
                        foreach (var match in matches)
                        {
                            shouldGoOn = IsExactly(word, wLength, wSecondChar, wThirdChar, wFourthChar, wFifthChar, rules, partIndex + 1, charIndex + variants[match].Length);

                            if (shouldGoOn)
                                return true;
                        }

                        return false;
                    }

                    if (shouldGoOn == false)
                    {
                        return false;
                    }
                    else
                    {
                        charIndex += goodLength;
                    }
                }
                else
                {
                    var part = p as string;
                    var length = part.Length;

                    if (!CanBeTheSame(part, length, word, wLength, charIndex))
                    {
                        return false;
                    }

                    charIndex += length;
                }
            }

            return charIndex == wLength;
        }

        public bool IsExactly(string word, IReadOnlyList<Rule> rules)
        {
            var secondChar = word.Length > 1 ? word[1] : '\0';
            var thirdChar = word.Length > 2 ? word[2] : '\0';
            var fourthChar = word.Length > 3 ? word[3] : '\0';
            var fifthChar = word.Length > 4 ? word[4] : '\0';

            return IsExactly(word, word.Length, secondChar, thirdChar, fourthChar, fifthChar, rules);
        }

        public IEnumerable<string> GetVariants(IReadOnlyList<Rule> rules)
        {
            return Explode(rules).Select(parts => string.Join("", parts));
        }

        private static bool CanBeTheSame(string variant, int vLength, string text, int tLength, int charIndex)
        {
            if (charIndex + vLength > tLength)
                return false;

            for (int i = 0; i < vLength; i++)
            {
                if (variant[i] != text[i + charIndex])
                    return false;
            }

            return true;
        }

        private static bool CanBeTheSame(StringBuilder builder, string text)
        {
            if (builder.Length > text.Length)
                return false;

            for (int i = 0; i < builder.Length; i++)
            {
                if (builder[i] != text[i])
                    return false;
            }

            return true;
        }

        public bool IsCloseEnough(string word, IReadOnlyList<Rule> rules, StringBuilder builder = null, int i = 0)
        {
            builder = builder ?? new StringBuilder(word.Length);
            var matches = new List<int>(10);

            for (; i < Parts.Count; i++)
            {
                var p = Parts[i];

                if (p is int rule)
                {
                    matches.Clear();

                    var variants = rules[rule].Values;
                    var shouldGoOn = false;

                    for (int j = 0; j < variants.Length; j++)
                    {
                        var variant = variants[j];

                        builder.Append(variant);
                        var tLength = i == Parts.Count - 1 ? word.Length : Math.Min(builder.Length, word.Length);

                        if (Levenshtein.GetDistanceTwoRows(builder, word, tLength) <= Constants.ThresholdPlusOne)
                        {
                            shouldGoOn = true;
                            matches.Add(j);
                        }

                        builder.Remove(builder.Length - variant.Length, variant.Length);
                    }

                    if (matches.Count > 1 && i != Parts.Count - 1)
                    {
                        shouldGoOn = false;
                        foreach (var match in matches)
                        {
                            var originalLength = builder.Length;
                            builder.Append(variants[match]);

                            shouldGoOn = IsCloseEnough(word, rules, builder, i + 1);
                            builder.Remove(originalLength, builder.Length - originalLength);

                            if (shouldGoOn)
                                return true;
                        }

                        return false;
                    }

                    if (shouldGoOn == false)
                    {
                        return false;
                    }
                }
                else
                {
                    builder.Append(p as string);

                    if (builder.Length < Constants.ThresholdPlusOne)
                        continue;

                    var tLength = i == Parts.Count - 1 ? word.Length : Math.Min(builder.Length, word.Length);

                    if (Levenshtein.GetDistanceTwoRows(builder, word, tLength) > Constants.Threshold)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public IEnumerable<IEnumerable<string>> Explode(IReadOnlyList<Rule> rules)
        {
            var sets = new List<IEnumerable<string>>();
            for (int i = 0; i < Parts.Count; i++)
            {
                IEnumerable<string> set;
                if (Parts[i] is int rule)
                {
                    set = rules[rule].Values;
                }
                else
                {
                    set = Enumerable.Repeat(Parts[i] as string, 1);
                }

                sets.Add(set);
            }

            return CartesianProduct.Linq(sets).ToArray();
        }

        private List<string> Explode(string prefix, IReadOnlyList<object> parts, IReadOnlyList<Rule> rules)
        {
            var list = new List<string>();

            var builder = new StringBuilder(prefix, Template.Length);

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] is int rule)
                {
                    var values = rules[rule].Values;
                    foreach (var value in values)
                    {
                        builder.Append(value);

                        if (i < parts.Count - 1)
                        {
                            var newPrefix = builder.ToString();
                            list.AddRange(Explode(newPrefix, parts.Skip(i + 1).ToList(), rules));
                        }
                        else
                        {
                            list.Add(builder.ToString());
                        }

                        builder.Remove(builder.Length - value.Length, value.Length);
                    }
                }
                else
                {
                    builder.Append(parts[i] as string);
                }
            }

            return list;
        }

        public static List<string> GetTop(IReadOnlyList<Pattern> patterns, string word, int n, IReadOnlyList<Rule> rules)
        {
            ConcurrentBag<Pattern> closePatterns = new ConcurrentBag<Pattern>();
            Parallel.For(0, patterns.Count, i =>
            {
                if (patterns[i].IsCloseEnough(word, rules))
                {
                    closePatterns.Add(patterns[i]);
                }
            });

            var suggestions = new List<string>();

            if (closePatterns.Count == 0)
                return suggestions;

            var setsOfVariants = closePatterns
                    .Select(p => p.Explode(rules)
                        .Select(v => string.Join(string.Empty, v))
                        .Distinct()
                        .Select(v => new
                        {
                            Variant = v,
                            Distance = Levenshtein.GetDistanceTwoRows(word, v)
                        })
                        .OrderBy(i => i.Distance)
                        .Take(n)
                        .ToList()
                    ).ToList();

            return setsOfVariants.SelectMany(s => s)
                                .OrderBy(s => s.Distance)
                                .Take(n)
                                .Select(i => i.Variant)
                                .ToList();

            // TODO: Interleave the suggestions from all of the patterns
            // when they are at the same distance from the word
        }

        public override string ToString() => Template;
    }

    public struct Rule
    {
        public Rule(params string[] values) : this()
        {
            Values = values;
        }

        public string[] Values { get; }
    }
}

