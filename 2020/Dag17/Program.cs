using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag17
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
            var steps = 6;
            var cubeMap = CubeMapParser.Instance.ReadMap(inputFile);
            var activePositions = FindActivePositions(cubeMap);
            var game = new CubeGameOfLife();
            game.FillStart(activePositions);
            for (int i = 1; i <= steps; i++)
            {
                game.DoTurn(i);
            }
            var activeCubes = game.Cubes.Values.Count(c=>c.IsActiveInTurn(steps));
            return activeCubes;
        }

        private static IEnumerable<Position> FindActivePositions(CubeStatus[,] cubeMap)
        {
            var n = cubeMap.GetLength(0);
            var m = cubeMap.GetLength(1);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (cubeMap[i, j] == CubeStatus.Active)
                    {
                        yield return new Position(i, j, 0);
                    }
                }
            }
        }
        private static IEnumerable<HyperPosition> FindActiveHyperPositions(CubeStatus[,] cubeMap)
        {
            var n = cubeMap.GetLength(0);
            var m = cubeMap.GetLength(1);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (cubeMap[i, j] == CubeStatus.Active)
                    {
                        yield return new HyperPosition(i, j, 0, 0);
                    }
                }
            }
        }

        static int Second(string inputFile)
        {
            var steps = 6;
            var cubeMap = CubeMapParser.Instance.ReadMap(inputFile);
            var activePositions = FindActiveHyperPositions(cubeMap);
            var game = new HyperCubeGameOfLife();
            game.FillStart(activePositions);
            for (int i = 1; i <= steps; i++)
            {
                game.DoTurn(i);
            }
            var activeCubes = game.Cubes.Values.Count(c => c.IsActiveInTurn(steps));
            return activeCubes;
        }


        [DataTestMethod]
        [DataRow("test.txt", 112)]
        public void TestPart1(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", 848)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
