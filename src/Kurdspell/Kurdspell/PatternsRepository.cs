﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kurdspell
{
    public class PatternsRepository
    {
        public static List<Rule> GetRules()
        {
            return new List<Rule>
            {
                new Rule(new string[] { "m", "man", "t", "tan", "y", "yan" }),
                new Rule(new string[] { "m", "in", "a", "n", "et", "n" }),
                new Rule(new string[] { "om", "oyn", "oyt", "on", "wat", "on" }),
                new Rule(new string[] { "m", "in", "t", "n", "et", "n" }),
                new Rule(new string[] { "yaka", "yek", "yakan", "yan", "" }),
                new Rule(new string[] { "ish", "" }),
            };
        }

        public static List<Rule> GetKurdishRules()
        {
            return new List<Rule>
            {
                new Rule(new string[] { "م", "مان", "ت", "تان", "ی", "یان" }),
                new Rule(new string[] { "م", "ین", "ە", "ن", "ێت", "ن" }),
                new Rule(new string[] { "ۆم", "ۆین", "ۆیت", "ۆن", "وات", "ۆن" }),
                new Rule(new string[] { "م", "ین", "ت", "ن", "ێت", "ن" }),
                new Rule(new string[] { "یەکە", "یەک", "یەکان", "یان", "" }),
                new Rule(new string[] { "یش", "" }),
                 new Rule(new string[] { "م", "مان", "ت", "تان", "ی", "یان", "", "ە" }),
            };
        }

        public static List<Pattern> GetKurdishPatterns()
        {
            return new List<Pattern>
            {
                new Pattern("ب{0}بەخش{1}"),
                new Pattern("دەخ{2}"),
                new Pattern("دەکڕ{3}"),
                new Pattern("دەچ{3}"),
                new Pattern("مامۆستا{4}{0}{5}"),

                new Pattern("دارا{6}"),
                new Pattern("دوو"),
                new Pattern("دار{4}{6}{5}"),
                new Pattern("دیت"),
                new Pattern("دوور{6}"),
                new Pattern("ئۆی"),
                new Pattern("دادە{6}"),
                new Pattern("وەرە"),
                new Pattern("دەرێ"),
                new Pattern("دەوێ"),
                new Pattern("ئەوە"),
                new Pattern("ئازاد{6}"),
                new Pattern("دۆ{6}"),
                new Pattern("ئاو{6}"),
                new Pattern("زۆرە"),
            };
        }

        public static List<Pattern> GetPatterns()
        {
            var list = new List<Pattern>
            {
                new Pattern("Bi{0}bexsh{1}"),
                new Pattern("Dex{2}"),
                new Pattern("Dekrri{3}"),
                new Pattern("Dechi{3}"),
                new Pattern("Mamosta{4}{0}{5}"),
            };

            //for (int i = 0; i < 100_000; i++)
            //{
            //    list.Add(new Pattern(GetRandomPattern(5)));
            //}

            return list;
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