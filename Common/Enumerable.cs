using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class Enumerable
    {
        public static IEnumerable<long> LongRange(long start, long end)
        {
            var current = start;
            while (current < end)
            {
                yield return current++;
            }
            yield return current;
        }
    }
}
