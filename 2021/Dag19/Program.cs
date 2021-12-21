using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC
{
    [TestCategory("2021")]
    [TestClass]
    public class Program
    {
        public static List<Matrix4x4> TransformationMatrices { get; set; }
        [TestInitialize]
        public void TestInit()
        {
            InitTransformationMatrices();
        }
        static void Main(string[] args)
        {
            InitTransformationMatrices();
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        private static void InitTransformationMatrices()
        {
            //predetermined tha 24 different ration matrices. Could have calculated them but this was easier.
            TransformationMatrices = new List<Matrix4x4>
            {
                //            11 12 13    21 22 23    31 32 33
                new Matrix4x4( 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0,0,0,0,1),
                new Matrix4x4( 1, 0, 0, 0, 0,-1, 0, 0, 0, 0,-1, 0,0,0,0,1),
                new Matrix4x4( 1, 0, 0, 0, 0, 0, 1, 0, 0,-1, 0, 0,0,0,0,1),
                new Matrix4x4( 1, 0, 0, 0, 0, 0,-1, 0, 0, 1, 0, 0,0,0,0,1),
                new Matrix4x4(-1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0,0,0,0,1),
                new Matrix4x4(-1, 0, 0, 0, 0, 0,-1, 0, 0,-1, 0, 0,0,0,0,1),
                new Matrix4x4(-1, 0, 0, 0, 0, 1, 0, 0, 0, 0,-1, 0,0,0,0,1),
                new Matrix4x4(-1, 0, 0, 0, 0,-1, 0, 0, 0, 0, 1, 0,0,0,0,1),
                new Matrix4x4( 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 1, 0, 0, 0, 0,-1, 0,-1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 1, 0, 0, 1, 0, 0, 0, 0, 0,-1, 0,0,0,0,1),
                new Matrix4x4( 0, 1, 0, 0,-1, 0, 0, 0, 0, 0, 1, 0,0,0,0,1),
                new Matrix4x4( 0,-1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0,0,0,0,1),
                new Matrix4x4( 0,-1, 0, 0,-1, 0, 0, 0, 0, 0,-1, 0,0,0,0,1),
                new Matrix4x4( 0,-1, 0, 0, 0, 0, 1, 0,-1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0,-1, 0, 0, 0, 0,-1, 0, 1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0, 1, 0,-1, 0, 0, 0, 0,-1, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0, 1, 0, 0, 1, 0, 0,-1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0, 1, 0, 0,-1, 0, 0, 1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0,-1, 0, 0, 1, 0, 0, 1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0,-1, 0, 0,-1, 0, 0,-1, 0, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0,-1, 0, 1, 0, 0, 0, 0,-1, 0, 0,0,0,0,1),
                new Matrix4x4( 0, 0,-1, 0,-1, 0, 0, 0, 0, 1, 0, 0,0,0,0,1)
            };
        }

        static long First(string inputFile)
        {
            var scanners = new ScannerParser().ParseFile(inputFile);
            CalculateScannerPositions(scanners);
            var beacons = new HashSet<Vector3>();
            foreach (var scanner in scanners)
            {
                var points = scanner.Value.Points.Select(point => Vector3.Transform(point, scanner.Value.TransformationFromOrigin)); //get point in reference frame from scanner 0
                points = points.Select(point => Vector3.Add(point, scanner.Value.OriginOffset)); //add the offset of the local origin from the origin from scanner 0
                foreach (var point in points)
                {
                    if (!beacons.Contains(point)) { beacons.Add(point); }
                }
            }
            return beacons.Count;
        }
        static float Second(string inputFile)
        {
            var scanners = new ScannerParser().ParseFile(inputFile);
            CalculateScannerPositions(scanners);
            var maxDistance = 0F;
            for (int i = 0; i < scanners.Count; i++)
            {
                for (int j = i + 1; j < scanners.Count; j++)
                {
                    var d = (scanners[i].OriginOffset - scanners[j].OriginOffset);
                    var distance = Math.Abs(d.X) + Math.Abs(d.Y) + Math.Abs(d.Z);
                    maxDistance = distance > maxDistance ? distance : maxDistance;
                }
            }
            return maxDistance;
        }

        private static void CalculateScannerPositions(Dictionary<int, Scanner> scanners)
        {
            var linkedScanners = new List<ScannerCombination>();
            var n = scanners.Count;
            //calculate intrascanner distances between all points
            for (int i = 0; i < n; i++)
            {
                scanners[i].CalculateDistances();
            }
            //when 12 points match, two different scanners should have 66 equal distances.
            //Now create combinations of scanners who have at least those 66 equal distances
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    var overlappingDistances = scanners[i].Distances.Keys.Intersect(scanners[j].Distances.Keys);
                    if (overlappingDistances.Count() >= 66)
                    {
                        var probePointsSource = scanners[i].Distances.Where(d => overlappingDistances.Contains(d.Key)).SelectMany(kv => new[] { kv.Value.Item1, kv.Value.Item2 }).Distinct();
                        var probePointsTarget = scanners[j].Distances.Where(d => overlappingDistances.Contains(d.Key)).SelectMany(kv => new[] { kv.Value.Item1, kv.Value.Item2 }).Distinct();
                        linkedScanners.Add(new ScannerCombination
                        {
                            Source = scanners[i],
                            Target = scanners[j],
                            ProbePointsSource = probePointsSource.ToList(),
                            ProbePointsTarget = probePointsTarget.ToList()
                        });
                    }
                }
            }
            foreach (var overlap in linkedScanners)
            {
                //Just try each transformation. When the orientation is right, we should at least find 12 counts of the same translation vector
                //this vector is the transformation between source an target
                for (int transformationIndex = 0; transformationIndex < 24; transformationIndex++)
                {
                    var transformation = TransformationMatrices[transformationIndex];
                    var origingCounts = new Dictionary<Vector3, int>();
                    foreach (var pointS in overlap.ProbePointsSource)
                    {
                        foreach (var pointT in overlap.ProbePointsTarget)
                        {
                            var origintT = Vector3.Subtract(pointS, Vector3.Transform(pointT, transformation));
                            if (origingCounts.ContainsKey(origintT)) { origingCounts[origintT]++; }
                            else
                            {
                                origingCounts.Add(origintT, 1);
                            }
                        }
                    }
                    if (origingCounts.Values.Any(v => v >= 12))
                    {
                        var translation = origingCounts.Where(o => o.Value == origingCounts.Values.Max()).Select(o => o.Key).First();
                        overlap.TransformationNeeded = transformation;
                        overlap.TranslationNeeded = translation;
                        break;
                    }
                }
            }
            Console.WriteLine("Current status:");
            foreach (var item in linkedScanners)
            {
                Console.WriteLine($"Overlap {item.Source.Id}->{item.Target.Id} with translation {item.TranslationNeeded}");
            }
            //Now do the rotation and transformation algebra to get each transformation and rotation for each scanner in the reference frame of scanner 0
            scanners[0].OriginCalculationComplete = true;
            scanners[0].TransformationFromOrigin = TransformationMatrices[0];
            while (!scanners.Values.All(s => s.OriginCalculationComplete))
            {
                foreach (var scanner in scanners.Values.Where(s => !s.OriginCalculationComplete).ToList())
                {
                    if (scanner.OriginCalculationComplete) { continue; }
                    //try to find a transition to this scanner from a source
                    var scannerTransition = linkedScanners.Where(r => r.Target == scanner && r.Source.OriginCalculationComplete).FirstOrDefault();
                    if (scannerTransition == null)
                    {
                        //No scanner found from a source, try the other way around, from a target
                        scannerTransition = linkedScanners.Where(r => r.Source == scanner && r.Target.OriginCalculationComplete).FirstOrDefault();
                        if (scannerTransition == null)
                        {
                            //none found (yet), try latter in a next foreach loop.
                            continue;
                        }
                        //Here we try from target, so we need to invert the matrix (rotate in the counter direction).
                        Matrix4x4.Invert(scannerTransition.TransformationNeeded, out Matrix4x4 invert);
                        scanner.TransformationFromOrigin = Matrix4x4.Multiply(invert, scannerTransition.Target.TransformationFromOrigin);
                        //As Tot = Tos + Mos*Tst <-> Tos = Tot - Mos*Tst
                        scanner.OriginOffset = Vector3.Subtract(scannerTransition.Target.OriginOffset, Vector3.Transform(scannerTransition.TranslationNeeded, scanner.TransformationFromOrigin));
                        scanner.OriginCalculationComplete = true;
                        continue;
                    }
                    scanner.OriginOffset = Vector3.Add(scannerTransition.Source.OriginOffset, Vector3.Transform(scannerTransition.TranslationNeeded, scannerTransition.Source.TransformationFromOrigin));
                    scanner.TransformationFromOrigin = Matrix4x4.Multiply(scannerTransition.TransformationNeeded, scannerTransition.Source.TransformationFromOrigin);
                    scanner.OriginCalculationComplete = true;
                }
            }
            foreach (var item in scanners.Values)
            {
                Console.WriteLine($"Origin transform {item.Id}-> translation {item.OriginOffset}");
            }
        }
        
        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(79, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(3621, result);
        }
    }
    public class ScannerCombination
    {
        public Scanner Source { get; set; }
        public Scanner  Target { get; set; }
        public List<Vector3> ProbePointsSource { get; internal set; }
        public List<Vector3> ProbePointsTarget { get; internal set; }
        public Matrix4x4 TransformationNeeded { get; internal set; }
        public Vector3 TranslationNeeded { get; internal set; }
    }

    public class Scanner
    {
        public List<Vector3> Points { get; internal set; } = new List<Vector3>();
        public int Id { get; internal set; }

        public bool OriginCalculationComplete { get; set; }
        public Matrix4x4 TransformationFromOrigin { get; set; }
        public Vector3 OriginOffset { get; set; }

        public Dictionary<float,(Vector3,Vector3)> Distances { get; set; }

        public void CalculateDistances()
        {
            Distances = new Dictionary<float, (Vector3, Vector3)>();
            for (int i = 0; i < Points.Count; i++)
            {
                for (int j = i+1; j < Points.Count; j++)
                {
                    var distance = Vector3.Distance(Points[i], Points[j]);
                    if (Distances.ContainsKey(distance))
                    {
                        throw new InvalidOperationException("Duplicate distance found");
                    }
                    else
                    {
                        Distances.Add(distance, (Points[i], Points[j]));
                    }
                }
            }
        }
    }

    public class ScannerParser : LineParser
    {
        public Dictionary<int,Scanner> ParseFile(string filePath)
        {
            var scanners = new Dictionary<int,Scanner>();
            Scanner currentScanner = null;
            int scannerNumber = -1;
            foreach (var line in ReadData(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (line.StartsWith("---"))
                {
                    scannerNumber++;
                    currentScanner = new Scanner {
                        Id = scannerNumber
                    };
                    scanners.Add(scannerNumber,currentScanner);
                    continue;
                }
                var elements = line.Split(',').Select(s => Convert.ToSingle(s)).ToArray();
                var vector = new Vector3(elements[0],elements[1], elements[2]);
                currentScanner.Points.Add(vector);
            }
            return scanners;
        }
    }
}