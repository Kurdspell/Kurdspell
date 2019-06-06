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
        [InlineData("dekird", true)]
        [InlineData("kird", true)]
        [InlineData("dkird", false)]
        public void CheckSpelling(string word, bool expected)
        {
            var affixes = new List<Affix>
            {
                new Affix("0", "om", "oyn", "oyt", "on", "wat", "on"),
                new Affix("1", "im", "in", "it", "n", "et", "n"),
                new Affix("2", "m", "man", "t", "tan", "y", "yan"),
                new Affix("3", "em", "eyn", "eyt", "en", "at", "en"),
                new Affix("4", "m", "in", "a", "n", "et", "n" ),
                new Affix("5", "ish", "" ),
                new Affix("6", "de", ""),
            };

            var patterns = new List<Pattern>
            {
                new Pattern("dex[0]"),
                new Pattern("dekrr[1]"),
                new Pattern("supas[2]dek[3]"),
                new Pattern("m[4][5]mos"),
                new Pattern("[6]kird"),
            };

            var spellChecker = new SpellChecker(patterns, affixes);

            var actual = spellChecker.Check(word);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("دوورمان", true)]
        [InlineData("مامۆست", false)]
        [InlineData("دەتانیی", false)]
        public void CheckSpellingKurdish(string word, bool expected)
        {
            var affixes = new List<Affix>
            {
                new Affix("0", new string[] { "م", "مان", "ت", "تان", "ی", "یان","" }),
                new Affix("1", new string[] { "م", "ین", "ە", "ن", "ێت", "ن" }),
                new Affix("2", new string[] { "ۆم", "ۆین", "ۆیت", "ۆن", "وات", "ۆن" }),
                new Affix("3", new string[] { "م", "ین", "ت", "ن", "ێت", "ن" }),
                new Affix("4", new string[] { "یەکە", "یەک", "یەکان", "یان", "" }),
                new Affix("5", new string[] { "یش", "ێک","" }),
                new Affix("6", new string[] { "وە", "ووە","" }),
                new Affix("7", new string[] { "م", "مان", "ت", "تان", "ی", "یان", "", "ە" }),
            };

            var patterns = new List<Pattern>
            {
                new Pattern("دوور[0]"),
                new Pattern("مامۆستا[4][0][5]"),
                new Pattern("دە[7]تانی")
            };

            var spellChecker = new SpellChecker(patterns, affixes);

            var actual = spellChecker.Check(word);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("dexm", "dexom")]
        [InlineData("dekrim", "dekrrim")]
        [InlineData("spastandekeyn", "supastandekeyn")]
        [InlineData("supastandkem", "supastandekem")]
        [InlineData("dkird", "dekird")]
        [InlineData("krd", "kird")]
        public void GetSuggestions(string word, string suggestion)
        {
            var affixes = new List<Affix>
            {
                new Affix("0", "om", "oyn", "oyt", "on", "wat", "on"),
                new Affix("1", "im", "in", "it", "n", "et", "n"),
                new Affix("2", "m", "man", "t", "tan", "y", "yan"),
                new Affix("3", "em", "eyn", "eyt", "en", "at", "en"),
                new Affix("4", "de", ""),
            };

            var patterns = new List<Pattern>
            {
                new Pattern("dex[0]"),
                new Pattern("dekrr[1]"),
                new Pattern("supas[2]dek[3]"),
                new Pattern("[4]kird"),
            };

            var spellChecker = new SpellChecker(patterns, affixes);

            var list = spellChecker.Suggest(word, 3);
            Assert.True(list.Count <= 3);
            Assert.Contains(list, s => s == suggestion);
        }
    }
}
