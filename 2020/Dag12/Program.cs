using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag12
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = RunBoat("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = RunBoatWithWaypoint("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static int RunBoat(string inputFile)
        {
            var parser = new BoatInstructionParser();
            var instructions = parser.ReadData(inputFile);

            var boat = new Boat();
            foreach (var instruction in instructions)
            {
                switch (instruction.Operation)
                {
                    case OperationType.Forward:
                        boat.Move(instruction.Argument);
                        break;
                    case OperationType.Nord:
                        boat.Translate(new Position(0, -instruction.Argument));
                        break;
                    case OperationType.South:
                        boat.Translate(new Position(0, instruction.Argument));
                        break;
                    case OperationType.East:
                        boat.Translate(new Position(instruction.Argument, 0));
                        break;
                    case OperationType.West:
                        boat.Translate(new Position(-instruction.Argument, 0));
                        break;
                    case OperationType.Right:
                        boat.Rotate(instruction.Argument);
                        break;
                    case OperationType.Left:
                        boat.Rotate(-instruction.Argument);
                        break;
                    default:
                        break;
                }
            }

            return boat.ManhattanDistance;
        }

        static int RunBoatWithWaypoint(string inputFile)
        {
            var parser = new BoatInstructionParser();
            var instructions = parser.ReadData(inputFile);

            var boat = new BoathWithWaypoint();
            foreach (var instruction in instructions)
            {
                switch (instruction.Operation)
                {
                    case OperationType.Forward:
                        boat.Move(instruction.Argument);
                        break;
                    case OperationType.Nord:
                        boat.TranslateWaypoint(new Position(0, -instruction.Argument));
                        break;
                    case OperationType.South:
                        boat.TranslateWaypoint(new Position(0, instruction.Argument));
                        break;
                    case OperationType.East:
                        boat.TranslateWaypoint(new Position(instruction.Argument, 0));
                        break;
                    case OperationType.West:
                        boat.TranslateWaypoint(new Position(-instruction.Argument, 0));
                        break;
                    case OperationType.Right:
                        boat.RotateWaypoint(instruction.Argument);
                        break;
                    case OperationType.Left:
                        boat.RotateWaypoint(-instruction.Argument);
                        break;
                    default:
                        break;
                }
            }

            return boat.ManhattanDistance;
        }


        [DataTestMethod]
        [DataRow("test.txt", 25)]
        public void TestRunBoat(string inputFile, int expectedResult)
        {
            var result = RunBoat(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", 286)]
        public void TestRunBoatWaypoint(string inputFile, int expectedResult)
        {
            var result = RunBoatWithWaypoint(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
