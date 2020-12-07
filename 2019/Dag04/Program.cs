using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dag04
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            
            var first = new List<int[]> { new[] { 3 }, new[] { 4 }, new[] { 5 }, new[] { 6 }, new[] { 7 } };
            List<int[]> next = first;
            for (var gen=2; gen<=6;gen++)
            {
                next = AddGeneration(next, gen).ToList();
            }

            Console.WriteLine(next.Count);
            int n = 0;
            foreach (var password in next)
            {
                var counts = from p in password group p by p into s select new { Number = s.Key, Count = s.Count() };
                if (counts.Any(count => count.Count == 2)) { n = n + 1; }
            }
            Console.WriteLine(n);
        }

        private static IEnumerable<int[]> AddGeneration(List<int[]> current, int gen)
        {
            //var minValue = 356261;
            //var maxValue = 846303;

            for (int i = 0; i < current.Count; i++)
            {
                var currentElement = current[i];
                var lastElement = currentElement[gen - 2];
                //check if currentElement doesn't contain double numbers
                var shouldContainLast = gen == 6 && currentElement.Distinct().Count() == 5;
                for (int j = lastElement; j <= 9; j++)
                {
                    //gen = 2 => skip 3 and 4
                    if (gen==2 && lastElement == 3 && j <= 4){ continue; }
                    //gen = 3 => skip 5 for [3,5]
                    if (gen== 3 && lastElement == 5 && currentElement[0]==3 && j <= 5){ continue; }
                    //gen = 6 => check double numbers
                    if (shouldContainLast && lastElement!=j) { continue;}
                    int[] clone = new int[currentElement.Length + 1];
                    currentElement.CopyTo(clone, 0);
                    clone[currentElement.Length] = j;
                    yield return clone;
                }
            }
        }

        
    }
}
