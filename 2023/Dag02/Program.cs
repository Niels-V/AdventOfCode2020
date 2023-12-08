using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2023")]
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
            var parser = new GameParser();
            var gameData = parser.ReadData(inputFile);
            //there are only 12 red cubes, 13 green cubes, and 14 blue cubes
            var validGameIds = gameData
                .Where(game => game.IsValid(12, 13, 14))
                .Select(game => game.Id)
                .ToList();
            
            return validGameIds.Sum();
        }

        static long Second(string inputFile)
        {
            var parser = new GameParser();
            var gameData = parser.ReadData(inputFile);
            return gameData.Sum(game => game.FindPower());
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(2286, result);
        }
        [TestClass]
        public class Game {
            
            public int Id {get;set;}
            public List<GameSet> Sets {get;private set;} = new List<GameSet>();

            public bool IsValid(int maxRed, int maxGreen, int maxBlue) {
                return Sets.All(set=>set.Red <= maxRed && set.Blue <= maxBlue && set.Green <= maxGreen);
            }
            [TestMethod]
            public void TestIsValid()
            {
                var game = new Game { Id = 13 };
                game.Sets.Add(new GameSet { Blue = 3, Green = 12, Red = 12 });
                game.Sets.Add(new GameSet { Blue = 3, Green = 5, Red = 2 });
                game.Sets.Add(new GameSet { Blue = 7, Green = 3, Red = 13 });
                game.Sets.Add(new GameSet { Blue = 0, Green = 4, Red = 7 });
                game.Sets.Add(new GameSet { Blue = 3, Green = 3, Red = 5 });

                Assert.AreEqual(false, game.IsValid(12, 13, 14));
            }
            public int MinRed { get; set; }
            public int MinGreen { get; set; }
            public int MinBlue { get; set; }
            public long Power => MinRed * MinGreen * MinBlue;
            public long FindPower()
            {
                MinRed = 0;
                MinGreen = 0;
                MinBlue = 0;
                foreach (var set in Sets)
                {
                    MinRed = MinRed < set.Red ? set.Red : MinRed;
                    MinGreen = MinGreen < set.Green ? set.Green : MinGreen;
                    MinBlue = MinBlue < set.Blue ? set.Blue : MinBlue;
                }
                return Power;
            }
        }

        public class GameSet {
            public int Red {get;set;}
            public int Green {get;set;}
            public int Blue {get;set;}

        }
        [TestClass]
        public class GameParser : Common.Parser<Game> {
            protected override Game ParseLine(string line) {
                // line: Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
                // split ------|-----------------------------------------------
                var data = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                var id = data[0].Substring(5);
                var game = new Game{ Id=Convert.ToInt32(id)};
                //data[1]:  3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
                //split:   --------------|-----------------------|--------

                var sets = data[1].Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var set in sets) {
                    //set[0]:  3 blue, 4 red
                    //split:  -------|------
                    var setData = set.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var setInfo = new GameSet();
                    game.Sets.Add(setInfo);
                    foreach(string colorInfo in setData) {
                        var colorData = colorInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        switch (colorData[1]) {
                            case "red":
                                setInfo.Red = Convert.ToInt32(colorData[0]);
                                break;
                            case "green":
                                setInfo.Green = Convert.ToInt32(colorData[0]);
                                break;
                            case "blue":
                                setInfo.Blue = Convert.ToInt32(colorData[0]);
                                break;
                        }
                    }

                }
                return game;
           }
           [TestMethod]
           public void TestSet() {
            string s = "Game 10: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green";
            var result = ParseLine(s);
            Assert.AreEqual(10, result.Id);
            Assert.AreEqual(3, result.Sets.Count());
            Assert.AreEqual(0, result.Sets.First().Green);
            Assert.AreEqual(3, result.Sets.First().Blue);
            Assert.AreEqual(4, result.Sets.First().Red);

           }
            [TestMethod]
            public void TestSet2()
            {
                string s = "Game 13: 3 blue, 12 red, 12 green; 5 green, 3 blue, 2 red; 3 green, 7 blue, 13 red; 4 green, 7 red; 3 green, 3 blue, 5 red";
                var result = ParseLine(s);
                Assert.AreEqual(13, result.Id);
                Assert.AreEqual(5, result.Sets.Count());
                Assert.AreEqual(12, result.Sets.First().Green);
                Assert.AreEqual(3, result.Sets.First().Blue);
                Assert.AreEqual(12, result.Sets.First().Red);

            }
        }
    }
}
