using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dag13
{
    public struct BusInput
    {
        public long BusNumber { get; set; }
        public long SkipMinutes { get; set; }
    }
    [TestCategory("2020")]
    [TestClass]
    public class Program
    {
        static IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);

        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(string inputFile)
        {
            var input = Readlines(inputFile).ToList();
            var estimationTime = Convert.ToInt64(input[0]);
            var busInfo = input[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
            var busLines = busInfo.Select((bNr, i) => new { Index = i, BusNumber = bNr }).Where(l => l.BusNumber != "x")
                .Select(s => new { BusNumber = Convert.ToInt64(s.BusNumber), SkipMinutes = s.Index });
            var waitTimes = busLines.ToDictionary(b => b.BusNumber, b => FindWaitTime(estimationTime, b.BusNumber));
            var minWaitValue = waitTimes.Values.Min();
            var busNumber = waitTimes.First(w => w.Value == minWaitValue).Key;
            return busNumber * minWaitValue;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FindWaitTime(long estimationTime, long busNumber)
        {
            return busNumber - (estimationTime % busNumber);
        }
    
        static long Second(string inputFile)
        {
            var input = Readlines(inputFile).ToList();
            var busInfo = input[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
            return FindMagicTime(busInfo);
        }
        /// <summary>
        /// Solves the solution to
        /// Ni*ui ≡ 1 % ni
        /// by brute forcing all combiniations of ui
        /// The while loop is evaluated max ni times
        /// </summary>
        /// <param name="Ni"></param>
        /// <param name="ni"></param>
        /// <returns>ui</returns>
        private static long FindUi(long Ni, long ni)
        {
            var ui = 1L;
            while (true)
            {
                var remainder = Ni * ui % ni;
                if (remainder == 1) { return ui; }
                ui++;
            }
        }

        private static long FindMagicTime(IEnumerable<string> busInfo)
        {
            //Note that the buslines are all primes
            //And as this questions are about modulus, the Chinese Remainder Theorem can be applied
            //See: https://www.dave4math.com/mathematics/chinese-remainder-theorem/
            // First the bus question needs to transformed from Tfirstbus + skipMins to Tfirstbus
            // As:  (Tfirstbus + skipMinsBusNr) % BusNr = 0 <=> 
            //       Tfirstbus % BusNr = -skipMinsBusNr, but result of mod is always between 0 and (busNr-1), so
            //       Tfirstbus % BusNr = (BusNr-skipMinsBusNr) but on skipMinsBusNr, this should be 0, so Mod with BusNr
            //       Tfirstbus % BusNr = (BusNr-skipMinsBusNr) % busNr
            // solution is needed for all busses:
            // t ≡ (busnr - skipmins % busNr) % busNr 
            // so ni = busNr
            //    ai = (busnr - skipmins % busNr)
            //    Ni = N / ni
            var busLines = busInfo.Select((bNr, i) => new { Index = i, BusNumber = bNr }).Where(l => l.BusNumber != "x")
                .Select(s => new BusInput { BusNumber = Convert.ToInt64(s.BusNumber), SkipMinutes = s.Index }).ToList();
            //N = Σ ni for all busses
            var N = busLines.Select(b => b.BusNumber).Aggregate(1L, (product, busLine) => product *= busLine);
            //calculate all the parameters for each bus 
            var aiNiuis = busLines.Select(b => new { Bus = b, 
                ui = FindUi(N / b.BusNumber, b.BusNumber), 
                Ni = N/b.BusNumber,
                ai = (b.BusNumber - b.SkipMinutes) % b.BusNumber
            }).ToList();
            //the first t is now given as: Σ (ai * Ni * ui) (mod N)
            return aiNiuis.Sum(b => b.ai * b.Ni * b.ui) % N;
        }

        [DataTestMethod]
        [DataRow(939, 7, 6)]
        [DataRow(939, 59, 5)]
        [DataRow(939, 13, 10)]
        public void TestFindWaitTime(long estimationTime, long busNumber, long expectedResult)
        {
            var result = FindWaitTime(estimationTime, busNumber);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow(28, 3, 1)]
        [DataRow(21, 4, 1)]
        [DataRow(12, 7, 3)]
        public void TestFindUi(long Ni, long ni, long expectedResult)
        {
            var result = FindUi(Ni, ni);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow(new[] { "17", "x", "13", "19" }, 3417)]
        [DataRow(new[] { "67", "7", "59", "61" }, 754018)]
        [DataRow(new[] { "67", "x", "7", "59", "61" }, 779210)]
        [DataRow(new[] { "67", "7", "x", "59", "61" }, 1261476)]
        [DataRow(new[] { "1789", "37", "47", "1889" }, 1202161486)]
        public void TestFindMagicTime(string[] busses, long expectedResult)
        {
            var result = FindMagicTime(busses);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 295)]
        public void TestFirst(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 1068781)]
        public void TestSecond(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
