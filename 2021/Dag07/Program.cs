using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2021")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(string inputFile)
        {
            var initPositions = new IntCsvLineParser().ReadData(inputFile).OrderBy(x => x).ToList();
            var position = DetermineMovePosition(initPositions);

            return initPositions.Aggregate(0L, (sum, p) => sum += Math.Abs(p - position));
        }
        
        private static int DetermineMovePosition(List<int> initPositions)
        {
            var length = initPositions.Count;
            if (length % 2 == 0)
            {
                return initPositions[length / 2];
            }
            return initPositions[length / 2] + initPositions[length / 2 + 1] / 2;
        }

        static long Second(string inputFile)
        {
            var initPositions = new IntCsvLineParser().ReadData(inputFile);
            var positions = initPositions.GroupBy(v => v).ToDictionary(v => v.Key, v => v.Count());

            var fs = positions.Select(d=> GetWeightFunction(d.Value, d.Key)).ToList();
            var minPos = positions.Keys.Min();
            var maxPos = positions.Keys.Max();
            var minimumResultFound = long.MaxValue;
            var foundMinimalPosition = minPos;
            for (int p = minPos+1; p < maxPos; p++)
            {
                Console.WriteLine($"Start calculation pos {p}");
                var result = fs.Aggregate(0L, (sum, f) => sum += f(p));
                if (result<minimumResultFound)
                {
                    Console.WriteLine($"!!! Lower solution found: {result}");

                    minimumResultFound = result;
                    foundMinimalPosition = p;
                }
            }

            return minimumResultFound;
        }

        private static Func<int,long> GetWeightFunction(int weight, int p0)
        {
            long f(int p) { return weight * Math.Abs(p - p0) * (Math.Abs(p - p0) + 1) / 2; }
            return f;
        }

        [TestMethod]
        public void TestDetermineMovePosition()
        {
            var initPositions = new IntCsvLineParser().ReadData("test.txt").OrderBy(x => x).ToList();
            var result = DetermineMovePosition(initPositions);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(37, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(168, result);
        }
    }
}
