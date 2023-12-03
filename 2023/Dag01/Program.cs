using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
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
            var parser = new Calibration2Parser();
            var steps = parser.ReadData(inputFile);
            return steps.Sum();
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
            var result = Second("test2.txt");
            Assert.AreEqual(281, result);
        }

        [TestClass]
        public class Calibration2Parser : Common.Parser<int> {
            static readonly Regex calibrationRule = new("(\\d|(on(?=e))|(tw(?=o))|(thre(?=e))|(fou(?=r))|(fiv(?=e))|(si(?=x))|"+
            "(seve(?=n))|(eigh(?=t))|(nin(?=e)))", RegexOptions.Compiled);
   
            static string ReplaceDigit(string input) {
                return input.Replace("on","1")
                            .Replace("tw","2")
                            .Replace("thre","3")
                            .Replace("fou","4")
                            .Replace("fiv","5")
                            .Replace("si","6")
                            .Replace("seve","7")
                            .Replace("eigh","8")
                            .Replace("nin","9");
            }
            protected override int ParseLine(string line) {
                var regexResult = calibrationRule.Matches(line);
                var firstInt = Convert.ToInt32(ReplaceDigit(regexResult.First().Value));
                var lastInt =  Convert.ToInt32(ReplaceDigit(regexResult.Last().Value));
                
                return firstInt*10+lastInt;
           }
           [TestMethod]
           public void Combined() {
            string s = "5tg578fldlcxponefourtwonet";
            var result = ParseLine(s);
            Assert.AreEqual(51, result);
           }
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
}
