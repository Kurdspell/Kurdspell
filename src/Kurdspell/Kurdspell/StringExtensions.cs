using System.Collections.Generic;

namespace Kurdspell
{
    public static class StringExtensions
    {
        public static char CharAt(this List<string> parts, int index)
        {
            int j = 0;
            for (int i = 0; index > parts[i].Length - 1; i++)
            {
                j = i + 1;
                index -= parts[i].Length;
            }

            return parts[j][index];
        }
    }
}
