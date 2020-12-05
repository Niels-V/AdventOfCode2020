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
            {
                //Assume [A,B,C,D,E]
                if (!list.Any())
                {
                    //return empty Tuple
                    return Enumerable.Empty<Tuple<int, int>>();
                }
                var head = list.First(); // A
                var tail = list.Skip(1); // [B,C,D,E]
                return tail.Select(t => new Tuple<int, int>(head, t)) //Return all the possbile combinations you can make with head and tail elements
                                                                      // (A,B),(A,C),(A,D),(A,E)
                    .Concat(Pairs(tail)); //Return all pair combinations possible from the tail, is Pair([B,C,D,E])
            }
        }
        static IEnumerable<Tuple<int, int, int>> Triplets(IEnumerable<int> list)
        {
            //Assumbe [A,B,C,D,E]
            if (list.Count()<=2)
            {
                //return empty Tuple
                return Enumerable.Empty<Tuple<int, int, int>>();
            }
            if (list.Count() == 3)
            {
                //Assume called with [A,B,C] only (A,B,C) can be returned
                return Enumerable.Repeat(new Tuple<int,int,int>(list.First(),list.Skip(1).First(),list.Skip(2).First()),1);
            }
            var head = list.First(); //A
            var tail = list.Skip(1); // [B,C,D,E]
            return Pairs(tail).Select(t => new Tuple<int, int, int>(head, t.Item1, t.Item2))
                //Select all possible pairs from tail, and prepend them with head
                // (A,B,C),(A,B,D), ... , (A,D,E)
                .Concat(Triplets(tail)); //Return all triple combinations possible from the tail, is Triplets(tail)
        }

        static void Main(string[] args)
        {
            var ints = readInts("input.txt");
            var triplets = Triplets(ints);
            Func<Tuple<int, int, int>, bool> checkTriplet = (x) => (x.Item1 + x.Item2 + x.Item3 == 2020);
            var foundTriplet = triplets.First(checkTriplet);
            Console.WriteLine("({0},{1},{2}) = {3}", foundTriplet.Item1, foundTriplet.Item2, foundTriplet.Item3, foundTriplet.Item1 * foundTriplet.Item2 * foundTriplet.Item3);
        }

        static void MainStep1(string[] args)
        {
            var ints = readInts("input.txt");
            var pairs = Pairs(ints);
            Func<Tuple<int,int>,bool> checkPairs = (x) => (x.Item1 + x.Item2 == 2020);
            var foundPair = pairs.First(checkPairs);
            Console.WriteLine("({0},{1}) = {2}", foundPair.Item1, foundPair.Item2, foundPair.Item1 * foundPair.Item2);
        }
    }
}
