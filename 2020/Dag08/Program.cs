using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dag08
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var accAtLoop = FindAccumulatorAtLoopOrExit("input.txt");

            Console.WriteLine("Accumulator at loop found: {0}", accAtLoop.Item1);
            Console.WriteLine("Start tweaking program...");

            var result = FindNiceRun("input.txt");

            Console.WriteLine("Tweaked program succes, found accumulator: {0}", result.Item1);
        }

        public static Tuple<int, int> FindAccumulatorAtLoopOrExit(string inputFile)
        {
            var program = Parser.ReadData(inputFile).ToList();
            return RunProgram(program);
        }

        public static Tuple<int, int> FindNiceRun(string inputFile)
        {
            var program = Parser.ReadData(inputFile).ToList();

            for (int i = 0; i < program.Count; i++)
            {
                if (program[i].Operation == OperationType.Accumulator)
                {
                    continue;
                }
                var moddifiedProgram = Parser.ReadData(inputFile).ToList();
                moddifiedProgram[i].Operation = program[i].Operation == OperationType.NoOperation ? OperationType.Jumps : OperationType.NoOperation;

                var result = RunProgram(moddifiedProgram);
                //Item2 contains the instruction pointer at exit. When it points to the length
                //of the program, it should have exited normally (unless someone jumped there)
                if (result.Item2 == program.Count)
                {
                    return result;
                }
            }
            return null;
        }

        private static Tuple<int, int> RunProgram(List<Instruction> program)
        {
            var visitedNodes = new List<int>();
            var accumulator = 0;
            var currentNode = 0;
            while (!visitedNodes.Contains(currentNode) && currentNode < program.Count && currentNode >= 0)
            {
                var instruction = program[currentNode];
                var nodeStep = 1;
                switch (instruction.Operation)
                {
                    case OperationType.NoOperation:
                        break;
                    case OperationType.Accumulator:
                        accumulator += instruction.Argument;
                        break;
                    case OperationType.Jumps:
                        nodeStep = instruction.Argument;
                        break;
                }
                visitedNodes.Add(currentNode);
                currentNode += nodeStep;
            }
            return new Tuple<int, int>(accumulator, currentNode);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var accAtLoop = FindAccumulatorAtLoopOrExit("test.txt");
            Assert.AreEqual(5, accAtLoop.Item1);
            Assert.AreNotEqual(9, accAtLoop.Item2);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var result = FindNiceRun("test.txt");
            Assert.AreEqual(8, result.Item1);
            Assert.AreEqual(9, result.Item2);
        }
    }
}
