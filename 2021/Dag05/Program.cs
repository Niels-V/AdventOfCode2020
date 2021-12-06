using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AoC
{
    [DebuggerDisplay("({X},{Y})")]
    public struct Point { 
        public int X { get; set; }
        public int Y { get; set; }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X==p2.X && p1.Y==p2.Y;
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1==p2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point point)
            {
                return X == point.X && Y == point.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
    public struct Line {
        public Point Start { get; set; }
        public Point End { get; set; }

        public Orientation Orientation
        {
            get => GetOrientation();
        }

        public IEnumerable<Point> Points(bool diagonals=false)
        {
            if (Orientation == Orientation.NoLine)
            {
                yield return Start;
            }
            else if (Orientation == Orientation.Horizontal)
            {
                int end = End.X;
                int start = Start.X;
                int stepSize = (end > start) ? 1 : -1;
                int steps = (end - start) * stepSize;
                for (int step = 0; step <= steps; step++)
                {
                    yield return new Point { X = Start.X + step * stepSize, Y = Start.Y };
                }
            }
            else if (Orientation == Orientation.Vertical)
            {
                int end = End.Y;
                int start = Start.Y;
                int stepSize = (end > start) ? 1 : -1;
                int steps = (end - start) * stepSize;
                for (int step = 0; step <= steps; step++)
                {
                    yield return new Point { X = Start.X , Y = Start.Y + step * stepSize };
                }
            }
            else if (diagonals && Orientation == Orientation.NotStraight)
            {
                int endY = End.Y;
                int startY = Start.Y;
                int endX = End.X;
                int startX = Start.X;
                int stepSizeX = (endX > startX) ? 1 : -1;
                int stepSizeY = (endY > startY) ? 1 : -1;
                int steps = (endX - startX) * stepSizeX; // assume either X or Y gives same results
                for (int step = 0; step <= steps; step++)
                {
                    yield return new Point { X = Start.X + step * stepSizeX, Y = Start.Y + step * stepSizeY };
                }
            }
            yield break;
        }

        private Orientation GetOrientation()
        {
            if (Start == End) { return Orientation.NoLine; }
            if (Start.X == End.X) { return Orientation.Vertical; }
            if (Start.Y == End.Y) { return Orientation.Horizontal; }
            return Orientation.NotStraight;
        }
    }

    public enum Orientation
    {
        NotStraight,
        Horizontal,
        Vertical,
        NoLine
    }

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
            return CaculateOverlap(inputFile, false);
        }
        static long Second(string inputFile)
        {
            return CaculateOverlap(inputFile, true);
        }

        private static long CaculateOverlap(string inputFile, bool useDiagonals)
        {
            var lines = new Parser().ReadData(inputFile);
            var hits = new Dictionary<Point, int>();
            foreach (var line in lines)
            {
                foreach (var point in line.Points(useDiagonals))
                {
                    if (hits.ContainsKey(point))
                    {
                        hits[point] = hits[point] + 1;
                    }
                    else
                    {
                        hits.Add(point, 1);
                    }
                }
            }
            return hits.Values.Count(v => v > 1);
        }
             

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(12, result);
        }
    }
}
