using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dag19
{
    [TestCategory("2020")]
    [TestClass]
    public class Parser
    {
        static readonly Regex messageRule = new Regex("^(?<messageRule>((?<id>\\d+):\\s((?<char>\"\\w\"$)|((?<rule>\\d+\\s?)+$)|((?<orRule1>\\d+)\\s((?<orRule1>\\d+)\\s)?\\|\\s(?<orRule2>\\d+)(\\s(?<orRule2>\\d+))?$))))|(?<message>\\w+$)", RegexOptions.Compiled);

        public string InputFile { get; private set; }

        public Parser(string inputFile)
        {
            InputFile = inputFile;
        }

        public MessageRules Rules { get; private set; }
        public List<string> Messages { get; private set; }

        public IEnumerable<string> Readlines() => System.IO.File.ReadLines(InputFile);
        //internal static IEnumerable<Instruction> ReadData(string filePath)
        //{
        //    return readlines(filePath).Select(ParseLine);
        //}

        internal bool ParseLine(string line)
        {
            var regexResult = messageRule.Match(line);
            if (!regexResult.Success)
            {
                return false;
            }

            if (regexResult.Groups.ContainsKey("messageRule") && regexResult.Groups["messageRule"].Success)
            {
                var ruleId = Convert.ToInt32(regexResult.Groups["id"].Value);
                if (regexResult.Groups.ContainsKey("char") && regexResult.Groups["char"].Success)
                {
                    Rules.AddConstantRule(ruleId, regexResult.Groups["char"].Value[1]);
                }
                else if (regexResult.Groups.ContainsKey("rule") && regexResult.Groups["rule"].Success)
                {
                    Rules.AddFollowingRule(ruleId, regexResult.Groups["rule"].Captures.Select(c=> Convert.ToInt32(c.Value)).ToArray());
                }
                else if (regexResult.Groups.ContainsKey("orRule1") && regexResult.Groups["orRule1"].Success)
                {
                    var or1Arguments = regexResult.Groups["orRule1"].Captures.Select(c => Convert.ToInt32(c.Value)).ToArray();
                    var or2Arguments = regexResult.Groups["orRule2"].Captures.Select(c => Convert.ToInt32(c.Value)).ToArray();
                    Rules.AddFollowingOrRule(ruleId, or1Arguments, or2Arguments);
                }
            }
            else if (regexResult.Groups.ContainsKey("message") && regexResult.Groups["message"].Success)
            {
                Messages.Add(regexResult.Groups["message"].Value);
            }
            return regexResult.Success;
        }

        internal void Parse()
        {
            Rules = new MessageRules();
            Messages = new List<string>();
            foreach (var line in Readlines())
            {
                ParseLine(line);
            }
        }

        [DataTestMethod]
        [DataRow("0: 4 1 5", true)]
        [DataRow("1: 2 3 | 3 2", true)]
        [DataRow("2: 4 4 | 5 5", true)]
        [DataRow("3: 4 5 | 5 4", true)]
        [DataRow("4: \"a\"", true)]
        [DataRow("5: \"b\"", true)]
        [DataRow("", false)]
        [DataRow("ababbb", true)]
        [DataRow("bababa", true)]
        [DataRow("abbbab", true)]
        [DataRow("aaabbb", true)]
        public void Test_Regex_Operation(string input, bool expected)
        {
            var isMatch = ParseLine(input);
            Assert.AreEqual(expected, isMatch);
        }

       

        //[TestMethod]
        //public void RegexTest()
        //{
        //    foreach (var item in readlines("test.txt"))
        //    {
        //        var regExREsult = instructionRule.Match(item);
        //        Assert.IsTrue(regExREsult.Success);
        //    }
        //}

    }
}
