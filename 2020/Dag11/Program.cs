using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag11
{
    public static class Functions
    {

        public static SeatState SimulateAdjacentUntilStable(string inputFile)
        {
            var currentState = SeatMapParser.Instance.ReadMap(inputFile);
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
        public static LineOfSightSeatState SimulateLosUntilStable(string inputFile)
        {
            var currentState = SeatMapParser.Instance.ReadMap(inputFile);
            LineOfSightSeatState simNextState;
            do
            {
                simNextState = new LineOfSightSeatState(currentState);
                var nextState = simNextState.CalculateNewState();
                currentState = nextState;
                Console.WriteLine("Calculated new seatstate");
            } while (simNextState.NextStateChanged);
            return simNextState;
        }
    }

    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = FindStabilizedSeatsOccupied("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = FindStabilizedLosSeatsOccupied("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static int FindStabilizedLosSeatsOccupied(string inputFile)
        {
            return Functions.SimulateLosUntilStable(inputFile).NextStateSeatOccupied;
        }

        static int FindStabilizedSeatsOccupied(string inputFile)
        {
            return Functions.SimulateAdjacentUntilStable(inputFile).NextStateSeatOccupied;
        }

        [DataTestMethod]
        [DataRow("test.txt", 26)]
        public void TestFindStabilizedLosSeatsOccupied(string inputFile, int expectedResult)
        {
            var result = FindStabilizedLosSeatsOccupied(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 37)]
        public void TestFindStabilizedSeatsOccupied(string inputFile, int expectedResult)
        {
            var result = FindStabilizedSeatsOccupied(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
