using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common;

namespace Dag02
{
    [TestCategory("2021")]
    [TestClass]
    public class Parser : Parser<Instruction>
    {
        static readonly Regex instructionRule = new("^(?<operation>forward|down|up)\\s(?<argument>\\d+)$", RegexOptions.Compiled);

        protected override Instruction ParseLine(string line)
        {
            var regexResult = instructionRule.Match(line);

            var instruction = new Instruction
            {
                Operation = regexResult.Groups.Values.First(g => g.Name == "operation").Value.ParseEnumValue<OperationType>(),
                Argument = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "argument").Value)
            };
            return instruction;
        }

        [DataTestMethod]
        [DataRow("forward 5", OperationType.Forward)]
        [DataRow("up 5", OperationType.Up)]
        [DataRow("down 6", OperationType.Down)]
        [DataRow("forward 13", OperationType.Forward)]
        public void Test_Regex_Operation(string input, OperationType expectedOperation)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedOperation, result.Operation);
        }

        [DataTestMethod]
        [DataRow("forward 5", 5)]
        [DataRow("up 5", 5)]
        [DataRow("down 6", 6)]
        [DataRow("forward 13", 13)]
        public void Test_Regex_Argument(string input, int expectedArgument)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedArgument, result.Argument);
        }

        [TestMethod]
        public void RegexTest()
        {
            foreach (var item in Readlines("test.txt"))
            {
                var regExREsult = instructionRule.Match(item);
                Assert.IsTrue(regExREsult.Success);
            }
        }
    }
}
