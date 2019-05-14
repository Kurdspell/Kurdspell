using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kurdspell
{
    public static class Levenshtein
    {
        // The three implentations are based on this article: https://en.wikipedia.org/wiki/Levenshtein_distance

        public static int Number = 0;
        public static int GetDistanceRecursive(char[] text1, int length1, char[] text2, int length2)
        {
            Number++;
            if (length1 == 0) return length2;
            if (length2 == 0) return length1;

            var cost = 0;

            if (text1[length1 - 1] == text2[length2 - 1])
                cost = 0;
            else
                cost = 1;

            return Min(GetDistanceRecursive(text1, length1 - 1, text2, length2) + 1,
                       GetDistanceRecursive(text1, length1, text2, length2 - 1) + 1,
                       GetDistanceRecursive(text1, length1 - 1, text2, length2 - 1) + cost);
        }

        /// <summary>
        /// Computes and returns the Levenshtein edit distance between two strings, i.e. the
        /// number of insertion, deletion, and sustitution edits required to transform one
        /// string to the other. This value will be >= 0, where 0 indicates identical strings.
        /// Comparisons are case sensitive, so for example, "Fred" and "fred" will have a 
        /// distance of 1.
        /// http://blog.softwx.net/2014/12/optimizing-levenshtein-algorithm-in-c.html
        /// </summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Levenshtein_distance
        /// This is based on Sten Hjelmqvist's "Fast, memory efficient" algorithm, described
        /// at http://www.codeproject.com/Articles/13525/Fast-memory-efficient-Levenshtein-algorithm, 
        /// with some additional optimizations.
        /// </remarks>
        /// <param name="s">String being compared for distance.</param>
        /// <param name="t">String being compared against other string.</param>
        /// <returns>int edit distance, >= 0 representing the number of edits required to transform one string to the other.</returns>
        public static int GetDistanceOneRow(this string s, string t)
        {
            if (String.IsNullOrEmpty(s)) return (t ?? "").Length;
            if (String.IsNullOrEmpty(t)) return s.Length;

            // if strings of different lengths, ensure shorter string is in s. This can result in a little
            // faster speed by spending more time spinning just the inner loop during the main processing.
            if (s.Length > t.Length)
            {
                var temp = s; s = t; t = temp; // swap s and t
            }
            int sLen = s.Length; // this is also the minimun length of the two strings
            int tLen = t.Length;

            // suffix common to both strings can be ignored
            while ((sLen > 0) && (s[sLen - 1] == t[tLen - 1])) { sLen--; tLen--; }

            int start = 0;
            if ((s[0] == t[0]) || (sLen == 0))
            { // if there's a shared prefix, or all s matches t's suffix
              // prefix common to both strings can be ignored
                while ((start < sLen) && (s[start] == t[start])) start++;
                sLen -= start; // length of the part excluding common prefix and suffix
                tLen -= start;

                // if all of shorter string matches prefix and/or suffix of longer string, then
                // edit distance is just the delete of additional characters present in longer string
                if (sLen == 0) return tLen;

                t = t.Substring(start, tLen); // faster than t[start+j] in inner loop below
            }
            var v0 = new int[tLen];
            for (int j = 0; j < tLen; j++) v0[j] = j + 1;

            int current = 0;
            for (int i = 0; i < sLen; i++)
            {
                char sChar = s[start + i];
                int left = current = i;
                for (int j = 0; j < tLen; j++)
                {
                    int above = current;
                    current = left; // cost on diagonal (substitution)
                    left = v0[j];
                    if (sChar != t[j])
                    {
                        current++;              // substitution
                        int insDel = above + 1; // deletion
                        if (insDel < current) current = insDel;
                        insDel = left + 1;      // insertion
                        if (insDel < current) current = insDel;
                    }
                    v0[j] = current;
                }
            }
            return current;
        }

        public static int GetDistanceMatrix(string text1, string text2)
        {
            var matrix = new int[text1.Length, text2.Length];

            for (int i = 0; i < text1.Length; i++)
                matrix[i, 0] = i;

            for (int j = 0; j < text2.Length; j++)
                matrix[0, j] = j;

            for (int j = 1; j < text2.Length; j++)
            {
                for (int i = 1; i < text1.Length; i++)
                {
                    var substitutionCost = text1[i] == text2[j] ? 0 : 1;
                    matrix[i, j] = Min(matrix[i - 1, j] + 1,
                                       matrix[i, j - 1] + 1,
                                       matrix[i - 1, j - 1] + substitutionCost);
                }
            }

            return matrix[text1.Length - 1, text2.Length - 1];
        }

        /// <summary>
        /// Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of
        /// integers, where each integer represents the code point of a character in the source string.
        /// Includes an optional threshhold which can be used to indicate the maximum allowable distance.
        /// </summary>
        /// <param name="source">An array of the code points of the first string</param>
        /// <param name="target">An array of the code points of the second string</param>
        /// <param name="threshold">Maximum allowable distance</param>
        /// <returns>Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between the strings</returns>
        public static int DamerauLevenshteinDistance(int[] source, int[] target, int threshold)
        {

            int length1 = source.Length;
            int length2 = target.Length;

            // Return trivial case - difference in string lengths exceeds threshhold
            if (Math.Abs(length1 - length2) > threshold) { return int.MaxValue; }

            // Ensure arrays [i] / length1 use shorter length 
            if (length1 > length2)
            {
                Swap(ref target, ref source);
                Swap(ref length1, ref length2);
            }

            int maxi = length1;
            int maxj = length2;

            int[] dCurrent = new int[maxi + 1];
            int[] dMinus1 = new int[maxi + 1];
            int[] dMinus2 = new int[maxi + 1];
            int[] dSwap;

            for (int i = 0; i <= maxi; i++) { dCurrent[i] = i; }

            int jm1 = 0, im1 = 0, im2 = -1;

            for (int j = 1; j <= maxj; j++)
            {

                // Rotate
                dSwap = dMinus2;
                dMinus2 = dMinus1;
                dMinus1 = dCurrent;
                dCurrent = dSwap;

                // Initialize
                int minDistance = int.MaxValue;
                dCurrent[0] = j;
                im1 = 0;
                im2 = -1;

                for (int i = 1; i <= maxi; i++)
                {

                    int cost = source[im1] == target[jm1] ? 0 : 1;

                    int del = dCurrent[im1] + 1;
                    int ins = dMinus1[i] + 1;
                    int sub = dMinus1[im1] + cost;

                    //Fastest execution for min value of 3 integers
                    int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

                    if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
                        min = Math.Min(min, dMinus2[im2] + cost);

                    dCurrent[i] = min;
                    if (min < minDistance) { minDistance = min; }
                    im1++;
                    im2++;
                }
                jm1++;
                if (minDistance > threshold) { return int.MaxValue; }
            }

            int result = dCurrent[maxi];
            return (result > threshold) ? int.MaxValue : result;
        }

        static void Swap<T>(ref T arg1, ref T arg2)
        {
            T temp = arg1;
            arg1 = arg2;
            arg2 = temp;
        }


        public static int GetDistanceTwoRows(StringBuilder s, string t, int tLength = -1)
        {
            if (tLength == -1)
                Math.Min(s.Length, t.Length);

            var vector0 = new int[tLength + 1];
            var vector1 = new int[tLength + 1];
            int[] tempReference;

            for (int i = 0; i <= tLength; i++)
                vector0[i] = i;

            for (int i = 0; i < s.Length; i++)
            {
                vector1[0] = i + 1;

                for (int j = 0; j < tLength; j++)
                {
                    var substitutionCost = s[i] == t[j] ? 0 : 1;
                    vector1[j + 1] = Min(vector1[j] + 1,
                                         vector0[j + 1] + 1,
                                         vector0[j] + substitutionCost);
                }

                tempReference = vector0;
                vector0 = vector1;
                vector1 = tempReference;
            }

            return vector0[tLength];
        }

        public static int GetDistanceTwoRows(string s, string t)
        {
            var vector0 = new int[t.Length + 1];
            var vector1 = new int[t.Length + 1];
            int[] tempReference;

            for (int i = 0; i <= t.Length; i++)
                vector0[i] = i;

            for (int i = 0; i < s.Length; i++)
            {
                vector1[0] = i + 1;

                for (int j = 0; j < t.Length; j++)
                {
                    var substitutionCost = s[i] == t[j] ? 0 : 1;
                    vector1[j + 1] = Min(vector1[j] + 1,
                                         vector0[j + 1] + 1,
                                         vector0[j] + substitutionCost);
                }

                tempReference = vector0;
                vector0 = vector1;
                vector1 = tempReference;
            }

            return vector0[t.Length];
        }

        public static int GetDistanceTwoRows(List<string> s, string t)
        {
            var sLength = s.Sum(p => p.Length);

            var vector0 = new int[t.Length + 1];
            var vector1 = new int[t.Length + 1];
            int[] tempReference;

            for (int i = 0; i <= t.Length; i++)
                vector0[i] = i;

            for (int i = 0; i < sLength; i++)
            {
                vector1[0] = i + 1;

                for (int j = 0; j < t.Length; j++)
                {
                    var substitutionCost = s.CharAt(i) == t[j] ? 0 : 1;
                    vector1[j + 1] = Min(vector1[j] + 1,
                                         vector0[j + 1] + 1,
                                         vector0[j] + substitutionCost);
                }

                tempReference = vector0;
                vector0 = vector1;
                vector1 = tempReference;
            }

            return vector0[t.Length];
        }

        private static int Min(int number1, int number2, int number3)
        {
            if (number1 > number2)
            {
                return number2 > number3 ? number3 : number2;
            }
            else
            {
                return number1 > number3 ? number3 : number1;
            }
        }
    }
}