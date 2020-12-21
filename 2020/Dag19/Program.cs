using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Linq.Expressions;

namespace Dag19
{
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

        static int First(string inputFile)
        {
            var parser = new Parser(inputFile);
            parser.Parse();
            var rules = parser.Rules;

            return parser.Messages.Count(message => rules.IsValid(message));
        }

        static int Second(string inputFile)
        {
            var parser = new Parser(inputFile);
            parser.Parse();
            var rules = parser.Rules;
            //8: 42 | 42 8
            //11: 42 31 | 42 11 31
            rules.ChangeFollowingOrRule(8,new[] { 42 }, new[] { 42, 8 });
            rules.ChangeFollowingOrRule(11,new[] { 42,31 }, new[] { 42, 11,31});
            return parser.Messages.Count(message => rules.IsValid(message));
        }

        [DataTestMethod]
        [DataRow("test.txt", 2)]
        public void TestPart1(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test2.txt", 12)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test2.txt", "bbabbbbaabaabba", true)]
        [DataRow("test2.txt", "babbbbaabbbbbabbbbbbaabaaabaaa", true)]
        [DataRow("test2.txt", "aaabbbbbbaaaabaababaabababbabaaabbababababaaa", true)]
        [DataRow("test2.txt", "bbbbbbbaaaabbbbaaabbabaaa", true)]
        [DataRow("test2.txt", "bbbababbbbaaaaaaaabbababaaababaabab", true)]
        [DataRow("test2.txt", "ababaaaaaabaaab", true)]
        [DataRow("test2.txt", "ababaaaaabbbaba", true)]
        [DataRow("test2.txt", "baabbaaaabbaaaababbaababb", true)]
        [DataRow("test2.txt", "abbbbabbbbaaaababbbbbbaaaababb", true)]
        [DataRow("test2.txt", "aaaaabbaabaaaaababaa", true)]
        [DataRow("test2.txt", "aaaabbaabbaaaaaaabbbabbbaaabbaabaaa", true)]
        [DataRow("test2.txt", "aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba", true)]
        public void TestMessagePart2(string inputFile, string message, bool expectedResult)
        {
            var parser = new Parser(inputFile);
            parser.Parse();
            int newRuleId = parser.Rules.Rules.Keys.Max() + 1;
            var rules = parser.Rules;
            //8: 42 | 42 8
            rules.AddFollowingRule(newRuleId, System.Linq.Enumerable.Repeat(42, message.Length).ToArray()); //id=43
            for (int i=message.Length-1; i>1; i--)
            {
                rules.AddFollowingOrRule(newRuleId+1, System.Linq.Enumerable.Repeat(42, i).ToArray(), new[] { newRuleId++ }); //ruleid: 44, verwijst naar rule 43, add 1
            }
            //11: 42 31 | 42 11 31
            rules.ChangeFollowingOrRule(8, new[] { 42 }, new[] { 42, newRuleId++ });
            var halfMessageLength = message.Length / 2;
            rules.AddFollowingRule(newRuleId, System.Linq.Enumerable.Repeat(42, halfMessageLength).Concat(System.Linq.Enumerable.Repeat(31, halfMessageLength)).ToArray());
            for (int i = halfMessageLength - 1; i > 1; i--)
            {
                rules.AddFollowingOrRule(newRuleId + 1, System.Linq.Enumerable.Repeat(42, i).Concat(System.Linq.Enumerable.Repeat(31, halfMessageLength)).ToArray(), new[] { newRuleId++ });
            }
            rules.ChangeFollowingOrRule(11, new[] { 42, 31 }, new[] { 42, newRuleId, 31 });
            var isValid = rules.IsValid(message);
            Assert.AreEqual(expectedResult, isValid);


        }
    }
}
