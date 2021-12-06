using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag23
{
    [TestCategory("2020")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First("318946572", 100);
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("318946572", 10000000, 1000000);
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static string First(string input, int moves)
        {
            var inputCups = input.Select(c => Convert.ToInt32(c.ToString()));
            var game = new Game(inputCups.ToArray(), inputCups.Count());
            game.Play(moves);
            return game.LabelsOrderFinished();
        }

        static long Second(string input, int moves, int maxItems)
        {
            var inputCups = input.Select(c => Convert.ToInt32(c.ToString()));
            var game = new Game(inputCups.ToArray(), maxItems);
            game.Play(moves);
            return game.LabelFirstTwoNextProduct();
        }
        

        [DataTestMethod]
        [DataRow("389125467",10, "92658374")]
        [DataRow("389125467", 100, "67384529")]
        public void TestPart1(string input, int moves, string expectedResult)
        {
            var result = First(input, moves);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("389125467",10000000, 1000000, 149245887792)]
        public void TestPart2(string input, int moves,int maxItems, long expectedResult)
        {
            var result = Second(input, moves, maxItems);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
