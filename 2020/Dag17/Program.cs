using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag17
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
            var steps = 6;
            var cubeMap = CubeMapParser.Instance.ReadMap(inputFile);

            SeatState simNextState;
            do
            {
                simNextState = new SeatState(currentState);
                var nextState = simNextState.CalculateNewState();
                currentState = nextState;
                Console.WriteLine("Calculated new seatstate");
            } while (simNextState.NextStateChanged);
            return simNextState;
        }

        static int Second(string inputFile)
        {
            return -2;
        }


        [DataTestMethod]
        [DataRow("test.txt", 0)]
        public void TestPart1(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", 0)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
