using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
            var map = HeightmapParser.Instance.ReadMap(inputFile);
            var m = map.GetLength(0);
            var n = map.GetLength(1);
            var accumulatedRiskLevel = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((j == 0 || map[i, j - 1] > map[i, j]) &&
                        (i == 0 || map[i - 1, j] > map[i, j]) &&
                        (i == m - 1 || map[i + 1, j] > map[i, j]) &&
                        (j == n - 1 || map[i, j + 1] > map[i, j]))
                    {
                        //local minimum
                        accumulatedRiskLevel += map[i, j] + 1;
                    }
                }
            }
            return accumulatedRiskLevel;
        }

        static long Second(string inputFile)
        {
            var map = HeightmapParser.Instance.ReadMap(inputFile);
            var m = map.GetLength(0);
            var n = map.GetLength(1);
            //I will just keep track of basin number in basin index,
            //and count the number of points added to that basin in basinSizes.
            //For this, points are evaluated from topleft to bottom right,
            // so you only need to check the top and left point. When the
            // point joins two different basins, those are interconnected
            // and added up to one basin.

            var basinIndex = new int[m, n];
            var basinSizes = new Dictionary<int, int>();
            int lastBasinNumber = 1;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        //start with the first basin. Manually check that this isn't a boundary
                        basinIndex[i, j] = lastBasinNumber;
                        basinSizes.Add(1, 1);
                        continue;
                    }
                    //current point is a boundary
                    if (map[i, j] == 9) { basinIndex[i, j] = -1; continue; }

                    if ((j == 0 || map[i, j - 1] == 9) &&
                        (i == 0 || map[i - 1, j] == 9))
                    {
                        //left and upper are boundaries, add new basin
                        basinIndex[i, j] = ++lastBasinNumber;
                        basinSizes.Add(lastBasinNumber, 1);
                        continue;
                    }
                    var leftBasin = j > 0 ? basinIndex[i, j - 1] : -1;
                    var topBasin = i > 0 ? basinIndex[i - 1, j] : -1;
                    if (topBasin > 0 && leftBasin > 0)
                    {
                        //Both top and left are basins
                        if (topBasin == leftBasin)
                        {
                            //but they are the same, just add the current point to the same basin
                            basinIndex[i, j] = topBasin;
                            basinSizes[topBasin] = basinSizes[topBasin] + 1;
                        }
                        else
                        {
                            //both are different, join the left basin with the top one.
                            basinIndex[i, j] = topBasin;
                            basinSizes[topBasin] = basinSizes[topBasin] + 1;
                            JoinBasin(topBasin, leftBasin, basinIndex, basinSizes);
                        }
                    }
                    else if (topBasin > 0)
                    {
                        //Only the top is a basin, so left is boundary, add point to top basin
                        basinIndex[i, j] = topBasin;
                        basinSizes[topBasin] = basinSizes[topBasin] + 1;
                    }
                    else if (leftBasin > 0)
                    {
                        //Only the left is a basin, so top is boundary, add point to left basin
                        basinIndex[i, j] = leftBasin;
                        basinSizes[leftBasin] = basinSizes[leftBasin] + 1;
                    }
                }
            }
            //Take the 3 largest basins, and multiply their values.
            return basinSizes.Values.OrderByDescending(i=>i).Take(3).Aggregate(1,(i1,i2)=>i1*i2);
        }

        private static void JoinBasin(int topBasin, int leftBasin, int[,] basinIndex, Dictionary<int,int> basinSizes)
        {
            var m = basinIndex.GetLength(0);
            var n = basinIndex.GetLength(1);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (basinIndex[i,j] == leftBasin) { basinIndex[i, j] = topBasin; }
                }
            }
            basinSizes[topBasin]+=basinSizes[leftBasin];
            basinSizes[leftBasin] = 0;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(1134, result);
        }
    }

    public class HeightmapParser : CharMapParser<int>
    {
        private static HeightmapParser instance = null;
        public static HeightmapParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HeightmapParser();
                }
                return instance;
            }
        }
        protected override int Convert(char input) => System.Convert.ToInt32(input.ToString());
    }
}
