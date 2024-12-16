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
            var reports = parser.ReadData(inputFile).ToList();
            return reports.Count(IsSafeReport);
        }

        static long Second(string inputFile)
        {
            var parser = new IntSeriesParser(' ');
            var reports = parser.ReadData(inputFile).ToList();
            var results = 0;
            for (var i = 0;i<reports.Count; i++)
            {
                var report = reports[i].ToList();
                if (IsSafeReport(report))
                {
                    results++;
                    continue;
                }
                var foundMatch = false;
                //j will be the element to skip and try to match the resulting report
                for (var j = 0; j < report.Count; j++)
                {
                    var skippedElementReport = report.Take(j).Concat(report.Skip(j + 1));
                    if (IsSafeReport(skippedElementReport))
                    {
                        foundMatch = true;
                        break;
                    }
                }
                if (foundMatch)
                {
                    results++;
                }
            }
            return results;
        }

        private static bool IsSafeReport(IEnumerable<int> report)
        {
            var difference = report.Differences();
            return difference.All(x => x > 0 && x < 4) || difference.All(x => x < 0 && x > -4);
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(4, result);
        }
    }
}
