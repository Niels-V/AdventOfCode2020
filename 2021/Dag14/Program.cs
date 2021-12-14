using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var polymerInstruction = new Parser().ParseFile(inputFile);
            long difference = RunPolymerization(polymerInstruction, 10);

            return difference;
        }

        private static long RunPolymerization(PolymerInstructions polymerInstruction, int steps)
        {
            var polymer = polymerInstruction.Template;
            for (int step = 0; step < steps; step++)
            {
                polymer = GrowPolymer(polymerInstruction, polymer);
            }
            var monomerCounts = polymer.GroupBy(p => p).Select(p => new
            {
                Monomer = p.First(),
                Count = p.LongCount()
            });

            var difference = monomerCounts.Max(m => m.Count) - monomerCounts.Min(m => m.Count);
            return difference;
        }

        private static long RunCount(PolymerInstructions polymerInstruction, int steps)
        {
            var polymer = polymerInstruction.Template;
            var pairCount = new Dictionary<string, long>();
            for (int i = 0; i < polymer.Length - 1; i++)
                {
                    var p1 = polymer[i];
                    var p3 = polymer[i + 1];
                    string pair = string.Concat(p1, p3);
                if (pairCount.ContainsKey(pair)) { 
                    pairCount[pair] += 1; 
                }
                else
                {
                    pairCount.Add(pair, 1);
                }
            }
            for (int step = 0; step < steps; step++)
            {
                pairCount = GrowPolymerCounts(polymerInstruction, pairCount);
            }
            var monomerCounts = pairCount.SelectMany(p => p.Key.ToArray().Select(c => new { Key = c, Count = p.Value }))
                .GroupBy(a => a.Key).Select(grp => new { Key = grp.Key, Count = grp.Sum(g => g.Count) }).ToDictionary(k=>k.Key, c=>c.Count);
            monomerCounts[polymer.First()] += 1;
            monomerCounts[polymer.Last()] += 1;
            //All characters are counted twice now!
            var difference = (monomerCounts.Max(m => m.Value) - monomerCounts.Min(m => m.Value))/2;
            return difference;
        }

        private static Dictionary<string, long> GrowPolymerCounts(PolymerInstructions polymerInstruction, Dictionary<string, long> pairCount)
        {
            Dictionary<string, long> newCounts = new Dictionary<string, long>();
            foreach (var pair in pairCount.Keys)
            {
                var monomer = polymerInstruction.InsertionRules[pair];
                var newPair1 = string.Concat(pair[0] , monomer);
                var newPair2 = string.Concat(monomer , pair[1]);
                if (newCounts.ContainsKey(newPair1)) {
                    newCounts[newPair1] += pairCount[pair];
                } else
                {
                    newCounts.Add(newPair1, pairCount[pair]);
                }
                if (newCounts.ContainsKey(newPair2))
                {
                    newCounts[newPair2] += pairCount[pair];
                }
                else
                {
                    newCounts.Add(newPair2, pairCount[pair]);
                }
            }
            return newCounts;
        }

        private static string GrowPolymer(PolymerInstructions polymerInstruction, string polymer)
        {
            var sb = new StringBuilder();
            sb.Append(polymer[0]);
            for (int i = 0; i < polymer.Length - 1; i++)
            {
                var p1 = polymer[i];
                var p3 = polymer[i + 1];
                string pair = string.Concat(p1, p3);
                var p2 = polymerInstruction.InsertionRules[pair];
                sb.Append(p2);
                sb.Append(p3);
            }
            polymer = sb.ToString();
            return polymer;
        }

        static long Second(string inputFile)
        {
            var polymerInstruction = new Parser().ParseFile(inputFile);
            long difference = RunCount(polymerInstruction, 40);

            return difference;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(1588, result);
        }

        [TestMethod]
        public void TestParserTemplate()
        {
            var result = new Parser().ParseFile("test.txt").Template;
            Assert.AreEqual("NNCB", result);
        }

        [TestMethod]
        public void TestParserInstructions()
        {
            var result = new Parser().ParseFile("test.txt").InsertionRules;
            Assert.AreEqual(16, result.Count);
            Assert.AreEqual(true, result.ContainsKey("CN"));
            Assert.AreEqual('C', result["CN"]);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(2188189693529, result);
        }
    }

    public class PolymerInstructions
    {
        public string Template { get; set; }
        public Dictionary<string, char> InsertionRules { get; private set; } = new Dictionary<string, char>();
    }

    public class Parser : LineParser
    {
        public PolymerInstructions ParseFile(string filePath)
        {
            var instructions = new PolymerInstructions();
            var mode = 0;
            foreach (var line in ReadData(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    mode++;
                    continue;
                }
                if (mode == 0)
                {
                    instructions.Template=line;
                }
                else if (mode == 1)
                {
                    instructions.InsertionRules.Add(line.Substring(0,2),line[6]);
                }
            }
            return instructions;
        }
    }
}
