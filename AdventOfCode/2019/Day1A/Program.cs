using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day1A
{
    class Program
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        static IEnumerable<int> readInts(string filePath) => readlines(filePath).Select(s => Convert.ToInt32(s));


        static void MainA(string[] args)
        {
            var ints = readInts("input.txt");
            Debug.Assert(2 == fuelRequirement(12), "Expected outcome");
            Debug.Assert(2 == fuelRequirement(14), "Expected outcome");
            Debug.Assert(654 == fuelRequirement(1969), "Expected outcome");
            Debug.Assert(33583 == fuelRequirement(100756), "Expected outcome");
            
            var fuelNeeded = ints.Select(fuelRequirement).Sum();
            Console.WriteLine("{0}", fuelNeeded);
        }

        static void Main(string[] args)
        {
            var ints = readInts("input.txt");
            Debug.Assert(2 == fuelRequirementFull(12), "Expected outcome");
            Debug.Assert(2 == fuelRequirementFull(14), "Expected outcome");
            Debug.Assert(966 == fuelRequirementFull(1969), "Expected outcome");
            Debug.Assert(50346 == fuelRequirementFull(100756), "Expected outcome");

            var fuelNeeded = ints.Select(fuelRequirementFull).Sum();
            Console.WriteLine("{0}", fuelNeeded);
        }
        static int fuelRequirement(int mass)
        {
            return (mass / 3) - 2;
        }

        static int fuelRequirementFull(int mass)
        {
            int fuelNeeded = fuelRequirement(mass);
            int fullFuelNeeded = 0;
            while(fuelNeeded > 0)
            {
                fullFuelNeeded += fuelNeeded;
                fuelNeeded = fuelRequirement(fuelNeeded);
            }
            return fullFuelNeeded;
        }
    }
}
