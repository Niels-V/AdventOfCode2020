using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Dag14
{
    public class MemoryProgram
    {
        public Dictionary<ulong, ulong> Memory { get; private set; }
        public MemoryProgram()
        {
            Memory = new Dictionary<ulong, ulong>();
        }

        internal void Write(ulong address, ulong value)
        {
            if (Memory.ContainsKey(address)) { Memory[address] = value; }
            else { Memory.Add(address, value); }
        }

        internal void Write(IEnumerable<ulong> addresses, ulong value)
        {
            foreach (var address in addresses)
            {
                if (Memory.ContainsKey(address)) { Memory[address] = value; }
                else { Memory.Add(address, value); }
            }
        }
    }

    [TestCategory("2020")]
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

        static ulong First(string inputFile)
        {
            var parser = new InstructionParser();
            var instructions = parser.ReadData(inputFile);

            var memoryProgram = new MemoryProgram();
            Instruction currentMask = null;
            foreach (var instruction in instructions)
            {
                if (instruction.Operation == OperationType.Mask)
                {
                    currentMask = instruction;
                    continue;
                }
                var writeValue = ApplyMask1(instruction.Argument2, currentMask.Argument1, currentMask.Argument2);
                memoryProgram.Write(instruction.Argument1, writeValue);
            }
            return memoryProgram.Memory.Values.Aggregate(0UL, (acc, val) => acc += val); ;
        }

        private static ulong ApplyMask1(ulong currentValue, ulong bitsToWrite, ulong bitMask)
        {
            return (currentValue & (bitsToWrite | ~bitMask)) | (bitsToWrite & ~currentValue);
        }

        private static IEnumerable<ulong> ApplyMask2(ulong currentAddress, ulong bitsToWrite, ulong bitMask)
        {
             var mask =  (~bitMask & ~currentAddress & ~bitsToWrite)^ ~bitMask;
            var possibleSubmasks = GetPossibleSubmasks(bitMask).ToList();
            return possibleSubmasks.Select(m=> (m | mask)).ToList();
        }

        private static IEnumerable<ulong> GetPossibleSubmasks(ulong bitMask)
        {
            var x = new BitArray(BitConverter.GetBytes(bitMask));
            List<ulong> current = Enumerable.Empty<ulong>().ToList();
            for (int i = 0; i < 36; i++)
            {
                current = Sub(x, i, current).ToList();
            }
            return current;
        }

        private static IEnumerable<ulong> Sub(BitArray x, int index, List<ulong> current)
        {
            if (x[index])
            {
                if (!current.Any())
                {
                    yield return 0UL;
                    yield return 1UL << index;
                }
                foreach (var item in current)
                {
                    yield return item;
                    yield return (1UL << index) | item;
                }
            }
            else
            {
                foreach (var item in current)
                {
                    yield return item;
                }
            }
        }

        static ulong Second(string inputFile)
        {
            var parser = new InstructionParser2();
            var instructions = parser.ReadData(inputFile);

            var memoryProgram = new MemoryProgram();
            Instruction currentMask = null;
            foreach (var instruction in instructions)
            {
                if (instruction.Operation == OperationType.Mask)
                {
                    currentMask = instruction;
                    continue;
                }
                var writeAddresses = ApplyMask2(instruction.Argument1, currentMask.Argument1, currentMask.Argument2);
                memoryProgram.Write(writeAddresses, instruction.Argument2);
            }
            return memoryProgram.Memory.Values.Aggregate(0UL, (acc, val) => acc += val); ;
        }

        [DataTestMethod]
        [DataRow(1UL,0UL,1UL,0UL)]
        [DataRow(1UL,1UL,1UL,1UL)]
        [DataRow(1UL,0UL,0UL,1UL)]
        [DataRow(0UL,0UL,1UL,0UL)]
        [DataRow(0UL,1UL,1UL,1UL)]
        [DataRow(0UL,0UL,0UL,0UL)]
        public void TestApplyMask(ulong input, ulong write, ulong mask, ulong expectedResult)
        {
            var result = ApplyMask1(input, write, mask);
            Assert.AreEqual(expectedResult, result);
        }
        [DataTestMethod]
        [DataRow(new[] {true, true}, 3UL)]
        [DataRow(new[] { true, true, true}, 7UL)]
        [DataRow(new[] { false, true, true}, 3UL)]
        public void TestBitConvert(bool[] bits, ulong expectedResult)
        {
            var result = InstructionParser.ToUInt64(new System.Collections.BitArray(bits)); ;
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 165UL)]
        public void TestPart1(string inputFile, ulong expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test2.txt", 208UL)]
        public void TestPart2(string inputFile, ulong expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
