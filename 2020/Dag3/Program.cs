using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dag3
{
    class Program
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        static bool[,] ReadMap(string filePath) {
            var lines = readlines(filePath).ToList();
            var firstLine = lines.First();
            var m = firstLine.Length;
            var n = lines.Count();
            var result = new bool[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = lines[i][j] == '#';
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            Debug.Assert(2 == CountTrees("test.txt", 1, 1));
            Debug.Assert(7 == CountTrees("test.txt", 3, 1));
            Debug.Assert(3 == CountTrees("test.txt", 5, 1));
            Debug.Assert(4 == CountTrees("test.txt", 7, 1));
            Debug.Assert(2 == CountTrees("test.txt", 1, 2));


            long treeCount = 1;
            treeCount *= CountTrees("input.txt",1,1);
            treeCount *= CountTrees("input.txt", 3, 1);
            treeCount *= CountTrees("input.txt", 5, 1);
            treeCount *= CountTrees("input.txt", 7, 1);
            treeCount *= CountTrees("input.txt", 1, 2);
            Console.WriteLine("Number of trees: {0}", treeCount);
        }

        private static int CountTrees(string filePath,  int stepx, int stepy)
        {
            var map = ReadMap(filePath);
            var posy = 0;
            var posx = 0;
            var treeCount = 0;
            var m = map.GetLength(1);
            var n = map.GetLength(0);
            while (posy < n - 1)
            {
                posx += stepx;
                if (posx >= m) { posx -= m; } //infinite repeat in x direction
                posy += stepy;
                if (map[posy, posx]) { treeCount++; }
            }

            return treeCount;
        }
    }
}
