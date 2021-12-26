using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Common.CubeAlgebra;

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

            var representation = rebootsteps.Select(r => (instruction:r, cube: new AABB(r.Xmin, r.Ymin, r.Zmin, r.Xmax, r.Ymax, r.Zmax))).ToList();
            var cubes = new List<AABB>
            {
                representation[0].cube
            };
            for (int i = 1; i < representation.Count; i++)
            {
                var newCubes = new List<AABB>();
                var newCube = representation[i].cube;
                
                var notIntersects = cubes.Where(c => !c.IntersectWith(newCube));
                newCubes.AddRange(notIntersects);
                cubes.RemoveAll(c => !c.IntersectWith(newCube));
                //remove all contained in newCube
                cubes.RemoveAll(c => newCube.Contains(c));
                newCubes.AddRange(cubes.SelectMany(c => c.Remove(newCube)));
                cubes = newCubes;

                if (representation[i].instruction.InstructionOn)
                {
                    //of cube instruction is on, add newcube
                    cubes.Add(newCube);
                }
            }

            var sum = cubes.Sum(c=>c.Volume);

            return sum;
        }

        [TestMethod]
        public void TestCubeRemove()
        {
            //on x = 10..12, y = 10..12, z = 10..12
            //on x = 11..13, y = 11..13, z = 11..13
            var c1 = new AABB(10, 10, 10, 12, 12, 12);
            var c2 = new AABB(11, 11, 11, 13, 13, 13);
            var c3 = c1.Remove(c2);
            Assert.AreEqual(3, c3.Count());
            Assert.AreEqual(19, c3.Sum(c => c.Volume));
        }

        [TestMethod]
        public void TestCubeRemove2()
        {
            var c1 = new AABB(-1, -1, -1, 1, 1, 1);
            var c2 = new AABB(0, 0, 0, 2, 2, 2);
            var c3 = c1.Remove(c2);
            Assert.AreEqual(3, c3.Count());
            Assert.AreEqual(19, c3.Sum(c => c.Volume));
        }

        [TestMethod]
        public void TestCubeRemove3()
        {
            //on x = 10..12, y = 10..12, z = 10..12
            //on x = 11..13, y = 11..13, z = 11..13
            var c1 = new AABB(-10, -10, -10, -8, -8, -8);
            var c2 = new AABB(9, 9, 9, 11, 11, 11);
            var c3 = c1.Remove(c2);
            Assert.AreEqual(1, c3.Count());
            Assert.AreEqual(27, c3.Sum(c => c.Volume));
        }

        [TestMethod]
        public void TestCubeRemove4()
        {
            //on x = 10..12, y = 10..12, z = 10..12
            //on x = 11..13, y = 11..13, z = 11..13
            var c1 = new AABB(10, 10, 10, 10, 10, 10);
            var c2 = new AABB(9, 9, 9, 11, 11, 11);
            var c3 = c1.Remove(c2);
            Assert.AreEqual(0, c3.Count());
            Assert.AreEqual(0, c3.Sum(c => c.Volume));
        }
        [TestMethod]
        public void TestCubeRemove5()
        {
            //on x = 10..12, y = 10..12, z = 10..12
            //on x = 11..13, y = 11..13, z = 11..13
            var c1 = new AABB(10, 10, 10, 10, 10, 10);
            var c2 = new AABB(9, 9, 9, 11, 11, 11);
            var c3 = c2.Remove(c1);
            Assert.AreEqual(6, c3.Count());
            Assert.AreEqual(26, c3.Sum(c => c.Volume));
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
}
