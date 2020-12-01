using System;
using System.Collections.Generic;
using System.Linq;

namespace _01_C
{
    class Program
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        static IEnumerable<int> readInts(string filePath) => readlines(filePath).Select(s => Convert.ToInt32(s));
        static IEnumerable<Tuple<int,int>> Pairs(IEnumerable<int> list)
        {
            if (!list.Any())
            {
                return Enumerable.Empty<Tuple<int, int>>();
            }
            var head = list.First();
            var tail = list.Skip(1);
            return tail.Select(t => new Tuple<int, int>(head, t)).Concat(Pairs(tail));
        }

        static void Main(string[] args)
        {
            var ints = readInts("input.txt");
            var pairs = Pairs(ints);
            Func<Tuple<int,int>,bool> checkPairs = (x) => (x.Item1 + x.Item2 == 2020);
            var foundPair = pairs.First(checkPairs);
            Console.WriteLine("({0},{1}) = {2}", foundPair.Item1, foundPair.Item2, foundPair.Item1 * foundPair.Item2);
        }
    }
}
