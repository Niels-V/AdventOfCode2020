using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    public class Range{
        public int Min { get; set; }
        public int Max { get; set; }
    }
    public class RangePair {
        public Range Item1 { get; set; }
        public Range Item2 { get; set; }

        public bool FullOverlap { 
            get 
            {
                return (Item1.Min <= Item2.Min && Item1.Max >= Item2.Max) || 
                       (Item2.Min <= Item1.Min && Item2.Max >= Item1.Max) ;
            }
        }

        public bool PartialOverlap { 
            get 
            {
                return !((Item1.Max < Item2.Min && Item1.Max < Item2.Max) || 
                       (Item2.Max < Item1.Min && Item2.Max<Item1.Max)) ;
            }
        }
    }
    public class RangePairParser : Common.Parser<RangePair> {
        protected override RangePair ParseLine(string line) {
            var pairs = line.Split(new char[] {',','-'});
            return new RangePair{
                Item1 = new Range{ Min = Convert.ToInt32(pairs[0]),Max=Convert.ToInt32(pairs[1]) },
                Item2 = new Range{ Min = Convert.ToInt32(pairs[2]),Max=Convert.ToInt32(pairs[3]) }
            };
        }
    }
    [TestCategory("2022")]
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
            var parser = new RangePairParser();
            var data = parser.ReadData(inputFile);
            return data.Count(d=>d.FullOverlap);
        }

        static long Second(string inputFile)
        {
            var parser = new RangePairParser();
            var data = parser.ReadData(inputFile);
            return data.Count(d=>d.PartialOverlap);
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(4, result);
        }
    }
}
