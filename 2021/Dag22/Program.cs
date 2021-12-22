using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
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

                //activeBootCubes.SetValue(step.InstructionOn, pointIndex.ToArray());
            }
            return activeBootCubes.Count(c => c);
        }

        public class BlockCollection
        {
            public int XB { get; set; }
            public int YB { get; set; }
            public int ZB { get; set; }

            public int Xmin { get; set; }
            public int Xmax { get; set; }
            public int Ymin { get; set; }
            public int Ymax { get; set; }
            public int Zmin { get; set; }
            public int Zmax{ get; set; }

            private readonly ContinousCubeGroup[,,] blocks;
            public BlockCollection(int countX, int countY, int countZ, int segment)
            {
                XB = countX / 4;
                YB = countY / 2;
                ZB = countZ / 2;
                switch (segment)
                {
                    case 0:
                        Xmin = 0;
                        Xmax = XB; 
                        Ymin = 0;
                        Ymax = YB; 
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 1:
                        Xmin = 0; Ymin = YB; Zmin = 0;
                        Xmax = XB; Ymax = countY-YB; Zmax = ZB;
                        break;
                    case 2:
                        Xmin = XB; 
                        Xmax = 2*XB; 
                        Ymin = 0; Zmin = 0;
                        Ymax = YB; Zmax = ZB;
                        break;
                    case 3:
                        Xmin = XB; Ymin = YB; 
                        Xmax = 2*XB; Ymax = countY - YB; 
                        Zmin = 0;
                        Zmax = ZB; 
                        break;
                    case 4:
                        Xmin = 0; Ymin = 0; Zmin = ZB;
                        Xmax = XB; Ymax = YB; Zmax = countZ-ZB;
                        break;
                    case 5:
                        Xmin = 0; 
                        Xmax = XB; 
                        Ymin = YB; 
                        Ymax = countY-YB; 
                        Zmin = ZB;
                        Zmax = countZ - ZB;
                        break;
                    case 6:
                        Xmin = XB;
                        Xmax = 2 * XB;
                        Ymin = 0;
                        Ymax = YB;
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 7:
                        Xmin = XB;
                        Xmax = 2 * XB;
                        Ymin = YB;
                        Ymax = countY - YB;
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 8:
                        Xmin = 2*XB;
                        Xmax = 3 * XB;
                        Ymin = 0;
                        Ymax = YB;
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 9:
                        Xmin = 2 * XB;
                        Xmax = 3 * XB;
                        Ymin = YB;
                        Ymax = countY - YB;
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 10:
                        Xmin = 3 * XB;
                        Xmax = countX-3 * XB;
                        Ymin = 0;
                        Ymax = YB;
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 11:
                        Xmin = 2 * XB;
                        Xmax = 3 * XB;
                        Ymin = 0;
                        Ymax = YB;
                        Zmin = ZB;
                        Zmax = countZ - ZB;
                        break;
                    case 12:
                        Xmin = 2 * XB;
                        Xmax = 3 * XB;
                        Ymin = YB;
                        Ymax = countY - YB;
                        Zmin = ZB;
                        Zmax = countZ - ZB;
                        break;
                    case 13:
                        Xmin = 3 * XB;
                        Xmax = countX - 3 * XB;
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 14:
                        Xmin = 3 * XB;
                        Xmax = countX - 3 * XB;
                        Ymin = 0;
                        Ymax = YB;
                        Zmin = ZB;
                        Zmax = countZ - ZB;
                        Zmin = 0;
                        Zmax = ZB;
                        break;
                    case 15:
                        Xmin = 3 * XB;
                        Xmax = countX - 3 * XB;
                        Ymin = YB;
                        Ymax = countY - YB;
                        Zmin = ZB;
                        Zmax = countZ - ZB;
                        break;
                    default:
                        break;
                }
                blocks = new ContinousCubeGroup[Xmax-Xmin, Ymax-Ymin, Zmax-Zmin];
            }
            public ContinousCubeGroup Item(int x, int y, int z)
            {
                if (x>=Xmax)
                return blocks111!= null && x < XB && y < YB && z < ZB ? blocks111[x, y, z] :
                     blocks121!= null &&x < XB && y >= YB && z < ZB ? blocks121[x, y - YB, z] :
                     blocks211!= null &&x >= XB && x < 2 * XB && y < YB && z < ZB ? blocks211[x - XB, y, z] :
                     blocks221!= null &&x >= XB && x < 2 * XB && y > YB && z < ZB ? blocks221[x - XB, y - YB, z] :
                     blocks311!= null &&x >= 2 * XB && x < 3 * XB && y < YB && z < ZB ? blocks311[x - XB - XB, y, z] :
                     blocks321!= null &&x >= 2 * XB && x < 3 * XB && y >= YB && z < ZB ? blocks321[x - XB - XB, y - YB, z] :
                     blocks411!= null &&x >= 3 * XB && y < YB && z < ZB ? blocks411[x - XB - XB - XB, y, z] :
                     blocks421 != null && x >= 3 * XB && y > YB && z < ZB ? blocks421[x - XB - XB - XB, y - YB, z] :

                     blocks112!= null &&x < XB && y < YB ? blocks112[x, y, z - ZB] :
                     blocks122!= null &&x < XB && y >= YB ? blocks122[x, y - YB, z - ZB] :
                     blocks212!= null &&x >= XB && x < 2 * XB && y < YB ? blocks212[x - XB, y, z - ZB] :
                     blocks222!= null &&x >= XB && x < 2 * XB && y >= YB ? blocks222[x - XB, y - YB, z - ZB] :
                     blocks312!= null &&x >= 2 * XB && x < 3 * XB && y < YB && z < ZB ? blocks312[x - XB - XB, y, z - ZB] :
                     blocks322!= null &&x >= 2 * XB && x < 3 * XB && y >= YB && z < ZB ? blocks322[x - XB - XB, y - YB, z - ZB] :
                     blocks412!= null &&x >= 3 * XB && y < YB && z < ZB ? blocks412[x - XB - XB - XB, y, z - ZB] :
                     blocks422 != null ? blocks422[x - XB - XB - XB, y - YB, z - ZB] : null;
            }

            public void Set(int x, int y, int z, ContinousCubeGroup g)
            {
                if (x < 2 * XB)
                {
                    if (z < ZB)
                    {
                        if (blocks111 != null && x < XB && y < YB) { blocks111[x, y, z] = g; }
                        else if (blocks121 != null && x < XB && y >= YB) { blocks121[x, y - YB, z] = g; }
                        else if (blocks211 != null && x >= XB && x < 2 * XB && y < YB) { blocks211[x - XB, y, z] = g; }
                        else
                        {
                            if (blocks221 != null) blocks221[x - XB, y - YB, z] = g;
                        }
                    }
                    else if (blocks112 != null && x < XB && y < YB) { blocks112[x, y, z - ZB] = g; }
                    else if (blocks122 != null && x < XB && y >= YB) { blocks122[x, y - YB, z - ZB] = g; }
                    else if (blocks212 != null && x >= XB && x < 2 * XB && y < YB) { blocks212[x - XB, y, z - ZB] = g; }
                    else
                    {
                        if(blocks222 != null) blocks222[x - XB, y - YB, z - ZB] = g;
                    }
                }
                else 
                {
                    x = x - XB - XB;
                    if (z < ZB)
                    {
                        if (blocks311 != null && x < XB && y < YB) { blocks311[x, y, z] = g; }
                        else if (blocks321!= null &&x < XB && y >= YB) { blocks321[x, y - YB, z] = g; }
                        else if (blocks411 != null && x >= XB && x < 2 * XB && y < YB) { blocks411[x - XB, y, z] = g; }
                        else
                        {
                            if(blocks421 != null) blocks421[x - XB, y - YB, z] = g;
                        }
                    }
                    else if (blocks312 != null && x < XB && y < YB) { blocks312[x, y, z - ZB] = g; }
                    else if (blocks322 != null && x < XB && y >= YB) { blocks322[x, y - YB, z - ZB] = g; }
                    else if (blocks412 != null && x >= XB && x < 2 * XB && y < YB) { blocks412[x - XB, y, z - ZB] = g; }
                    else
                    {
                        if (blocks422 != null) blocks422[x - XB, y - YB, z - ZB] = g;
                    }
                }
            }
        }

        static long Second(string inputFile)
        {
            var rebootsteps = new Parser().ReadData(inputFile).ToList();
            var xborders = rebootsteps.SelectMany(r => r.X).OrderBy(i => i).Distinct().SelectMany(i => System.Linq.Enumerable.Range(i - 1, 3)).Distinct().OrderBy(i => i).ToList();
            var yborders = rebootsteps.SelectMany(r => r.Y).OrderBy(i => i).Distinct().SelectMany(i => System.Linq.Enumerable.Range(i - 1, 3)).Distinct().OrderBy(i => i).ToList();
            var zborders = rebootsteps.SelectMany(r => r.Z).OrderBy(i => i).Distinct().SelectMany(i => System.Linq.Enumerable.Range(i - 1, 3)).Distinct().OrderBy(i => i).ToList();
            var xsegments = xborders.Count();
            var ysegments = yborders.Count();
            var zsegments = zborders.Count();
            long sum = 0;
            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine("Calculating segment " + i);
                BlockCollection blocks = GetBlocks(xborders, yborders, zborders, xsegments, ysegments, zsegments, i);
                SwitchBlocks(rebootsteps, xborders, yborders, zborders, blocks);
                sum += CountBlocks(xsegments, ysegments, zsegments, blocks);
            }
            return sum;
        }

        private static void SwitchBlocks(List<RebootStep> rebootsteps, List<int> xborders, List<int> yborders, List<int> zborders, BlockCollection blocks)
        {
            foreach (var rebootstep in rebootsteps)
            {
                var xMinIndex = xborders.IndexOf(rebootstep.Xmin);
                var xMaxIndex = xborders.IndexOf(rebootstep.Xmax);
                var yMinIndex = yborders.IndexOf(rebootstep.Ymin);
                var yMaxIndex = yborders.IndexOf(rebootstep.Ymax);
                var zMinIndex = zborders.IndexOf(rebootstep.Zmin);
                var zMaxIndex = zborders.IndexOf(rebootstep.Zmax);

                for (int i = xMinIndex; i <= xMaxIndex; i++)
                {
                    for (int j = yMinIndex; j <= yMaxIndex; j++)
                    {
                        for (int k = zMinIndex; k <= zMaxIndex; k++)
                        {
                            ContinousCubeGroup g = blocks.Item(i, j, k);
                            if (g!=null) g.On = rebootstep.InstructionOn;
                        }
                    }
                }
            }
        }

        private static long CountBlocks(int xsegments, int ysegments, int zsegments, BlockCollection blocks)
        {
            var sum = 0L;
            for (int i = 0; i < xsegments - 1; i++)
            {
                for (int j = 0; j < ysegments - 1; j++)
                {
                    for (int k = 0; k < zsegments - 1; k++)
                    {
                        var g = blocks.Item(i, j, k);
                        if (g!=null && g.On) { sum += g.Count; }
                    }
                }
            }

            return sum;
        }

        private static BlockCollection GetBlocks(List<int> xborders, List<int> yborders, List<int> zborders, int xsegments, int ysegments, int zsegments, int segment)
        {
            var blocks = new BlockCollection(xsegments - 1, ysegments - 1, zsegments - 1, segment);
            var minX = xborders.First();
            var minY = yborders.First();
            var minZ = zborders.First();
            var minXY = minY;
            var minYZ = minZ;
            for (int i = 1; i < xsegments; i++)
            {
                var maxX = xborders.ElementAt(i) - 1;

                for (int j = 1; j < ysegments; j++)
                {
                    var maxY = yborders.ElementAt(j) - 1;
                    for (int k = 1; k < zsegments; k++)
                    {
                        var maxZ = zborders.ElementAt(k) - 1;
                        ContinousCubeGroup g = new ContinousCubeGroup(minX, maxX, minY, maxY, minZ, maxZ, false);
                        blocks.Set(i - 1, j - 1, k - 1, g);
                        minZ = maxZ + 1;
                    }
                    minZ = minYZ;
                    minY = maxY + 1;
                }
                minY = minXY;
                minX = maxX + 1;
            }

            return blocks;
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

    public class ContinousCubeGroup
    {
        public int Xmin { get; set; }
        public int Ymin { get; set; }
        public int Zmin { get; set; }

        public int Xmax { get; set; }
        public int Ymax { get; set; }
        public int Zmax { get; set; }

        public bool On { get; set; }

        public long Count => ((long)(Xmax - Xmin + 1)) * ((long)(Ymax - Ymin + 1)) * ((long)(Zmax - Zmin + 1));

        public ContinousCubeGroup(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, bool status)
        {
            Xmin = xmin;
            Xmax = xmax;
            Ymin = ymin;
            Ymax = ymax;
            Zmin = zmin;
            Zmax = zmax;
            On = status;
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
