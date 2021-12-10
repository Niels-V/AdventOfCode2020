using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    public class ChunkPair
    {
        public char OpenCharacter { get; private set; }
        public char CloseCharacter { get; private set; }
        public long SyntaxErrorScore { get; private set; }
        public long CompletionScore { get; private set; }
        public ChunkPair(char openChar, char closeChar, long errorScore, long completionScore)
        {
            OpenCharacter = openChar; CloseCharacter = closeChar; SyntaxErrorScore = errorScore; CompletionScore = completionScore;
        }
    }
    public static class ChunkPairs
    {
        private static readonly ChunkPair p1 = new('(', ')', 3, 1);
        private static readonly ChunkPair p2 = new('[', ']', 57, 2);
        private static readonly ChunkPair p3 = new('{', '}', 1197, 3);
        private static readonly ChunkPair p4 = new('<', '>', 25137, 4);

        public static bool IsOpenCharacter(char c)
        {
            return c switch
            {
                '(' or '[' or '{' or '<' => true,
                _ => false,
            };
        }
        public static bool IsCloseCharacter(char c)
        {
            return c switch
            {
                ')' or ']' or '}' or '>' => true,
                _ => false,
            };
        }

        public static ChunkPair FindByOpenCharacter(char open)
        {
            return open switch
            {
                '(' => p1,
                '[' => p2,
                '{' => p3,
                '<' => p4,
                _ => null,
            };
        }
        public static ChunkPair FindByCloseCharacter(char close)
        {
            return close switch
            {
                ')' => p1,
                ']' => p2,
                '}' => p3,
                '>' => p4,
                _ => null,
            };
        }
    }
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
            var lines = new LineParser().ReadData(inputFile);
            var errorScore = 0L;
            foreach (var line in lines)
            {
                var stack = new Stack<char>();
                foreach (var c in line)
                {
                    if (ChunkPairs.IsOpenCharacter(c))
                    {
                        stack.Push(c);
                    }
                    if (ChunkPairs.IsCloseCharacter(c))
                    {
                        var pair = ChunkPairs.FindByCloseCharacter(c);

                        if (stack.Peek() != pair.OpenCharacter)
                        {
                            //syntax error
                            errorScore += pair.SyntaxErrorScore;
                            break;
                        }
                        stack.Pop();
                    }
                }
            }
            return errorScore;
        }
        static long Second(string inputFile)
        {
            var lines = new LineParser().ReadData(inputFile);
            //var errorLines = new List<string>();
            var completionScores = new List<long>();
            foreach (var line in lines)
            {
                var completionScore = 0L;
                var stack = new Stack<char>();
                var syntaxError = false;
                foreach (var c in line)
                {
                    if (ChunkPairs.IsOpenCharacter(c))
                    {
                        stack.Push(c);
                    }
                    if (ChunkPairs.IsCloseCharacter(c))
                    {
                        var pair = ChunkPairs.FindByCloseCharacter(c);

                        if (stack.Peek() != pair.OpenCharacter)
                        {
                            //syntax error
                            syntaxError = true;
                            break;
                        }
                        stack.Pop();
                    }
                }
                while (!syntaxError && stack.Count > 0)
                {
                    var charToClose = stack.Pop();
                    var pair = ChunkPairs.FindByOpenCharacter(charToClose);
                    completionScore *= 5;
                    completionScore += pair.CompletionScore;
                }
                if (completionScore > 0) { completionScores.Add(completionScore); }
            }
            return completionScores.OrderBy(l => l).ElementAt(completionScores.Count / 2);
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(26397, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(288957, result);
        }
    }
}
