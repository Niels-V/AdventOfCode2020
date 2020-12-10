using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag10
{
    public static class Functions
    {
        public static int MagicCalculation(this Dictionary<long, int> counts)
        {
            if (counts.Count() != 2) { throw new ArgumentException("Counts should only contains elements 1 and 3", nameof(counts)); }
            return counts[1] * counts[3];
        }
        public static Dictionary<T, int> Counts<T>(this IEnumerable<T> list)
        {
            var distinctElements = list.Distinct();
            return distinctElements.Select(e => new { Item = e, Count = list.Count(i => i.Equals(e)) })
                .ToDictionary(d => d.Item, d => d.Count);
        }

        public static IEnumerable<long> Differences(this IList<long> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                yield return list[i + 1] - list[i];
            }
            yield break;
        }

        public static IList<long> JoltageCollection(this IEnumerable<long> list)
        {
            return list.OrderBy(l => l).Prepend(0).Append(list.Max() + 3).ToList();
        }

        public static Dictionary<uint, uint> FindSegmentsWithLength(this IEnumerable<long> differenceList)
        {
            var dict = new Dictionary<uint, uint>();
            uint currentLength = 0;
            for (uint i = 0; i < differenceList.Count(); i++)
            {
                if (differenceList.ElementAt((int)i) == 1)
                {
                    currentLength++;
                }
                else
                {
                    if (currentLength > 1)
                    {
                        if (dict.ContainsKey(currentLength))
                        {
                            dict[currentLength] += 1;
                        }
                        else
                        {
                            dict.Add(currentLength, 1);
                        }
                    }
                    currentLength = 0;
                }
            }
            return dict;
        }
    }

    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = MagicCalculation("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = DifferenceCalculation("input.txt");
            Console.WriteLine("Program succes, found number of items: {0}", result2);

        }


        static int MagicCalculation(IEnumerable<long> unorderedList)
        {
            return unorderedList.JoltageCollection().Differences().Counts().MagicCalculation();
        }

        static long LongPow(long x, uint pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        static long DifferenceCalculation(IEnumerable<long> unorderedList)
        {
            var differences = unorderedList.JoltageCollection().Differences().ToList();
            var segments = differences.FindSegmentsWithLength();
            long result = 1;
            for (int i = 0; i < segments.Count; i++)
            {
                uint segmentLength = segments.ElementAt(i).Key;
                uint numberOfSegements = segments.ElementAt(i).Value;
                result *= LongPow(FindNumberOfCombinations(segmentLength), numberOfSegements);
            }
            return result;
        }

        private static long FindNumberOfCombinations(uint segmentLength)
        {
            switch (segmentLength)
            {
                case 0: return 0;
                case 1: return 1;
                case 2: return 2;
                case 3: return 4;
                case 4: return 7;
                default:
                    throw new ArgumentOutOfRangeException(nameof(segmentLength));
            }
        }

        static int MagicCalculation(string inputFile)
        {
            var parser = new LongParser();
            var numbers = parser.ReadData(inputFile).ToList();
            return MagicCalculation(numbers);
        }

        static long DifferenceCalculation(string inputFile)
        {
            var parser = new LongParser();
            var numbers = parser.ReadData(inputFile).ToList();
            return DifferenceCalculation(numbers);
        }

        [DataTestMethod]
        [DataRow(new long[] { 16, 10, 15, 5, 1, 11, 7, 19, 6, 12, 4 }, 35)]
        public void TestMagicCalculation(IEnumerable<long> list, int expectedResult)
        {
            var result = MagicCalculation(list);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow(new long[] { 16, 10, 15, 5, 1, 11, 7, 19, 6, 12, 4 }, 8)]
        public void TestDifferenceCalculation(IEnumerable<long> list, int expectedResult)
        {
            var result = DifferenceCalculation(list);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 220)]
        public void TestMagicCalculationFile(string inputFile, int expectedResult)
        {
            var result = MagicCalculation(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 19208)]
        public void TestDifferenceCalculationFile(string inputFile, int expectedResult)
        {
            var result = DifferenceCalculation(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
