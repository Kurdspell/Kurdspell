using System;
using System.Collections.Generic;
using Xunit;

namespace Kurdspell.Tests
{
    public class SpellCheckerTests
    {
        [Theory]
        [InlineData("d", false)]
        [InlineData("dexom", true)]
        [InlineData("dekrrim", true)]
        [InlineData("supastandekeyn", true)]
        [InlineData("daxn", false)]
        [InlineData("supastndekeyn", false)]
        [InlineData("supastandekem", true)]
        [InlineData("mamostaaa", false)]
        public void CheckSpelling(string word, bool expected)
        {
            var rules = new List<Rule>
            {
                new Rule("om", "oyn", "oyt", "on", "wat", "on"),
                new Rule("im", "in", "it", "n", "et", "n"),
                new Rule("m", "man", "t", "tan", "y", "yan"),
                new Rule("em", "eyn", "eyt", "en", "at", "en"),
                new Rule(new string[] { "m", "in", "a", "n", "et", "n" }),
                new Rule(new string[] { "ish", "" }),
            };

            var patterns = new List<Pattern>
            {
                new Pattern("dex{0}"),
                new Pattern("dekrr{1}"),
                new Pattern("supas{2}dek{3}"),
                new Pattern("m{4}{5}mos"),
            };

            var spellChecker = new SpellChecker(patterns, rules);

            var actual = spellChecker.Check(word);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("دوورمان", true)]
        [InlineData("مامۆست", false)]
        [InlineData("دەتانیی", false)]
        public void CheckSpellingKurdish(string word, bool expected)
        {
            var rules = new List<Rule>
            {
                new Rule(new string[] { "م", "مان", "ت", "تان", "ی", "یان","" }),
                new Rule(new string[] { "م", "ین", "ە", "ن", "ێت", "ن" }),
                new Rule(new string[] { "ۆم", "ۆین", "ۆیت", "ۆن", "وات", "ۆن" }),
                new Rule(new string[] { "م", "ین", "ت", "ن", "ێت", "ن" }),
                new Rule(new string[] { "یەکە", "یەک", "یەکان", "یان", "" }),
                new Rule(new string[] { "یش", "ێک","" }),
                new Rule(new string[] { "وە", "ووە","" }),
                new Rule(new string[] { "م", "مان", "ت", "تان", "ی", "یان", "", "ە" }),
            };

            var patterns = new List<Pattern>
            {
                new Pattern("دوور{0}"),
                new Pattern("مامۆستا{4}{0}{5}"),
                new Pattern("دە{7}تانی")
            };

            var spellChecker = new SpellChecker(patterns, rules);

            var actual = spellChecker.Check(word);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("dexm", "dexom")]
        [InlineData("dekrim", "dekrrim")]
        [InlineData("spastandekeyn", "supastandekeyn")]
        [InlineData("supastandkem", "supastandekem")]
        public void GetSuggestions(string word, string suggestion)
        {
            var rules = new List<Rule>
            {
                new Rule("om", "oyn", "oyt", "on", "wat", "on"),
                new Rule("im", "in", "it", "n", "et", "n"),
                new Rule("m", "man", "t", "tan", "y", "yan"),
                new Rule("em", "eyn", "eyt", "en", "at", "en"),
            };

            var patterns = new List<Pattern>
            {
                new Pattern("dex{0}"),
                new Pattern("dekrr{1}"),
                new Pattern("supas{2}dek{3}"),
            };

            var spellChecker = new SpellChecker(patterns, rules);

            var list = spellChecker.Suggest(word, 3);
            Assert.True(list.Count <= 3);
            Assert.Contains(list, s => s == suggestion);
        }
    }
}
