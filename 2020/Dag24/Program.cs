using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag24
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt", 100);
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static int First(string inputFile)
        {
            var parser = new LineParser();
            var directions = parser.ReadData(inputFile).ToList();
            var endPoints = directions.Select(dir => HexGrid.Walk(dir));
            var pointCounts = endPoints.GroupBy(p => p.ToString()).Select(p => new
            {
                Point = p.First(),
                Count = p.Count()
            });
            //number of black tiles are the tiles that are atleast one time flipped, and have an odd amount of flips:
            var blackTilesCount = pointCounts.Count(p => p.Count % 2 == 1);
            return blackTilesCount;
        }

        static int Second(string inputFile, int turns)
        {
            var parser = new LineParser();
            var directions = parser.ReadData(inputFile).ToList();
            var endPoints = directions.Select(dir => HexGrid.Walk(dir));
            var pointCounts = endPoints.GroupBy(p => p.ToString()).Select(p => new
            {
                Point = p.First(),
                Count = p.Count()
            });
            //number of black tiles are the tiles that are atleast one time flipped, and have an odd amount of flips:
            var blackTilePositions = pointCounts.Where(p => p.Count % 2 == 1).Select(p=>p.Point);

            var game = new HexGameOfLife();
            game.FillStart(blackTilePositions);
            for (int i = 1; i <= turns; i++)
            {
                game.DoTurn(i);
            }
            var result = game.HexTiles.Values.Count(t => t.IsBlackInTurn(turns));
            return result;
        }


        [DataTestMethod]
        [DataRow("test.txt", 10)]
        public void TestPart1(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        //Day 1: 15
        [DataRow("test.txt", 1, 15)]
        //Day 2: 12
        [DataRow("test.txt", 2, 12)]
        //Day 3: 25
        [DataRow("test.txt", 3, 25)]
        //Day 4: 14
        [DataRow("test.txt", 4, 14)]
        //Day 5: 23
        [DataRow("test.txt", 5, 23)]
        //Day 6: 28
        [DataRow("test.txt", 6, 28)]
        //Day 7: 41
        [DataRow("test.txt", 7, 41)]
        //Day 8: 37
        [DataRow("test.txt", 8, 37)]
        //Day 9: 49
        [DataRow("test.txt", 9, 49)]
        //Day 10: 37
        [DataRow("test.txt", 10, 37)]

        //Day 20: 132
        [DataRow("test.txt", 20, 132)]
        //Day 30: 259
        [DataRow("test.txt", 30, 259)]
        //Day 40: 406
        [DataRow("test.txt", 40, 406)]
        //Day 50: 566
        [DataRow("test.txt", 50, 566)]
        //Day 60: 788
        [DataRow("test.txt", 60, 788)]
        //Day 70: 1106
        [DataRow("test.txt", 70, 1106)]
        //Day 80: 1373
        [DataRow("test.txt", 80, 1373)]
        //Day 90: 1844
        [DataRow("test.txt", 90, 1844)]
        //Day 100: 2208
        [DataRow("test.txt", 100, 2208)]
        public void TestPart2(string inputFile, int turns, int expectedResult)
        {
            var result = Second(inputFile, turns);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
