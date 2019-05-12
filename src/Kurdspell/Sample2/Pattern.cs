using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kurdspell;

namespace Kurdspell2
{
    public struct Pattern
    {
        public Pattern(string template, IReadOnlyList<Rule> rules)
        {
            Template = template.ToLower();

            var dictionary = new Dictionary<string, Rule>();
            var parts = new List<string>();

            int index = 0;
            int start = 0;

            MinLength = 0;
            MaxLength = 0;

            while (index < Template.Length)
            {
                if (Template[index] == '{')
                {
                    if (start != index)
                    {
                        var current = Template.Substring(start, index - start);
                        parts.Add(current);
                        MinLength += current.Length;
                        MaxLength += current.Length;
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

                    if (!dictionary.ContainsKey(text))
                        dictionary.Add(text, rules[number]);
                    parts.Add("$" + text);

                    start = index + 1;

                    MaxLength += rules[number].Values.Max(v => v.Length);
                    MaxLength += rules[number].Values.Min(v => v.Length);
                }
                else if (index == template.Length - 1 && Template[index] != '}')
                {
                    parts.Add(Template.Substring(start, index + 1 - start));
                }

                index++;
            }

            Rules = dictionary;
            Parts = parts;

            _firstChar = Template[0];
            _secondChar = Template.Length >= 2 ? Template[1] : '\0';
            _thirdChar = Template.Length >= 3 ? Template[2] : '\0';
            _fourthChar = Template.Length >= 4 ? Template[3] : '\0';
            _fifthChar = Template.Length >= 5 ? Template[4] : '\0';

            _length = (byte)Template.Length;
        }

        public readonly string Template;
        public IReadOnlyDictionary<string, Rule> Rules { get; }
        public IReadOnlyList<string> Parts { get; }
        public int MaxLength { get; }
        public int MinLength { get; }

        private readonly char _firstChar;
        private readonly char _secondChar;
        private readonly char _thirdChar;
        private readonly char _fourthChar;
        private readonly char _fifthChar;
        private readonly byte _length;

        public bool IsExactly(string word, int wLength, char wFirstChar, char wSecondChar, char wThirdChar, char wFourthChar, char wFifthChar, int partIndex = 1, int charIndex = 0)
        {
            if (charIndex == 0)
            {
                if (_firstChar != wFirstChar)
                {
                    return false;
                }

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
                var part = Parts[partIndex];

                if (part[0] == '$')
                {
                    if (matches == null)
                        matches = new List<int>();
                    else
                        matches.Clear();

                    var variants = Rules[part.Substring(1)].Values;
                    var shouldGoOn = false;

                    int goodLength = 0;

                    for (int j = 0; j < variants.Length; j++)
                    {
                        var variant = variants[j];

                        if (CanBeTheSame(variant, variant.Length, word, wLength, charIndex))
                        {
                            shouldGoOn = true;
                            goodLength += variant.Length;
                            matches.Add(j);
                        }
                    }

                    if (matches.Count > 1 && partIndex != Parts.Count - 1)
                    {
                        shouldGoOn = false;
                        foreach (var match in matches)
                        {
                            shouldGoOn = IsExactly(word, wLength, wFirstChar, wSecondChar, wThirdChar, wFourthChar, wFifthChar, partIndex + 1, charIndex + variants[match].Length);

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
                    var length = part.Length;

                    if (!CanBeTheSame(part, length, word, wLength, charIndex))
                    {
                        return false;
                    }

                    charIndex += length;
                }
            }
            return true;
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

        public bool IsCloseEnough(string word, StringBuilder builder = null, int i = 0)
        {
            if (Template[0] != word[0])
                return false;

            builder = builder ?? new StringBuilder();
            var matches = new List<int>();

            for (; i < Parts.Count; i++)
            {
                var part = Parts[i];

                if (part[0] == '$')
                {
                    matches.Clear();

                    var variants = Rules[part.Substring(1)].Values;
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
                            builder.Append(variants[match]);
                            var originalLength = builder.Length;

                            shouldGoOn = IsCloseEnough(word, builder, i + 1);
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
                    builder.Append(part);

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

        public IEnumerable<IEnumerable<string>> Explode()
        {
            var sets = new List<IEnumerable<string>>();
            foreach (var part in Parts)
            {
                IEnumerable<string> set;
                if (part.StartsWith("$"))
                {
                    set = Rules[part.Substring(1)].Values;
                }
                else
                {
                    set = Enumerable.Repeat(part, 1);
                }

                sets.Add(set);
            }

            return CartesianProduct.Linq(sets).ToArray();
        }

        private List<string> Explode(string prefix, IReadOnlyList<string> parts)
        {
            var list = new List<string>();

            var builder = new StringBuilder(prefix, Template.Length);

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i].StartsWith("$"))
                {
                    var values = Rules[parts[i].Substring(1)].Values;
                    foreach (var value in values)
                    {
                        builder.Append(value);

                        if (i < parts.Count - 1)
                        {
                            var newPrefix = builder.ToString();
                            list.AddRange(Explode(newPrefix, parts.Skip(i + 1).ToList()));
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
                    builder.Append(parts[i]);
                }
            }

            return list;
        }

        public static List<string> GetTop(IEnumerable<Pattern> patterns, string word, int n)
        {
            patterns = patterns.Where(p => p.IsCloseEnough(word));

            var suggestions = new List<string>();

            var suggestionLists = new List<string[]>();
            var expanded = patterns.SelectMany(p => p.Explode()).Select(i => string.Join(string.Empty, i))
                                                   .OrderBy(s => Levenshtein.GetDistanceTwoRows(s, word)).ToArray();
            suggestionLists.Add(expanded);

            int index = 0;
            int j = 0;
            for (int i = 0; i < n && i < suggestionLists.Sum(l => l.Length); i++)
            {
                if (suggestionLists[j].Length <= i)
                {
                    suggestionLists.RemoveAt(j);
                    j--;
                }

                if (!suggestions.Contains(suggestionLists[j][index]))
                    suggestions.Add(suggestionLists[j][index]);

                index++;

                if (j == suggestionLists.Count)
                    j = 0;
            }

            return suggestions;
        }

        public override string ToString() => Template;
    }

    public struct Rule
    {
        public Rule(string[] values) : this()
        {
            Values = values;
        }

        public string[] Values { get; }
    }
}

