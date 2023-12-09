using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2023")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            //var result = First("input.txt");
            //Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(string inputFile)
        {
            var parser = new MapParser();
            var (route, map) = parser.ReadMap(inputFile);
            var currentPosition = map["AAA"];
            int stepsTaken = 0;
            do
            {
                var nextStep = route.NextDirection();
                currentPosition = nextStep=='L' ? currentPosition.Left : currentPosition.Right;
                stepsTaken++;
            } while (currentPosition.Key != "ZZZ");
            
            return stepsTaken;
        }

        static long Second(string inputFile)
        {
            var parser = new MapParser();
            var (route, map) = parser.ReadMap(inputFile);
            var currentPositions = map.Where(m=>m.Key.EndsWith('A')).Select(m=>m.Value).ToList();
            var positions = currentPositions.Count;
            var firstPositions = new long[positions];
            long stepsTaken = 0L;
            do
            {
                var nextStep = route.NextDirection();
                currentPositions = nextStep == 'L' ?
                    currentPositions.Select(cp => cp.Left).ToList() :
                    currentPositions.Select(cp => cp.Right).ToList();
                
                stepsTaken++;
                for (int i = 0; i < positions; i++)
                {
                    if (currentPositions[i].Key.EndsWith('Z'))
                    {if (firstPositions[i] == 0) { firstPositions[i] = stepsTaken; }
                        Console.WriteLine($"{stepsTaken}: Node {i} is on {currentPositions[i].Key}");
                    }
                }
            } while (stepsTaken<1000000);
            //brute force doesn't work. But they hit always the same node. So:
            //0:19631 1:17287 2:12599 3:23147 4:13771 5:20803 node 0 is always hit by a multiply of 19631 steps, node 1 by 17287
            //so we need the Least Common Multiple of those six numbers:

            return Lcm(firstPositions);
        }
        /// <summary>
        /// Greatest Common Divisor
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        static long Gcd(long n1, long n2)
        {
            if (n2 == 0)
            {
                return n1;
            }
            else
            {
                return Gcd(n2, n1 % n2);
            }
        }

        public static long Lcm(long[] numbers)
        {
            return numbers.Aggregate((S, val) => S * val / Gcd(S, val));
        }

        [TestMethod]
        public void TestPart1a()
        {
            var result = First("test.txt");
            Assert.AreEqual(2, result);
        }
        [TestMethod]
        public void TestPart1b()
        {
            var result = First("test2.txt");
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test3.txt");
            Assert.AreEqual(6, result);
        }
    }
    public class MapItem
    {
        public string Key { get; set; }
        public MapItem Left { get; set; }
        public MapItem Right { get; set; }
    }
    public class Route
    {
        public Route(string routeString)
        {
            RouteString = routeString;
            _index = 0;
        }

        public char NextDirection()
        {
            var nextStep = RouteString[_index];
            _index++;
            if (_index>=RouteString.Length)
            {
                _index = 0;
            }
            return nextStep;
        }
        public string RouteString { get; }

        private int _index;
    }
    public class MapParser
    {
        protected IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);

        public Tuple<Route, Dictionary<string,MapItem>> ReadMap(string filePath)
        {
            var lines = Readlines(filePath).ToList();
            var route = new Route(lines.First());
            
            var map = new Dictionary<string, MapItem>();
            foreach (var line in lines.Skip(2))
            {
                //AAA = (BBB, CCC)
                var from = line.Substring(0, 3);
                var left = line.Substring(7, 3);
                var right = line.Substring(12, 3);
                if (!map.ContainsKey(from)) { var fromItem = new MapItem { Key = from }; map.Add(from, fromItem); }
                if (!map.ContainsKey(left)) { var leftItem = new MapItem { Key = left }; map.Add(left, leftItem); }
                if (!map.ContainsKey(right)) { var rightItem = new MapItem { Key = right }; map.Add(right, rightItem); }
                map[from].Left = map[left];
                map[from].Right = map[right];
            }
            return new (route, map);
        }
    }
}
