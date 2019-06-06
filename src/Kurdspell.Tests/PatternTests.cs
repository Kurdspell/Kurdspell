using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Kurdspell.Tests
{
    public class PatternTests
    {
        private readonly IReadOnlyDictionary<string, Affix> _affixes;
        public PatternTests()
        {
            var affixes = new List<Affix>
            {
                new Affix("0", "man", "tan"),
            };

            _affixes = affixes.ToDictionary(a => a.Name);
        }

        [Fact]
        public void NormalExplosion()
        {
            var pattern = new Pattern("slaw[0]");
            var explosion = pattern.Explode(_affixes);

            var expected = new List<List<string>>
            {
                new List<string> { "slaw", "man"},
                new List<string> { "slaw", "tan"},
            };

            Assert.Equal(expected, explosion);
        }

        [Fact]
        public void ExplosionWithDuplicateAffixCleansUpDuplicateAffixes()
        {
            var pattern = new Pattern("slaw[0][0]");
            var explosion = pattern.Explode(_affixes);

            var expected = new List<List<string>>
            {
                new List<string> { "slaw", "man", "tan"},
                new List<string> { "slaw", "tan", "man"},
            };

            Assert.Equal(expected, explosion);
        }

        [Fact]
        public void ExplosionWithDuplicateAffixAndNonAffixCleansUpOnlyDuplicateAffixes()
        {
            var pattern = new Pattern("man[0]man[0]tan");
            var explosion = pattern.Explode(_affixes);

            var expected = new List<List<string>>
            {
                new List<string> { "man", "man", "man", "tan", "tan"},
                new List<string> { "man", "tan", "man", "man", "tan"},
            };

            Assert.Equal(expected, explosion);
        }
    }
}
