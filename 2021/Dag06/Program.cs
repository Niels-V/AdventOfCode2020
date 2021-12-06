using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestClass]
    public class Program
    {
        public static readonly int FishCycleTime = 7;
        public static readonly int NewchildCycleTime = 8;

        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(string inputFile)
        {
            int days = 80;
            return Simulate(inputFile, days);
        }

        private static long Simulate(string inputFile, int days)
        {
            var initVector = new IntCsvLineParser().ReadData(inputFile);

            var oldCounts = initVector.GroupBy(v => v).ToDictionary(v=>v.Key, v=>v.LongCount());
            
            for (int i = 0; i < days; i++)
            {
                Dictionary<int, long> newCounts = new Dictionary<int, long>(8);
                for (int g = 1; g <= NewchildCycleTime; g++)
                {
                    if (oldCounts.ContainsKey(g))
                    {
                        newCounts.Add(g-1, oldCounts[g]);
                    }
                }
                if (oldCounts.ContainsKey(0))
                {
                    if (newCounts.ContainsKey(FishCycleTime - 1))
                    {
                        newCounts[FishCycleTime - 1] += oldCounts[0];
                    }
                    else
                    {
                        newCounts.Add(FishCycleTime - 1, oldCounts[0]);
                    }
                    newCounts.Add(NewchildCycleTime, oldCounts[0]);
                }
                Console.WriteLine($"{i + 1}:");
                foreach (var item in newCounts)
                {
                    Console.WriteLine($"\t{item.Key}:{item.Value}");

                }
                oldCounts = newCounts;
            }
            return oldCounts.Sum(f => f.Value);
        }

        static long Second(string inputFile)
        {
            int days = 256;
            return Simulate(inputFile, days);
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(5934, result);
        }

        [DataTestMethod]
        [DataRow(1,5)]
        [DataRow(2,6)]
        [DataRow(3,7)]
        [DataRow(4,9)]
        [DataRow(5,10)]
        [DataRow(6,10)]
        [DataRow(7,10)]
        [DataRow(8,10)]
        [DataRow(9,11)]
        [DataRow(10, 12)]
        [DataRow(11, 15)]
        [DataRow(12, 17)]
        [DataRow(13, 19)]
        [DataRow(14, 20)]
        [DataRow(15, 20)]
        [DataRow(16, 21)]
        [DataRow(17, 22)]
        [DataRow(18, 26)]
        public void TestSimulate(int days, long count)
        {
            var result = Simulate("test.txt", days);
            Assert.AreEqual(count, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(26984457539, result);
        }
    }
}
