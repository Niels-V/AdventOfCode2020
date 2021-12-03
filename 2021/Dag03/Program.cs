using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace Dag03
{
    public class Parser : CharMapParser<bool>
    {
        protected override bool Convert(char input)
        {
            if (input=='1') { return true; }
            if (input == '0') { return false; }
            throw new InvalidOperationException("Unknown input");
        }
    }
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

        internal static int ToInt32(BitArray bitArray)
        {
            var array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        static long First(string inputFile)
        {
            var boolMap = new Parser().ReadMap(inputFile);
            var m = boolMap.GetLength(0);//aantal rijen
            var n = boolMap.GetLength(1);//aantal kolommen
            
            var gamma = new BitArray(n);
            var epsilon = new BitArray(n);

            for (int j = 0; j < n; j++)
            {
                var trueCount = 0;
                for (int i = 0; i < m; i++)
                {
                    if (boolMap[i, j]) { trueCount++; }
                }
                if (trueCount > m/2)
                {
                    //BitArrats werken van least significant naar most significant, dus vul de bit array van rechts naar links
                    gamma.Set(n-j-1, true);
                } 
                else
                {
                    epsilon.Set(n-j-1, true);
                }
            }
            var gammaValue = ToInt32(gamma);
            var epsilonValue = ToInt32(epsilon);

            return gammaValue * epsilonValue;
        }

        static int Second(string inputFile)
        {
            var boolMap = new Parser().ReadMap(inputFile);
            var m = boolMap.GetLength(0);//aantal rijen
            var n = boolMap.GetLength(1);//aantal kolommen

            var o2Map = (bool[,])boolMap.Clone();
            var co2Map = (bool[,])boolMap.Clone();
            
            for (int j = 0; j < n; j++)
            {
                o2Map = FindO2Map(o2Map, j);
                co2Map = FindCO2Map(co2Map, j);
            }
            BitArray o2Rating = new BitArray(n);
            BitArray co2Rating = new BitArray(n);
            for (int j = 0; j < n; j++)
            {
                o2Rating.Set(n - j - 1, o2Map[0,j]);
                co2Rating.Set(n - j - 1, co2Map[0, j]);
            }
            var o2RatingValue  = ToInt32(o2Rating);
            var co2RatingValue = ToInt32(co2Rating);

            return o2RatingValue * co2RatingValue;
        }

        private static bool[,] FindO2Map(bool[,] boolMap, int iteration)
        {
            var m = boolMap.GetLength(0);//aantal rijen
            var n = boolMap.GetLength(1);//aantal kolommen
            if (m==1)
            {
                return boolMap;
            }
            var trueCount = 0;
            for (int i = 0; i < m; i++)
            {
                if (boolMap[i, iteration]) { trueCount++; }
            }
            var useBool = trueCount >= m / 2d;
            var newRows = useBool ? trueCount : m - trueCount;
            var newMap = new bool[newRows, n];
            var newRowIndex = 0;
            for (int i = 0; i < m; i++)
            {
                if (boolMap[i, iteration] == useBool)
                {
                    for (int j = 0; j < n; j++)
                    {
                        newMap[newRowIndex, j] = boolMap[i, j];
                    }
                    newRowIndex++;
                }
            }
            return newMap;
        }

        private static bool[,] FindCO2Map(bool[,] boolMap, int iteration)
        {
            var m = boolMap.GetLength(0);//aantal rijen
            var n = boolMap.GetLength(1);//aantal kolommen
            if (m == 1)
            {
                return boolMap;
            }
            var trueCount = 0;
            for (int i = 0; i < m; i++)
            {
                if (boolMap[i, iteration]) { trueCount++; }
            }
            var useBool = trueCount < m / 2d;
            var newRows = useBool ? trueCount : m - trueCount;
            var newMap = new bool[newRows, n];
            var newRowIndex = 0;
            for (int i = 0; i < m; i++)
            {
                if (boolMap[i, iteration] == useBool)
                {
                    for (int j = 0; j < n; j++)
                    {
                        newMap[newRowIndex, j] = boolMap[i, j];
                    }
                    newRowIndex++;
                }
            }
            return newMap;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(198, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(230, result);
        }
    }
}
