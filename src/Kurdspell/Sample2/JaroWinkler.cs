using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;

namespace Kurdspell
{
    // http://the-lazy-coder.blogspot.com/2014/10/evaluating-similarity-between-strings.html

    /// <summary>
    /// Jaro-Winkler is an algorithm to compare strings and evaluate the level of similarity on a range between 0 (not similar) and 1 (perfect match)
    /// The degree to which two words are similar is also known as "Distance". Jaro-Winkler tries to calculate the "Distance" between two words,
    /// which reflects the number of steps required to perform in one word in order to make it identical to the other one (steps such as:"add a character", "transpose a character", "delete a character", etc
    /// The more steps required, the closer the "Distance" is to zero. The less steps required, the closer the "Distance" is to one.
    /// For more information http://en.wikipedia.org/wiki/Jaro%E2%80%93Winkler_distance
    /// For details on the implementation http://isolvable.blogspot.co.uk/2011/05/jaro-winkler-fast-fuzzy-linkage.html
    /// We need to use the Jaro-Winkler algorithm to find possible matches between the database and Security names entered by the users in trade imports
    /// in order to suggest possible matches for the secuity names obtained from the broker.
    /// SQL has a FREETEXT function, as well as a SOUNDEX for similar purposes but they do not offer the degree of flexibility we need.
    /// FREETEXT searches for similar words, synonyms, alterations of a verb in different tenses also match, but it always has to be real words (something like McDNLDS would't match any similar word as McDnlds is not a real noun or verb)
    /// AS for SOUNDEX, it matches words that phonetically sound similar, but it doesn't help us 
    /// in more complex scenarios like "ytpe" and "type" which sound totally different however they are very similar words.
    /// </summary>
    public class JaroWinkler
    {
        private const double defaultMismatchScore = 0.0;
        private const double defaultMatchScore = 1.0;

        /// <summary>
        /// Gets the similarity between two strings by using the Jaro-Winkler algorithm.
        /// A value of 1 means perfect match. A value of zero represents an absolute no match
        /// </summary>
        /// <param name="_firstWord"></param>
        /// <param name="_secondWord"></param>
        /// <returns>a value between 0-1 of the similarity</returns>
        /// 
        public static double RateSimilarity(string _firstWord, string _secondWord)
        {
            // Converting to lower case is not part of the original Jaro-Winkler implementation
            // But we don't really care about case sensitivity in DIAMOND and wouldn't decrease security names similarity rate just because
            // of Case sensitivity
            _firstWord = _firstWord.ToLower();
            _secondWord = _secondWord.ToLower();

            if ((_firstWord != null) && (_secondWord != null))
            {
                if (_firstWord == _secondWord)
                    //return (SqlDouble)defaultMatchScore;
                    return defaultMatchScore;
                else
                {
                    // Get half the length of the string rounded up - (this is the distance used for acceptable transpositions)
                    int halfLength = Math.Min(_firstWord.Length, _secondWord.Length) / 2 + 1;

                    // Get common characters
                    StringBuilder common1 = GetCommonCharacters(_firstWord, _secondWord, halfLength);
                    int commonMatches = common1.Length;

                    // Check for zero in common
                    if (commonMatches == 0)
                        //return (SqlDouble)defaultMismatchScore;
                        return defaultMismatchScore;

                    StringBuilder common2 = GetCommonCharacters(_secondWord, _firstWord, halfLength);

                    // Check for same length common strings returning 0 if is not the same
                    if (commonMatches != common2.Length)
                        //return (SqlDouble)defaultMismatchScore;
                        return defaultMismatchScore;

                    // Get the number of transpositions
                    int transpositions = 0;
                    for (int i = 0; i < commonMatches; i++)
                    {
                        if (common1[i] != common2[i])
                            transpositions++;
                    }

                    int j = 0;
                    j += 1;

                    // Calculate Jaro metric
                    transpositions /= 2;
                    double jaroMetric = commonMatches / (3.0 * _firstWord.Length) + commonMatches / (3.0 * _secondWord.Length) + (commonMatches - transpositions) / (3.0 * commonMatches);
                    //return (SqlDouble)jaroMetric;
                    return jaroMetric;
                }
            }

            //return (SqlDouble)defaultMismatchScore;
            return defaultMismatchScore;
        }

        /// <summary>
        /// Returns a string buffer of characters from string1 within string2 if they are of a given
        /// distance seperation from the position in string1.
        /// </summary>
        /// <param name="firstWord">string one</param>
        /// <param name="secondWord">string two</param>
        /// <param name="separationDistance">separation distance</param>
        /// <returns>A string buffer of characters from string1 within string2 if they are of a given
        /// distance seperation from the position in string1</returns>
        private static StringBuilder GetCommonCharacters(string firstWord, string secondWord, int separationDistance)
        {
            if ((firstWord != null) && (secondWord != null))
            {
                StringBuilder returnCommons = new StringBuilder(20);
                StringBuilder copy = new StringBuilder(secondWord);
                int firstWordLength = firstWord.Length;
                int secondWordLength = secondWord.Length;

                for (int i = 0; i < firstWordLength; i++)
                {
                    char character = firstWord[i];
                    bool found = false;

                    for (int j = Math.Max(0, i - separationDistance); !found && j < Math.Min(i + separationDistance, secondWordLength); j++)
                    {
                        if (copy[j] == character)
                        {
                            found = true;
                            returnCommons.Append(character);
                            copy[j] = '#';
                        }
                    }
                }
                return returnCommons;
            }
            return null;
        }
    }
}
