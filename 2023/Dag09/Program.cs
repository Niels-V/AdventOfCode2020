using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var parser = new IntSeriesParser(' ');
            var series = parser.ReadData(inputFile).Select(x => x.ToArray()).ToList();
            var extrapolations = series.Select(ExtrapolateForward);
            return extrapolations.Sum();
        }
        public static int ExtrapolateForward(int[] serie)
        {
            //Lagrange Interpolation Method
            var n = serie.Length;
            //Xi = i
            //Yi = serie[i]
            // xp is the x-value we want to calculate yp for, ie n+1
            var xp = n + 1;
            return Extrapolate(serie, n, xp);
        }
        public static int ExtrapolateBackward(int[] serie)
        {
            //Lagrange Interpolation Method
            var n = serie.Length;
            //Xi = i
            //Yi = serie[i]
            // xp is the x-value we want to calculate yp for, ie 0
            var xp = 0;
            return Extrapolate(serie, n, xp);
        }
        /// <summary>
        /// Extrapolate the serie with a polynomial, expecting a serie which is evenly spaced, ie it 
        /// is assumed Xi = {1, 2, 3, ... , n }. And Yi = { serie[0], serie[1], serie[2], ..., serie[n-1] }
        /// </summary>
        /// <param name="serie">The serie data</param>
        /// <param name="n">The number of data points.</param>
        /// <param name="xp">The x value the result needs to be calculated for.</param>
        /// <returns>The yp value</returns>
        /// <remarks>This method assumes the series and xp and yp are always integers. That depends on the input series.</remarks>
        private static int Extrapolate(int[] serie, int n, int xp)
        {
            double yp = 0;
            for (int i = 1; i <= n; i++)
            {
                double p = 1;
                for (int j = 1; j <= n; j++)
                {
                    if (i != j)
                    {
                        //Calculate p = p * (xp - Xj) / (Xi - Xj)
                        p = p * (xp - j) / (i - j);
                    }
                }
                //     Calculate yp = yp + p * Yi
                yp += p * serie[i - 1];
            }

            return Convert.ToInt32(yp);
        }

        static long Second(string inputFile)
        {
            var parser = new IntSeriesParser(' ');
            var series = parser.ReadData(inputFile).Select(x => x.ToArray()).ToList();
            var extrapolations = series.Select(ExtrapolateBackward);
            return extrapolations.Sum();
        }


        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(114, result);
        }
        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(2, result);
        }
    }
}
