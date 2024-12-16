using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2024")]
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
            var parser = new IntSeriesParser(' ');
            var series = parser.ReadData(inputFile).ToList();
            var leftList = series.Select(x=>x.First()).Order().ToList();
            var rightList = series.Select(x=>x.Skip(1).First()).Order().ToList();

            var distances = leftList.Zip(rightList).Select(x => Math.Abs(x.First - x.Second));
            return distances.Sum();
        }

        static long Second(string inputFile)
        {
            var parser = new IntSeriesParser(' ');
            var series = parser.ReadData(inputFile).ToList();
            var leftList = series.Select(x => x.First()).Order().ToList();
            var rightList = series.Select(x => x.Skip(1).First()).Order().ToList();

            var similarities = leftList.Select(x => rightList.Count(y => y == x) * x);
            return similarities.Sum();
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(31, result);
        }
    }
}
