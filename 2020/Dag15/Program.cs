using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag15
{
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

        static int First(string inputFile)
        {
            var parser = new IntParser();
            var inputs = parser.ReadData(inputFile).ToList();
            return StartCallingNumbers(inputs, 2020);
        }

        private static int StartCallingNumbers(List<int> inputs, int numberOfCalls)
        {
            var calledLast = new Dictionary<int, int>(); //contains each number which is called (in te key) and in which last turn (in the value)
            var calledNumber = inputs[0];
            //var calledBeforeLast = new Dictionary<int, int>(); //contains each number which is called (in te key) and in which one-before-last turn (in the value)
            int oldCalledNumber;
            for (int i = 1; i < inputs.Count; i++)
            {
                oldCalledNumber = calledNumber;
                calledNumber = inputs[i];
                if (calledLast.ContainsKey(oldCalledNumber))
                {
                    calledLast[oldCalledNumber] = i;
                }
                else
                {
                    calledLast.Add(oldCalledNumber, i);
                }
            }
            for (int turn = inputs.Count+1; turn <= numberOfCalls; turn++)
            {
                oldCalledNumber = calledNumber;
                if (calledLast.ContainsKey(oldCalledNumber))
                {
                    calledNumber = turn - 1 - calledLast[oldCalledNumber];
                    calledLast[oldCalledNumber] = turn-1;
                }
                else
                {
                    calledNumber = 0;
                    calledLast.Add(oldCalledNumber, turn-1);
                }
            }
            return calledNumber;
        }

        private static int CallNumber(List<int> called)
        {
            var lastCalled = called.Last();
            for (int k = called.Count - 2; k >= 0; k--)
            {
                if (called[k] == lastCalled)
                {
                    return called.Count - k - 1;
                }
            }
            return 0;
        }

        static int Second(string inputFile)
        {
            var parser = new IntParser();
            var inputs = parser.ReadData(inputFile).ToList();
            return StartCallingNumbers(inputs, 30000000);
        }

        [DataTestMethod]
        [DataRow(new[] { 0, 3, 6 }, 0)]
        [DataRow(new[] { 0, 3, 6, 0 }, 3)]
        [DataRow(new[] { 0, 3, 6, 0, 3 }, 3)]
        [DataRow(new[] { 0, 3, 6, 0, 3, 3 }, 1)]
        [DataRow(new[] { 0, 3, 6, 0, 3, 3, 1 }, 0)]
        [DataRow(new[] { 0, 3, 6, 0, 3, 3, 1, 0 }, 4)]
        [DataRow(new[] { 0, 3, 6, 0, 3, 3, 1, 0, 4 }, 0)]
        public void TestCallNumber(int[] input, int expectedResult)
        {
            var result = CallNumber(input.ToList());
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow(new[] { 0, 3, 6 }, 10, 0)]
        [DataRow(new[] { 2, 1, 3 }, 2020, 10)]
        [DataRow(new[] { 1, 3, 2 }, 2020, 1)]
        [DataRow(new[] { 1, 2, 3 }, 2020, 27)]
        [DataRow(new[] { 2, 3, 1 }, 2020, 78)]
        [DataRow(new[] { 3, 2, 1 }, 2020, 438)]
        [DataRow(new[] { 3, 1, 2 }, 2020, 1836)]
        [DataRow(new[] { 0, 3, 6 }, 30000000, 175594)]
        [DataRow(new[] { 1, 3, 2 }, 30000000, 2578)]
        [DataRow(new[] { 2, 1, 3 }, 30000000, 3544142)]
        [DataRow(new[] { 1, 2, 3 }, 30000000, 261214)]
        [DataRow(new[] { 2, 3, 1 }, 30000000, 6895259)]
        [DataRow(new[] { 3, 2, 1 }, 30000000, 18)]
        [DataRow(new[] { 3, 1, 2 }, 30000000, 362)]
        public void TestCallingNumbers(int[] input, int times, int expectedResult)
        {
            var result = StartCallingNumbers(input.ToList(), times);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 436)]
        public void TestPart1(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", 175594)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
