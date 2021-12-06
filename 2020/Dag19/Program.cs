using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Linq.Expressions;

namespace Dag19
{
    [TestCategory("2020")]
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
            //As brute forcing / calculating all options via IsValid is quite hard cq. I couldn't
            // get my head around to, I analyzed the rules. As both the part two 
            // rules had the same format, it was easier to break down the message in different word sized parts.
            return parser.Messages.Count(message => HasValidMessageParts(parser, message));
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

        public static bool HasValidMessageParts(Parser parser, string message)
        {
            //Decide word length for the 42 and 31 rules, do not assume they are equal but validate that.
            var wordLength42 = FindWordLength(42, parser);
            var wordLength31 = FindWordLength(31, parser);
            if (wordLength31 != wordLength42) { throw new InvalidOperationException("We did assume equal word length for rule 42 and rule 31"); }

            return MessageParts(parser, message, wordLength42);
        }

        public static bool MessageParts(Parser parser, string message, int wordLength)
        {
            //Basically we implement the modifications of rule 8 and 11 not in the rule's, but change the analysis code in this block for it.

            //Modified rule 8 is that the message should starts with one or more 42 words.
            //Modified rle 11 is that the message ends with one ore more 42 words, and an equal amount of 31 words.
            //So the minimum message is word42 word42 word31. And as those words have fixed length, we can validate each word of the message,
            // as we determined the word length:
            if (message.Length % wordLength != 0) { return false; } //the message should fit an exactly amount of words, so return false when that isn't so.
            var segments = message.Length / wordLength; //the message should contain this amount of words.
            var valid42s = new List<bool>(); //will contain a bool with if that segments is valid according to rule 42
            var valid31s = new List<bool>();//will contain a bool with if that segments is valid according to rule 31
            for (int i = 0; i < segments; i++)
            {
                var segmentMessage = message.Substring(i * wordLength, wordLength); //what is the word of the segment
                var valid42 = parser.Rules[42].IsValid(segmentMessage, 0).IsValid; //determine if the word is valid according to the rule
                var valid31 = parser.Rules[31].IsValid(segmentMessage, 0).IsValid;
                valid42s.Add(valid42);
                valid31s.Add(valid31);
            }
            //rule 0 = 8 11 = 42{m} 42{n} 31{n} = 42{n+m} 31{n} with m>=1 and n>=1
            //So n+m>=2:
            if (!valid42s[0] || !valid42s[1]) { return false; }
            //And last element should be 31:
            if (!valid31s[valid31s.Count-1]) { return false; }
            //Now count the consecutive 42 words, and when false switch to 31 words
            //The sum of those should be the amount of words in the message,
            //And there should be more 42 words than 31 words (because n+m>n)
            var consec42 = 0;
            var consec31 = 0;
            for (int i = 0; i < valid42s.Count; i++)
            {
                if (valid42s[i]) { consec42++; } else { break; }
            }
            for (int i = consec42; i < valid31s.Count; i++)
            {
                if (valid31s[i]) { consec31++; } else { break; }
            }
            return (consec42 + consec31 == segments) && consec42 > consec31;
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
        [DataRow("test2.txt", "abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa", false)]
        public void TestMessagePart2(string inputFile, string message, bool expectedResult)
        {
            var parser = new Parser(inputFile);
            parser.Parse();
            var isValid = HasValidMessageParts(parser, message);
            Assert.AreEqual(expectedResult, isValid);
        }

        public static int FindWordLength(int ruleId, string inputFile)
        {
            var parser = new Parser(inputFile);
            parser.Parse();
            return FindWordLength(ruleId, parser);
        }

        private static int FindWordLength(int ruleId, Parser parser)
        {
            var rules = parser.Rules;
            var result = rules[ruleId].FindWordLength();
            if (result.Item1 == result.Item2) { return result.Item1; }
            throw new InvalidOperationException("Variable word width found");
        }

        [DataTestMethod]
        [DataRow("test2.txt", 42, 5, 5)]
        [DataRow("test2.txt", 31, 5, 5)]
        [DataRow("input.txt", 42, 8, 8)]
        [DataRow("input.txt", 31, 8, 8)]
        public void TestFindWordLength(string inputFile, int ruleId, int expectedResult)
        {
            var result = FindWordLength(42, inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}