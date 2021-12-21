using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    public class DeterministicDice
    {
        public int Throws { get; set; }
        public int Roll()
        {
            Throws++;
            return ((Throws - 1) % 100) + 1;
        }
        public int Roll(int times)
        {
            var result = 0;
            for (int i = 0; i < times; i++)
            {
                result += Roll();
            }
            return result;
        }
    }
    public class Player
    {
        public int Position { get; set; }
        public int Points { get; set; }
        public void Move(int steps)
        {
            Position = ((Position + steps - 1) % 10) + 1;
            Points += Position;
        }
    }

    [TestCategory("2021")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First(8, 6);
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second(8, 6);
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(int player1Start, int player2Start)
        {
            var dice = new DeterministicDice();
            var player1 = new Player { Position = player1Start };
            var player2 = new Player { Position = player2Start };
            bool player1turn = true;
            while (player1.Points < 1000 && player2.Points < 1000)
            {
                var player = player1turn ? player1 : player2;
                player.Move(dice.Roll(3));
                player1turn = !player1turn;
            }
            var losingPlayer = player1turn ? player1 : player2;
            return losingPlayer.Points * dice.Throws;
        }

        static long Second(int player1Start, int player2Start)
        {
            var pointCounts = new Dictionary<(int, int, int, int), long>()
            {
                {(player1Start, player2Start, 0, 0), 1 }
            };
            var player1Wins = 0L;
            var player2Wins = 0L;
            //build the frequency table of the 3 dice throws.
            var throw3diceResults = new Dictionary<int, long>();
            for (int d1 = 1; d1 <= 3; d1++)
            {
                for (int d2 = 1; d2 <= 3; d2++)
                {
                    for (int d3 = 1; d3 <= 3; d3++)
                    {
                        if (!throw3diceResults.TryAdd(d1 + d2 + d3, 1L)) { throw3diceResults[d1 + d2 + d3]++; }
                    }
                }
            }
            Dictionary<(int, int, int, int), long> newCounts;
            while (pointCounts.Count > 0)
            {
                //player1 turn
                newCounts = new Dictionary<(int, int, int, int), long>();
                foreach (var point in pointCounts)
                {
                    foreach (var throw3 in throw3diceResults)
                    {
                        var position = ((point.Key.Item1 + throw3.Key -1) % 10) + 1;
                        var points = point.Key.Item3 + position;
                        if (!newCounts.TryAdd((position, point.Key.Item2, points, point.Key.Item4), point.Value*throw3.Value)) { 
                            newCounts[(position, point.Key.Item2, points, point.Key.Item4)] += point.Value*throw3.Value; 
                        }
                    }
                }
                pointCounts = newCounts;

                //checkwins
                player1Wins += pointCounts.Where(c => c.Key.Item3 >= 21).Sum(c => c.Value);
                pointCounts = pointCounts.Where(c => c.Key.Item3 < 21).ToDictionary(c => c.Key, c => c.Value);
                //if all universes ended with wins, return.
                if (pointCounts.Count == 0) { break; }
                //player2 turn

                newCounts = new Dictionary<(int, int, int, int), long>();
                foreach (var point in pointCounts)
                {
                    foreach (var throw3 in throw3diceResults)
                    {
                        var position = ((point.Key.Item2 + throw3.Key - 1) % 10) + 1;
                        var points = point.Key.Item4 + position;
                        if (!newCounts.TryAdd((point.Key.Item1, position, point.Key.Item3, points), point.Value * throw3.Value))
                        {
                            newCounts[(point.Key.Item1, position, point.Key.Item3, points)] += point.Value * throw3.Value;
                        }
                    }
                }
                pointCounts = newCounts;
                //checkwins
                player2Wins += pointCounts.Where(c => c.Key.Item4 >= 21).Sum(c => c.Value);
                pointCounts = pointCounts.Where(c => c.Key.Item4 < 21).ToDictionary(c => c.Key, c => c.Value);
            }
            return Math.Max(player1Wins, player2Wins);
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First(4, 8);
            Assert.AreEqual(739785, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second(4, 8);
            Assert.AreEqual(444356092776315, result);
        }
    }
}
