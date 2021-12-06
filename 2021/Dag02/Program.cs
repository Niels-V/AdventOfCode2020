using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Dag02
{
    public enum OperationType
    {
        [EnumMember(Value = "forward")]
        Forward,
        [EnumMember(Value = "down")]
        Down,
        [EnumMember(Value = "up")]
        Up
    }

    public struct Position
    {
        public int X;
        public int Depth;
        public int Aim;

        public Position(int x, int depth, int aim)
        {
            X = x; Depth = depth; Aim = aim;
        }
    }

    public class Instruction
    {
        public OperationType Operation { get; set; }
        public int Argument { get; set; }
    }

    [TestCategory("2021")]
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
            var subRoute = new Parser().ReadData(inputFile);
            Position position = new();
            return subRoute.Aggregate(position, (oldPosition, instruction) =>
            {
                return instruction.Operation switch
                {
                    OperationType.Forward => new(oldPosition.X + instruction.Argument, oldPosition.Depth, oldPosition.Aim),
                    OperationType.Down => new(oldPosition.X, oldPosition.Depth + instruction.Argument, oldPosition.Aim),
                    OperationType.Up => new(oldPosition.X, oldPosition.Depth - instruction.Argument, oldPosition.Aim),
                    _ => throw new InvalidOperationException(),
                };
            }, position => position.X * position.Depth);
        }

        static int Second(string inputFile)
        {
            var subRoute = new Parser().ReadData(inputFile);

            Position position = new();
            return subRoute.Aggregate(position, (oldPosition, instruction) =>
            {
                return instruction.Operation switch
                {
                    OperationType.Forward => new(oldPosition.X + instruction.Argument, oldPosition.Depth + oldPosition.Aim * instruction.Argument, oldPosition.Aim),
                    OperationType.Down => new(oldPosition.X, oldPosition.Depth, oldPosition.Aim + instruction.Argument),
                    OperationType.Up => new(oldPosition.X, oldPosition.Depth, oldPosition.Aim - instruction.Argument),
                    _ => throw new InvalidOperationException(),
                };
            }, position => position.X * position.Depth);
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(150, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(900, result);
        }
    }
}
