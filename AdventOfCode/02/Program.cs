using System;
using System.Collections.Generic;
using System.Linq;

namespace _02
{
    class Program
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        static IEnumerable<int> readInts(string filePath) => readlines(filePath).Select(s => Convert.ToInt32(s));
        static IEnumerable<Tuple<int, int>> Pairs(IEnumerable<int> list)
        {
            if (!list.Any())
            {
                return Enumerable.Empty<Tuple<int, int>>();
            }
            var head = list.First();
            var tail = list.Skip(1);
            return tail.Select(t => new Tuple<int, int>(head, t)).Concat(Pairs(tail));
        }
        static IEnumerable<Tuple<int, int, int>> Triplets(IEnumerable<int> list)
        {
            if (list.Count()<=2)
            {
                return Enumerable.Empty<Tuple<int, int, int>>();
            }
            if (list.Count() == 3)
            {
                return Enumerable.Repeat(new Tuple<int,int,int>(list.First(),list.Skip(1).First(),list.Skip(2).First()),1);
            }
            var head = list.First();
            var tail = list.Skip(1);
            return Pairs(tail).Select(t => new Tuple<int, int, int>(head, t.Item1, t.Item2)).Concat(Triplets(tail));
        }

        static void Main(string[] args)
        {
            var ints = readInts("input.txt");
            var triplets = Triplets(ints);
            Func<Tuple<int, int, int>, bool> checkTriplet = (x) => (x.Item1 + x.Item2 + x.Item3 == 2020);
            var foundTriplet = triplets.First(checkTriplet);
            Console.WriteLine("({0},{1},{2}) = {3}", foundTriplet.Item1, foundTriplet.Item2, foundTriplet.Item3, foundTriplet.Item1 * foundTriplet.Item2 * foundTriplet.Item3);
        }
    }
}
