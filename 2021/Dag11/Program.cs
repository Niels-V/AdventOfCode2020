using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            HashSet<Octopus> set = Setup(map);
            for (int step = 1; step <= 100; step++)
            {
                foreach (var octopus in set)
                {
                    octopus.Increase();
                }
                foreach (var octopus in set)
                {
                    octopus.DoTurn(step, false);
                }
                foreach (var octopus in set)
                {
                    octopus.Reset(step);
                }
            }
            return set.Aggregate(0L, (sum, oct) => sum + oct.FlashCount);
        }

        private static HashSet<Octopus> Setup(int[,] map)
        {
            var m = map.GetLength(0);
            var n = map.GetLength(1);
            HashSet<Octopus> set = new(n * m);
            Octopus[,] mapOct = new Octopus[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var octopus = new Octopus { Energy = map[i, j] };
                    mapOct[i, j] = octopus;
                    if (i > 0 && j > 0)
                    {
                        octopus.NW = mapOct[i - 1, j - 1];
                        octopus.NW.SE = octopus;
                    }
                    if (i > 0)
                    {
                        octopus.N = mapOct[i - 1, j];
                        octopus.N.S = octopus;
                    }
                    if (i > 0 && j < n - 1)
                    {
                        octopus.NE = mapOct[i - 1, j+1];
                        octopus.NE.SW = octopus;
                    }
                    if (j > 0)
                    {
                        octopus.W = mapOct[i, j - 1];
                        octopus.W.E = octopus;
                    }
                    set.Add(octopus);
                }
            }

            return set;
        }

        static long Second(string inputFile)
        {
            var map = IntMapParser.Instance.ReadMap(inputFile);
            HashSet<Octopus> set = Setup(map);
            for (int step = 1; step <= 10000; step++)
            {
                foreach (var octopus in set)
                {
                    octopus.Increase();
                }
                foreach (var octopus in set)
                {
                    octopus.DoTurn(step, false);
                }
                foreach (var octopus in set)
                {
                    octopus.Reset(step);
                }
                if (set.All(oct => oct.Energy == 0)) { return step; }
            }
            return -1;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(1656, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(195, result);
        }
    }
    [DebuggerDisplay("{Energy}")]

    public class Octopus
    {
        public Octopus()
        {
            LastFlashTurn = -1;
            FlashCount = 0;
        }
        public int Energy { get; set; }
        public int LastFlashTurn { get; set; }
        public int FlashCount { get; set; }
        public Octopus NW { get; set; }
        public Octopus N { get; set; }
        public Octopus NE { get; set; }
        public Octopus W { get; set; }
        public Octopus E { get; set; }
        public Octopus SW { get; set; }
        public Octopus S { get; set; }
        public Octopus SE { get; set; }

        public void DoTurn(int turnNumber, bool increase)
        {
            if (increase) { Energy++; }
            if (Energy > 9 && LastFlashTurn!=turnNumber)
            {
                Flash(turnNumber);
            }
        }

        public void Flash(int turnNumber)
        {
            LastFlashTurn = turnNumber;
            FlashCount++;
            NW?.DoTurn(turnNumber, true);
            N?.DoTurn(turnNumber, true);
            NE?.DoTurn(turnNumber, true);
            W?.DoTurn(turnNumber, true);
            E?.DoTurn(turnNumber, true);
            SW?.DoTurn(turnNumber, true);
            S?.DoTurn(turnNumber, true);
            SE?.DoTurn(turnNumber, true);

        }

        internal void Increase()
        {
            Energy++;
        }

        internal void Reset(int step)
        {
            if (Energy>9) { Energy = 0; }
        }
    }

}
