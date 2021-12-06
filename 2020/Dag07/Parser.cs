using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag07
{
    [TestCategory("2020")]
    [TestClass]
    public class Parser
    {
        static readonly Regex bagRule = new Regex("^(?<color>.+) bags? contain(((( (?<amount>\\d)+ (?<targetcolor>.*?) bags?,?)+))|( no other bags)).$", RegexOptions.Compiled);
        
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        internal static IEnumerable<BagRule> ReadData(string filePath)
        {
            return readlines(filePath).Select(ParseLine);
        }

        internal static BagRule ParseLine(string line)
        {
            var regexResult = bagRule.Match(line);
            BagRule rule = new BagRule { Color = regexResult.Groups.Values.First(g => g.Name == "color").Value };
            var amounts = regexResult.Groups.Values.First(g => g.Name == "amount").Captures;
            var targetcolors = regexResult.Groups.Values.First(g => g.Name == "targetcolor").Captures;
            for (int i = 0; i < amounts.Count; i++)
            {
                rule.ShouldContain.Add(targetcolors[i].Value, Convert.ToInt32(amounts[i].Value));
            }

            return rule;
        }

        [DataTestMethod]
        [DataRow("light red bags contain 1 bright white bag, 2 muted yellow bags.", "light red")]
        [DataRow("dark orange bags contain 3 bright white bags, 4 muted yellow bags.", "dark orange")]
        [DataRow("bright white bags contain 1 shiny gold bag.", "bright white")]
        [DataRow("faded blue bags contain no other bags.", "faded blue")]
        public void Test_Regex_Colors(string input, string expectedColor)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedColor, result.Color);
        }

        [DataTestMethod]
        [DataRow("light red bags contain 1 bright white bag, 2 muted yellow bags.", 2)]
        [DataRow("dark orange bags contain 3 bright white bags, 4 muted yellow bags.", 2)]
        [DataRow("bright white bags contain 1 shiny gold bag.", 1)]
        [DataRow("faded blue bags contain no other bags.", 0)]
        [DataRow("faded yellow bags contain 2 dull crimson bags, 3 muted indigo bags, 2 plaid crimson bags, 3 clear green bags.",4)]
        public void Test_Regex_NumberOfTargetColors(string input, int expectedTargetColors)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedTargetColors, result.ShouldContain.Count);
        }

        [DataTestMethod]
        [DataRow("light red bags contain 1 bright white bag, 2 muted yellow bags.", 3)]
        [DataRow("dark orange bags contain 3 bright white bags, 4 muted yellow bags.", 7)]
        [DataRow("bright white bags contain 1 shiny gold bag.", 1)]
        [DataRow("faded blue bags contain no other bags.", 0)]
        [DataRow("faded yellow bags contain 2 dull crimson bags, 3 muted indigo bags, 2 plaid crimson bags, 3 clear green bags.",10)]
        public void Test_Regex_SumOfTargetBags(string input, int expectedTargetColors)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedTargetColors, result.ShouldContain.Sum(b=>b.Value));
        }

        [DataTestMethod]
        [DataRow("light red bags contain 1 bright white bag, 2 muted yellow bags.", "bright white")]
        [DataRow("dark orange bags contain 3 bright white bags, 4 muted yellow bags.", "bright white")]
        [DataRow("bright white bags contain 1 shiny gold bag.", "shiny gold")]
        public void Test_Regex_FirstTargetColorName(string input, string expectedTargetColor)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedTargetColor, result.ShouldContain.First().Key);
        }

        [TestMethod]
        public void RegexTest()
        {
            foreach (var item in readlines("test.txt"))
            {
                var regExREsult = bagRule.Match(item);
                Assert.IsTrue(regExREsult.Success);
            }
        }

    }
}
