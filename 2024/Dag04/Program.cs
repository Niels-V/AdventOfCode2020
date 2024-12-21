using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC
{
    public class XmasMap : CharMapParser<char>
    {
        public int N { get; private set; }
        public char[,] Map { get; private set; }
        public int M { get; private set; }


        protected override char Convert(char input)
        {
            return input;
        }

        internal void Load(string inputFile)
        {
            var map = ReadMap(inputFile);
            Map = map;
            M = map.GetLength(0);//aantal rijen
            N = map.GetLength(1);//aantal kolommen
        }

        internal int FoundXmasWordsOnPosition(int i, int j)
        {
            //Check on X
            if (Map[i, j] != 'X') return 0;
            //Count the words
            var foundWords = 0;
            //Look in all eight directions
            for (int dirI = -1; dirI <= 1; dirI++)
            {
                for (int dirJ = -1; dirJ <= 1; dirJ++)
                {
                    //Skip the no direction
                    if (dirI==0 && dirJ == 0) continue;
                    //Find XMAS in the specified direction
                    if (FindXmas(i,j, dirI, dirJ)) { foundWords++; }
                }
            }
            //Return the count
            return foundWords;
        }
        internal bool FoundMasInXOnPosition(int i, int j)
        {
            //Check on A
            if (Map[i, j] != 'A') return false;
            //Look in all four diagonal directions
            if (i < 1 || j >= M-1) return false;
            if (j < 1 || j >= N-1) return false;

            return ((Map[i - 1, j + 1] == 'M' && Map[i + 1, j - 1] == 'S') || (Map[i - 1, j + 1] == 'S' && Map[i + 1, j - 1] == 'M')) &&
                 ((Map[i + 1, j + 1] == 'M' && Map[i - 1, j - 1] == 'S') || (Map[i + 1, j + 1] == 'S' && Map[i - 1, j - 1] == 'M'));
        }


        internal bool FindXmas(int i, int j, int dirI, int dirJ)
        {
            if (dirI<-1||dirI>1) throw new ArgumentOutOfRangeException(nameof(dirI));
            if (dirJ < -1 || dirJ > 1) throw new ArgumentOutOfRangeException(nameof(dirJ));
            var endI = i + 3 * dirI;
            var endJ = j + 3 * dirJ;
            if (endI<0 || endI >= M) return false;
            if (endJ < 0 || endJ >= N) return false;
            if (Map[i + 1 * dirI, j + 1 * dirJ] != 'M') return false;
            if (Map[i + 2 * dirI, j + 2 * dirJ] != 'A') return false;
            if (Map[i + 3 * dirI, j + 3 * dirJ] != 'S') return false;
            return true;
        }
    }

    [TestCategory("2024")]
    [TestClass]
    public partial class Program
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
            var wordMap = new XmasMap();
            wordMap.Load(inputFile);
            var foundWords = 0;
            for (var i = 0; i < wordMap.M; i++)
            {
                for (var j = 0; j < wordMap.N; j++)
                {
                    foundWords += wordMap.FoundXmasWordsOnPosition(i, j);
                }
            }
            return foundWords;
        }

        static long Second(string inputFile)
        {
            var wordMap = new XmasMap();
            wordMap.Load(inputFile);
            var foundWords = 0;
            for (var i = 1; i < wordMap.M-1; i++)
            {
                for (var j = 1; j < wordMap.N-1; j++)
                {
                    if (wordMap.FoundMasInXOnPosition(i, j))
                    {
                        foundWords++;
                    }
                }
            }
            return foundWords;
        }

        
        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(18, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(9, result);
        }

        
    }
}
