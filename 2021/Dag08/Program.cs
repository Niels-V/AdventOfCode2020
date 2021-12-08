using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            var boards = new Parser().ReadData(inputFile);
            return boards.Aggregate(0, (sum, b) => { return sum + b.OutputValues.Count(v => v.Length == 2 || v.Length == 3 || v.Length == 4 || v.Length == 7); });
        }

        static long Second(string inputFile)
        {
            var boards = new Parser().ReadData(inputFile);
            return boards.Aggregate(0, (sum, b) => { return sum + b.Output(); });
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(26, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(61229, result);
        }
        [DataTestMethod]
        [DataRow("test.txt", 0, 8394)]
        [DataRow("test.txt", 1, 9781)]
        [DataRow("test.txt", 2, 1197)]
        [DataRow("test.txt", 3, 9361)]
        [DataRow("test.txt", 4, 4873)]
        [DataRow("test.txt", 5, 8418)]
        [DataRow("test.txt", 6, 4548)]
        [DataRow("test.txt", 7, 1625)]
        [DataRow("test.txt", 8, 8717)]
        [DataRow("test.txt", 9, 4315)]
        public void TestLinePart2(string inputFile, int line, int expectedValue)
        {
            var boards = new Parser().ReadData(inputFile);
            Assert.AreEqual(expectedValue, boards.ElementAt(line).Output());
        }
    }
    public class Board
    {
        public List<string> InputValues { get; private set; }
        public List<string> OutputValues { get; private set; }

        public int Output()
        {
            string[] values = new string[10];
            values[1] = InputValues.First(s => s.Length == 2);
            values[4] = InputValues.First(s => s.Length == 4);
            values[7] = InputValues.First(s => s.Length == 3);
            values[8] = InputValues.First(s => s.Length == 7);
            values[3] = InputValues.First(s => s.Length == 5 && values[7].All(c=>s.Contains(c)));
            values[9] = InputValues.First(s => s.Length == 6 && values[3].All(c=>s.Contains(c)));
            values[0] = InputValues.First(s => s.Length == 6 && s!= values[9] && values[7].All(c=>s.Contains(c)));
            values[6] = InputValues.First(s => s.Length == 6 && s!= values[9] && s!=values[0]);
            values[5] = InputValues.First(s => s.Length == 5 && s != values[3] && s.All(c=> values[9].Contains(c)));
            values[2] = InputValues.First(s => s.Length == 5 && s!= values[5] && s!=values[3]);

            var outputNumber = 1000 * Array.FindIndex(values, s => s == OutputValues[0]) +
                100 * Array.FindIndex(values, s => s == OutputValues[1]) +
                10 * Array.FindIndex(values, s => s == OutputValues[2]) +
                Array.FindIndex(values, s => s == OutputValues[3]);
            return outputNumber;
        }

        public Board()
        {
            InputValues = new List<string>(10);
            OutputValues = new List<string>(4);
        }
    }

    [TestCategory("2021")]
    [TestClass]
    public class Parser : Parser<Board>
    {
        static readonly Regex instructionRule = new("^((?<input>[abcdefg]{2,7})\\s?){10}\\s?\\|\\s?((?<display>[abcdefg]{2,7})\\s?){4}$", RegexOptions.Compiled);
        
        protected override Board ParseLine(string line)
        {
            var regexResult = instructionRule.Match(line);

            var board = new Board();
            //just sort the strings so they have all the same order, makes finding back the same numbers easier.
            board.InputValues.AddRange(regexResult.Groups.Values.First(g => g.Name == "input").Captures.Select(c => string.Concat(c.Value.OrderBy(c => c))));
            board.OutputValues.AddRange(regexResult.Groups.Values.First(g => g.Name == "display").Captures.Select(c => string.Concat(c.Value.OrderBy(c => c))));
            return board;
        }

        [TestMethod]
        public void TestParseLine()
        {
            var result = ParseLine("acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf");
            Assert.AreEqual(4, result.OutputValues.Count);
            Assert.AreEqual(10, result.InputValues.Count);
            Assert.AreEqual("cdbaf", result.OutputValues[3]);
            Assert.AreEqual("cdfeb", result.OutputValues[0]);
            Assert.AreEqual("acedgfb", result.InputValues[0]);
        }
    }
}
