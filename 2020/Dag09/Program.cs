using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag09
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = FindFirstMismatch("input.txt", 25);
            Console.WriteLine("Program succes, found result: {0}", result);
            var encryptionWeakness = FindEncryptionWeakness("input.txt", result);
            Console.WriteLine("Program succes, found weakness: {0}", encryptionWeakness);

        }
        public static long FindFirstMismatch(string inputFile, int preambleLength)
        {
            var parser = new LongParser();
            var numbers = parser.ReadData(inputFile).ToList();
            var result = numbers.Select((x, i) => new Tuple<long, bool>(x, IsSumInList(FindSubList(numbers, i, preambleLength), x)));
            return result.First(x => !x.Item2).Item1;
        }

        public static IEnumerable<long> FindSubList(IEnumerable<long> list, int index, int preambleLength)
        {
            if (index < preambleLength) return Enumerable.Empty<long>();
            return list.Skip(index - preambleLength).Take(preambleLength);
        }

        public static bool IsSumInList(IEnumerable<long> list, long expectedSum)
        {
            if (list.Count() == 0) return true;
            return Pairs(list).Select(x => x.Item1 + x.Item2).Any(x => x == expectedSum);
        }

        public static long FindEncryptionWeakness(string inputFile, long expectedSum)
        {
            var parser = new LongParser();
            var list = parser.ReadData(inputFile).ToList();
            return SumMinMax(FindContiniousSum(list, expectedSum));
        }

        public static IEnumerable<long> FindContiniousSum(IEnumerable<long> list, long expectedSum)
        {
            for (int i = 0; i < list.Count()-1; i++) //all elements except last
            {
                for (int j = 2; j <= list.Count()-i; j++) //
                {
                    var foundSum = list.Skip(i).Take(j).Sum();
                    if (foundSum > expectedSum) { break; }
                    if (foundSum == expectedSum)
                    {
                        return list.Skip(i).Take(j).ToList();
                    }
                }
            }
            return Enumerable.Empty<long>();
        }

        public static long SumMinMax(IEnumerable<long> list)
        {
            return list.Max() + list.Min();
        }

        static IEnumerable<Tuple<long, long>> Pairs(IEnumerable<long> list)
        {
            {
                //Assume [A,B,C,D,E]
                if (!list.Any())
                {
                    //return empty Tuple
                    return Enumerable.Empty<Tuple<long, long>>();
                }
                var head = list.First(); // A
                var tail = list.Skip(1); // [B,C,D,E]
                return tail.Select(t => new Tuple<long, long>(head, t)) //Return all the possbile combinations you can make with head and tail elements
                                                                      // (A,B),(A,C),(A,D),(A,E)
                    .Concat(Pairs(tail)); //Return all pair combinations possible from the tail, is Pair([B,C,D,E])
            }
        }

        [DataTestMethod]
        [DataRow(new long[] { 35, 20, 15, 25, 47, 40, 62, 55}, 127, new long[] { 15, 25, 47, 40 })]
        public void TestFindContiniousSum(IEnumerable<long> list, int sum, IEnumerable<long> expectedList)
        {
            var result = FindContiniousSum(list, sum);
            CollectionAssert.AreEqual(expectedList.ToList(), result.ToList());
        }

        [DataTestMethod]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 55, true)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 50, true)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 35, true)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 60, true)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 40, true)]
        [DataRow(new long[] { 20, 15, 25, 47, 40 }, 62, true)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 67, true)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 62, true)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 63, false)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 61, false)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 5, false)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 15, false)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 1, false)]
        [DataRow(new long[] { 35, 20, 15, 25, 47 }, 20, false)]
        [DataRow(new long[] { }, 20, true)]
        public void TestIsSumInList(IEnumerable<long> list, int sum, bool expectedResult)
        {
            var result = IsSumInList(list, sum);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 0, 3, new long[] { })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 1, 3, new long[] { })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 2, 3, new long[] { })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 3, 3, new long[] { 1, 2, 3 })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 4, 3, new long[] { 2, 3, 4 })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5, 3, new long[] { 3, 4, 5 })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 6, 3, new long[] { 4, 5, 6 })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 7, 3, new long[] { 5, 6, 7 })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 8, 3, new long[] { 6, 7, 8 })]
        [DataRow(new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 9, 3, new long[] { 7, 8, 9 })]
        public void TestFindSubList(IEnumerable<long> list, int index, int preambleLength, IEnumerable<long> expectedList)
        {
            var result = FindSubList(list, index, preambleLength);
            CollectionAssert.AreEqual(expectedList.ToList(), result.ToList());
        }
        [TestMethod]
        public void TestProgram()
        {
            var result = FindFirstMismatch("test.txt", 5);
            Assert.AreEqual(127, result);
        }

        [TestMethod]
        public void TestFindEncryptionWeakness()
        {
            var result = FindEncryptionWeakness("test.txt", 127);
            Assert.AreEqual(62, result);
        }
    }
}
