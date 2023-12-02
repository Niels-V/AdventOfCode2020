using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC
{
    [TestCategory("2023")]
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
            var parser = new CalibrationParser();
            var steps = parser.ReadData(inputFile);
            return steps.Sum();
        }

        static long Second(string inputFile)
        {
            return -1;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(142, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(-1, result);
        }

        public class CalibrationParser : Common.Parser<int> {
            static readonly Regex calibrationRule = new("(\\d)", RegexOptions.Compiled);
   
        protected override int ParseLine(string line) {
            var regexResult = calibrationRule.Matches(line);
            var firstInt = Convert.ToInt32(regexResult.First().Value);
            var lastInt =  Convert.ToInt32(regexResult.Last().Value);
            
            return firstInt*10+lastInt;
        }
    }
}
