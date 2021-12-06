using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag22
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

        static long First(string inputFile)
        {
            var parser = new IntDeckParser();
            var decks = parser.ReadDecks(inputFile);
            var game = new CombatGame(decks[0], decks[1]);
            var result = game.PlayGame();
            return result;
        }

        static long Second(string inputFile)
        {
            var parser = new IntDeckParser();
            var decks = parser.ReadDecks(inputFile);
            var game = new RecursiveCombatGame(decks[0], decks[1],1);
            var winner = game.PlayGame();
            var result = game.CalculateScore(winner);
            return result;
        }


        [DataTestMethod]
        [DataRow("test.txt", 306)]
        public void TestPart1(string inputFile, long expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", 291)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void CheckHashsetIntArray()
        {
            var hashset = new HashSet<string>();
            hashset.Add(string.Join("-",new[] { 1, 2, 3 }));
            hashset.Add(string.Join("-", new[] { 4, 5, 6 }));
            hashset.Add(string.Join("-", new[] { 4, 5}));
            hashset.Add(string.Join("-", new[] { 4, 5, 7 }));
            var result = hashset.Contains(string.Join("-", new[] { 4, 5 }));
            Assert.IsTrue(result);
        }
    }
}
