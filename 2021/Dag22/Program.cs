using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC
{
    [TestCategory("2021")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            //var result = First("input.txt");
            //Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(string inputFile)
        {
            var rebootsteps = new Parser().ReadData(inputFile);
            //var activePoints = new HashSet<Cube>();
            bool[] activeBootCubes = new bool[101 * 101 * 101];
            foreach (var step in rebootsteps)
            {
                var xmin = Math.Max(step.Xmin, -50);
                var xmax = Math.Min(step.Xmax, 50);
                var ymin = Math.Max(step.Ymin, -50);
                var ymax = Math.Min(step.Ymax, 50);
                var zmin = Math.Max(step.Zmin, -50);
                var zmax = Math.Min(step.Zmax, 50);
                if (xmin > 50 || xmax < -50 || ymin > 50 || ymax < -50 || zmin > 50 || zmax < -50) { continue; }
                var xrange = System.Linq.Enumerable.Range(xmin, xmax - xmin + 1);
                var yrange = System.Linq.Enumerable.Range(ymin, ymax - ymin + 1);
                var zrange = System.Linq.Enumerable.Range(zmin, zmax - zmin + 1);

                var pointIndex = xrange.SelectMany(x => yrange.SelectMany(y => zrange.Select(z => (x + 50) + 101 * (y + 50) + 101 * 101 * (z + 50))));

                foreach (var index in pointIndex)
                {
                    activeBootCubes[index] = step.InstructionOn;
                }
            }
            return activeBootCubes.Count(c => c);
        }

        static long Second(string inputFile)
        {
            var rebootsteps = new Parser().ReadData(inputFile).ToList();
            var xborders = rebootsteps.SelectMany(r => r.X).OrderBy(i => i).Distinct().SelectMany(i => System.Linq.Enumerable.Range(i - 1, 3)).Distinct().OrderBy(i => i).ToList();
            var yborders = rebootsteps.SelectMany(r => r.Y).OrderBy(i => i).Distinct().SelectMany(i => System.Linq.Enumerable.Range(i - 1, 3)).Distinct().OrderBy(i => i).ToList();
            var zborders = rebootsteps.SelectMany(r => r.Z).OrderBy(i => i).Distinct().SelectMany(i => System.Linq.Enumerable.Range(i - 1, 3)).Distinct().OrderBy(i => i).ToList();
            var sum = CountSegments(xborders, yborders, zborders, rebootsteps);

            return sum;
        }

        private static long CountSegments(List<int> xborders, List<int> yborders, List<int> zborders, List<RebootStep> rebootsteps)
        {
            var xsegments = xborders.Count;
            var ysegments = yborders.Count;
            var zsegments = zborders.Count;
            var sum = 0L;
            var minX = xborders[0];
            var minY = yborders[0];
            var minZ = zborders[0];
            var minXY = minY;
            var minYZ = minZ;
            Console.WriteLine($"Counting progress of lines in x direction. Total of {xsegments}:");
            Console.Write("Line: 0000");
            var sw = new Stopwatch();sw.Start();
            for (int i = 1; i < xsegments; i++)
            {
                Console.Write("\b\b\b\b" + string.Format("{0:D4}", i));

                var maxX = xborders[i] - 1;

                for (int j = 1; j < ysegments; j++)
                {
                    var maxY = yborders[j] - 1;
                    for (int k = 1; k < zsegments; k++)
                    {
                        var maxZ = zborders[k] - 1;
                        var blockStatus = false;
                        foreach (var rebootstep in rebootsteps)
                        {
                            var instruction = rebootstep.GetInstruction(minX, minY, minZ);
                            blockStatus = instruction.HasValue ? instruction.Value : blockStatus;
                        }
                        if (blockStatus)
                        {
                            sum += ((long)(maxX - minX + 1)) * ((long)(maxY - minY + 1)) * ((long)(maxZ - minZ + 1));
                        }
                        minZ = maxZ + 1;
                    }
                    minZ = minYZ;
                    minY = maxY + 1;
                }

                minY = minXY;
                minX = maxX + 1;
            }
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine(sw.Elapsed.ToString());
            return sum;
        }



        [TestMethod]
        public void TestPart1()
        {
            var result = First("test-1.txt");
            Assert.AreEqual(39, result);
        }
        [TestMethod]
        public void TestPart1b()
        {
            var result = First("test-2.txt");
            Assert.AreEqual(590784, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test-3.txt");
            Assert.AreEqual(2758514936282235, result);
        }
        [TestMethod]
        public void TestPart2b()
        {
            var result = Second("test-1.txt");
            Assert.AreEqual(39, result);
        }
    }
    public class RebootStep
    {
        public bool InstructionOn { get; set; }
        public int Xmin { get; set; }
        public int Ymin { get; set; }
        public int Zmin { get; set; }
        public int Xmax { get; set; }
        public int Ymax { get; set; }
        public int Zmax { get; set; }
        public int[] X => new[] { Xmin, Xmax };
        public int[] Y => new[] { Ymin, Ymax };
        public int[] Z => new[] { Zmin, Zmax };
        public bool? GetInstruction(int x, int y, int z)
        {
            return (Xmin <= x && x <= Xmax &&
                Ymin <= y && y <= Ymax &&
                Zmin <= z && z <= Zmax) ? InstructionOn : null;
        }
    }

    public class Parser : Parser<RebootStep>
    {
        static readonly Regex instructionRule = new("^(?<operation>on|off)\\sx=(?<xmin>-?\\d+)..(?<xmax>-?\\d+),y=(?<ymin>-?\\d+)..(?<ymax>-?\\d+),z=(?<zmin>-?\\d+)..(?<zmax>-?\\d+)$", RegexOptions.Compiled);

        protected override RebootStep ParseLine(string line)
        {
            var regexResult = instructionRule.Match(line);

            var instruction = new RebootStep
            {
                InstructionOn = regexResult.Groups.Values.First(g => g.Name == "operation").Value == "on",
                Xmin = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "xmin").Value),
                Xmax = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "xmax").Value),
                Ymin = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "ymin").Value),
                Ymax = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "ymax").Value),
                Zmin = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "zmin").Value),
                Zmax = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "zmax").Value)
            };
            return instruction;
        }
    }

    public struct Cube
    {
        public Cube(int x, int y, int z//, int w
                                       )
        {
            X = x;
            Y = y;
            Z = z;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public override string ToString() => $"[{X},{Y},{Z}]";
    }
}
