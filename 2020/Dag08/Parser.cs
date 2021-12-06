using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag08
{
    [TestCategory("2020")]
    [TestClass]
    public class Parser
    {
        static readonly Regex instructionRule = new Regex("^(?<operation>nop|acc|jmp)\\s(?<argument>[+\\-]\\d+)$", RegexOptions.Compiled);
        
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        internal static IEnumerable<Instruction> ReadData(string filePath)
        {
            return readlines(filePath).Select(ParseLine);
        }

        internal static Instruction ParseLine(string line)
        {
            var regexResult = instructionRule.Match(line);
            
            var instruction = new Instruction { 
                Operation = regexResult.Groups.Values.First(g => g.Name == "operation").Value.ParseEnumValue<OperationType>(),
                Argument = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "argument").Value)
            };
            return instruction;
        }

        [DataTestMethod]
        [DataRow("nop +0", OperationType.NoOperation)]
        [DataRow("acc +1", OperationType.Accumulator)]
        [DataRow("jmp +4", OperationType.Jumps)]
        [DataRow("acc +3", OperationType.Accumulator)]
        [DataRow("jmp -3", OperationType.Jumps)]
        [DataRow("acc -99", OperationType.Accumulator)]
        [DataRow("acc +1", OperationType.Accumulator)]
        [DataRow("jmp -4", OperationType.Jumps)]
        [DataRow("acc +6", OperationType.Accumulator)]
        public void Test_Regex_Operation(string input, OperationType expectedOperation)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedOperation, result.Operation);
        }

        [DataTestMethod]
        [DataRow("nop +0", 0)]
        [DataRow("acc +1", 1)]
        [DataRow("jmp +4", 4)]
        [DataRow("acc +3", 3)]
        [DataRow("jmp -3", -3)]
        [DataRow("acc -99", -99)]
        [DataRow("acc +1", 1)]
        [DataRow("jmp -4", -4)]
        [DataRow("acc +6", 6)]
        public void Test_Regex_Argument(string input, int expectedArgument)
        {
            var result = ParseLine(input);
            Assert.AreEqual(expectedArgument, result.Argument);
        }

        [TestMethod]
        public void RegexTest()
        {
            foreach (var item in readlines("test.txt"))
            {
                var regExREsult = instructionRule.Match(item);
                Assert.IsTrue(regExREsult.Success);
            }
        }

    }
}
