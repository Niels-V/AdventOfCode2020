using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AoC
{
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
            var map = IntMapParser.Instance.ReadMap(inputFile);
            return CalculatePathRisk(map);
        }

        static long Second(string inputFile)
        {
            var mapSource = IntMapParser.Instance.ReadMap(inputFile);
            int[,] map = ExpandMap(mapSource, 5);
            return CalculatePathRisk(map);
        }

        private static long CalculatePathRisk(int[,] map)
        {
            var m = map.GetLength(0);
            var n = map.GetLength(1);

            //Determine initial path risks by adding the risk by traveling right and down, take the lowest amount of steps
            var pathRisk = new int[m, n];
            for (int i = 1; i < m; i++)
            {
                pathRisk[i, 0] = pathRisk[i - 1, 0] + map[i, 0];
            }
            for (int j = 1; j < n; j++)
            {
                pathRisk[0, j] = pathRisk[0, j - 1] + map[0, j];
            }
            for (int i = 1; i < m; i++)
            {
                for (int j = 1; j < n; j++)
                {
                    var lowestRisk = Math.Min(pathRisk[i - 1, j], pathRisk[i, j - 1]);
                    pathRisk[i, j] = lowestRisk + map[i, j];
                }
            }
            Console.WriteLine($"Current risk: {pathRisk[m - 1, n - 1]}");
            //There might be a route with more steps but lower risk, so recompute the risk by taking
            // all neighbourhs. And recompute until stable 
            var recompute = true;
            while (recompute)
            {
                recompute = false;
                var oldRisk = pathRisk;
                pathRisk = new int[m, n];
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        var fromTop = i > 0 ? oldRisk[i - 1, j] + map[i, j] : int.MaxValue;
                        var fromLeft = j > 0 ? oldRisk[i, j - 1] + map[i, j] : int.MaxValue;
                        var fromBottom = i < n - 1 ? oldRisk[i + 1, j] + map[i, j] : int.MaxValue;
                        var fromRight = j < m - 1 ? oldRisk[i, j + 1] + map[i, j] : int.MaxValue;
                        pathRisk[i, j] = new int[] { oldRisk[i, j], fromTop, fromLeft, fromBottom, fromRight }.Min();
                        if (pathRisk[i, j] < oldRisk[i, j]) { recompute = true; }
                    }
                }
                Console.WriteLine($"Current risk: {pathRisk[m - 1, n - 1]}");
            }
            return pathRisk[m - 1, n - 1];
        }

        private static int[,] ExpandMap(int[,] mapSource, int mapExpansion)
        {
            var mapH = mapSource.GetLength(0);
            var mapW = mapSource.GetLength(1);
            var map = new int[mapExpansion * mapH, mapExpansion * mapW];
            for (int i = 0; i < mapExpansion; i++)
            {
                for (int j = 0; j < mapExpansion; j++)
                {
                    var riskModifier = i + j;
                    for (int k = 0; k < mapH; k++)
                    {
                        for (int l = 0; l < mapW; l++)
                        {
                            var newValue = mapSource[k, l] + riskModifier;
                            newValue = newValue > 9 ? newValue - 9 : newValue;
                            map[k + i * mapH, l + j * mapW] = newValue;
                        }
                    }
                }
            }
            return map;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(40, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(315, result);
        }
    }
}
