using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag01
{
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
            var parser = new LongParser();
            var numbers = parser.ReadData(inputFile).ToList();
            var differences = numbers.Differences();
            return differences.Count(d=>d>0);
        }

        static int Second(string inputFile)
        {
            var parser = new LongParser();
            var numbers = parser.ReadData(inputFile).ToList();
            var differences = numbers.Differences3();
            return differences.Count(d => d > 0);
        }

        

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(7, result);
        }


        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(5, result);
        }
    }

    public static class Extension
    {
        public static IEnumerable<long> Differences3(this IList<long> list)
        {
            for (int i = 0; i < list.Count - 3; i++)
            {
                yield return list[i + 3] - list[i];
            }
            yield break;
        }
    }
}
