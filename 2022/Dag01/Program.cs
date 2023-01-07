using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2022")]
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
            var parser = new NullIntParser();
            var data = parser.ReadData(inputFile);
            var maxCalories = 0L;
            var currentCalories = 0L;
            foreach(int? item in data) {
                if(item.HasValue) {
                    currentCalories += item.Value;
                }
                else {
                    if (currentCalories > maxCalories) {
                        maxCalories = currentCalories;
                    }
                    currentCalories = 0L;
                }
            }
            if (currentCalories > maxCalories) {
                maxCalories = currentCalories;
            }
            return maxCalories;
        }

        static long Second(string inputFile)
        {
            var parser = new SnackParser();
            return parser.ReadData(inputFile).OrderDescending().Take(3).Sum();
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(24000, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(45000, result);
        }
    }

    public class SnackParser 
    {
        IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);
        public IEnumerable<long> ReadData(string filePath) { 
            var lineData = Readlines(filePath).Select(ParseLine);
            var runningTotal = 0L;
            foreach(var line in lineData) {
                if (!line.HasValue) {
                    yield return runningTotal;
                    runningTotal=0L;
                } else {
                    runningTotal += line.Value;
                }
            }
            yield return runningTotal;
        }

        protected int? ParseLine(string line) => string.IsNullOrWhiteSpace(line) ? new int?() : new int?(Convert.ToInt32(line));
    }
}
