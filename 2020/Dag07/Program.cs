using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dag07
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var bagCount = FindContainingBagColorCount("input.txt");

            Console.WriteLine("Bag colors found: {0}", bagCount);
            var insideBagCount = FindBagsInsideBagColorCount("input.txt", "shiny gold");
            Console.WriteLine("Bags inside found: {0}", insideBagCount);
        }

        private static int FindContainingBagColorCount(string inputFile)
        {
            var bagRules = Parser.ReadData(inputFile).ToList();
            var inputColors = new List<string>() { "shiny gold" };
            var processedColors = new List<string>();
            var oldColorCount = -1;

            while (oldColorCount != processedColors.Count)
            {
                oldColorCount = processedColors.Count;
                var containingColors = bagRules.Where(b => b.ShouldContain.Any(c => inputColors.Any(ic => ic == c.Key))).Select(b => b.Color).Distinct().ToList();
                processedColors = processedColors.Union(containingColors).ToList();
                inputColors = containingColors;
            }
            return processedColors.Count;
        }

        private static int FindBagsInsideBagColorCount(string inputFile, string startColor)
        {
            var bagRules = Parser.ReadData(inputFile).ToList();
            Bag bag = new Bag { Color = startColor };
            AddInsideBags(bagRules, bag);
            
            return bag.ContentsCount(bagRules) -1;
        }

        private static void AddInsideBags(List<BagRule> bagRules, Bag bag)
        {
            bag.Contents.AddRange(bagRules.First(b => b.Color == bag.Color).ShouldContain.Select(b => new Bag { Color = b.Key }));
            foreach (var bag2 in bag.Contents)
            {
                AddInsideBags(bagRules, bag2);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var bagCount = FindContainingBagColorCount("test.txt");
            Assert.AreEqual(4, bagCount);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var bagCount = FindBagsInsideBagColorCount("test2.txt", "dark blue");
            Assert.AreEqual(2, bagCount);
            bagCount = FindBagsInsideBagColorCount("test2.txt", "dark green");
            Assert.AreEqual(6, bagCount);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var bagCount = FindBagsInsideBagColorCount("test2.txt", "shiny gold");
            Assert.AreEqual(126, bagCount);
        }
    }
}
