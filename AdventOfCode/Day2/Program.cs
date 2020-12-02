using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day2
{
    class Program
    {
        static readonly string input = "1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,9,1,19,1,19,5,23,1,23,6,27,2,9,27,31,1,5,31,35,1,35,10,39,1,39,10,43,2,43,9,47,1,6,47,51,2,51,6,55,1,5,55,59,2,59,10,63,1,9,63,67,1,9,67,71,2,71,6,75,1,5,75,79,1,5,79,83,1,9,83,87,2,87,10,91,2,10,91,95,1,95,9,99,2,99,9,103,2,10,103,107,2,9,107,111,1,111,5,115,1,115,2,119,1,119,6,0,99,2,0,14,0";
        static void Main(string[] args)
        {
            var test = new List<int> { 1, 1, 1, 4, 99, 5, 6, 0, 99 };
            Run(test);
            Debug.Assert(30 == test[0]);


            for(int i=0;i<100;i++)
            {
                for(int j=0;j<100;j++)
                {
                    int result = CheckProgram(i, j);
                    if (result == 19690720)
                    {
                        Console.WriteLine("Noun: {0}, Verb: {1}", i, j);
                        break;
                    }
                }
                Console.WriteLine("Next noun");
            }
            //Console.WriteLine("Postion 0: {0}", result);
        }

        private static int CheckProgram(int noun, int verb)
        {
            var program = input.Split(",").ToList().Select(s => Convert.ToInt32(s)).ToList();
            program[1] = noun;
            program[2] = verb;
            Run(program);
            int result = program[0];
            return result;
        }

        private static void Run(List<int> program)
        {
            int index = 0;
            while (index >= 0)
            {
                index = Process(program, index);
            }
        }

        private static int Process(List<int> program, int index)
        {
            var opCode = program[index];
            switch (opCode)
            {
                case 1:
                    //Addition
                    program[program[index + 3]] = program[program[index + 2]] + program[program[index + 1]];
                    return index + 4;
                case 2:
                    //Multiplication
                    program[program[index + 3]] = program[program[index + 2]] * program[program[index + 1]];
                    return index + 4;
                case 99:
                    //Terminiation
                    return -1;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
