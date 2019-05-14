using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kurdspell
{
    public class ComposedString
    {
        private readonly string[] _parts;
        public ComposedString(params string[] parts)
        {
            _parts = parts;
            foreach (var p in _parts)
            {
                Length += p.Length;
            }
        }

        public int Length { get; }
        public char this[int index]
        {
            get
            {
                if (index > Length - 1)
                    throw new IndexOutOfRangeException();

                int j = 0;
                for (int i = 0; index > _parts[i].Length - 1; i++)
                {
                    j = i + 1;
                    index -= _parts[i].Length;
                }

                return _parts[j][index];
            }
        }

    }
}
