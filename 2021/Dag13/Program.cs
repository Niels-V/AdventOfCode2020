using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
            Console.WriteLine("Program succes, found result: {0}", Environment.NewLine+result2);
        }

        static long First(string inputFile)
        {
            var sheet = new Parser().ParseFile(inputFile);
            var foldInstruction = sheet.Instruction.First();

            var newPoints = new HashSet<Point>();
            foreach (var point in sheet.Points)
            {
                var newPoint = PositionAfterFold(point, foldInstruction);
                if (!newPoints.Contains(newPoint))
                {
                    newPoints.Add(newPoint);
                }
            }
            return newPoints.LongCount();
        }

        static Point PositionAfterFold(Point point, FoldInstruction instruction)
        {
            if (instruction.Direction == FoldDirection.Horizontal)
            {
                var y = point.Y > instruction.Position ? 2 * instruction.Position - point.Y : point.Y;
                return new Point(point.X, y);
            }
            var x = point.X > instruction.Position ? 2 * instruction.Position - point.X : point.X;

            return new Point(x, point.Y);
        }

        static string Second(string inputFile)
        {
            var sheet = new Parser().ParseFile(inputFile);
            var points = sheet.Points.ToHashSet();
            foreach (var foldInstruction in sheet.Instruction)
            {
                var newPoints = new HashSet<Point>();
                foreach (var point in points)
                {
                    var newPoint = PositionAfterFold(point, foldInstruction);
                    if (!newPoints.Contains(newPoint))
                    {
                        newPoints.Add(newPoint);
                    }
                }
                points = newPoints;
            }

            var width = points.Max(p => p.X);
            var height = points.Max(p => p.Y);
            var sb = new StringBuilder((width + 3) * height+1);
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    sb.Append(points.Contains(new Point(x, y)) ? "#" : ".");
                }

                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(17, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(@"#####
#...#
#...#
#...#
#####
", result);
        }
    }
    public class Sheet
    {
        public List<Point> Points { get; private set; } = new List<Point>();
        public List<FoldInstruction> Instruction { get; private set; } = new List<FoldInstruction>();
    }
    public class FoldInstruction
    {
        public int Position { get; set; }
        public FoldDirection Direction { get; set; }
    }

    public class Parser : LineParser
    {
        static readonly Regex pointRule = new Regex("^(?<X>\\d+),(?<Y>\\d+)$", RegexOptions.Compiled);
        static readonly Regex foldRule = new Regex("^fold along (?<direction>y|x)=(?<position>\\d+)$", RegexOptions.Compiled);

        public Sheet ParseFile(string filePath)
        {
            var sheet = new Sheet();
            var mode = 0;
            foreach (var line in ReadData(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    mode++;
                    continue;
                }
                if (mode == 0)
                {
                    var regResult = pointRule.Match(line);
                    var point = new Point(Convert.ToInt32(regResult.Groups.Values.First(g => g.Name == "X").Captures[0].Value)
                        , Convert.ToInt32(regResult.Groups.Values.First(g => g.Name == "Y").Captures[0].Value)
                    );
                    sheet.Points.Add(point);
                }
                else if (mode == 1)
                {
                    var foldResult = foldRule.Match(line);
                    var foldInstruction = new FoldInstruction
                    {
                        Direction = foldResult.Groups.Values.First(g => g.Name == "direction").Captures[0].Value == "x" ? FoldDirection.Vertical : FoldDirection.Horizontal,
                        Position = Convert.ToInt32(foldResult.Groups.Values.First(g => g.Name == "position").Captures[0].Value)
                    };

                    sheet.Instruction.Add(foldInstruction);
                }
            }
            return sheet;
        }
    }
    public enum FoldDirection
    {
        Horizontal,Vertical
    }
}