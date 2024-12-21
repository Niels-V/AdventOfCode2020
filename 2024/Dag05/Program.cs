using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC
{

    [TestCategory("2024")]
    [TestClass]
    public partial class Program
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
            var pageOrders = PagesProcessor.ReadData(inputFile);
            var result = 0;
            foreach (var pagePrint in pageOrders.PagePrints)
            {
                bool validPrint = pageOrders.CheckValidity(pagePrint);
                if (validPrint)
                {
                    result += pagePrint[pagePrint.Length / 2];
                }
            }
            return result;
        }

        static long Second(string inputFile)
        {
            var pageOrders = PagesProcessor.ReadData(inputFile);
            var result = 0;
            foreach (var pagePrint in pageOrders.PagePrints)
            {
                bool validPrint = pageOrders.CheckValidity(pagePrint);
                if (!validPrint)
                {
                    var reorder = pageOrders.Reorder(pagePrint);
                    result += pagePrint[reorder.Length / 2];
                }
            }
            return result;
        }


        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(143, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(123, result);
        }


    }
    public class PageOrderInfo
    {
        public PageOrderInfo()
        {
            PageOrderRules = [];
            PagePrints = [];
        }
        public List<Tuple<int, int>> PageOrderRules { get; set; }
        public List<int[]> PagePrints { get; internal set; }

        internal bool CheckValidity(int[] pagePrint)
        {
            for (int i = 0; i < pagePrint.Length-1; i++)
            {
                for (int j = i+1; j<pagePrint.Length; j++)
                {
                    var pageToCheck = new Tuple<int, int>(pagePrint[i], pagePrint[j]);
                    bool pageResult = CheckPage(pageToCheck);
                    if (pageResult == false) { return false; }
                }
            }
            return true;
        }

        internal int[] Reorder(int[] pagePrint)
        {
            int[] reorder = pagePrint;
            for (int i = 0; i < reorder.Length - 1; i++)
            {
                for (int j = i + 1; j < reorder.Length; j++)
                {
                    var pageToCheck = new Tuple<int, int>(reorder[i], reorder[j]);
                    bool pageResult = CheckPage(pageToCheck);
                    if (pageResult == false) {
                        reorder[i] = pageToCheck.Item2;
                        reorder[j] = pageToCheck.Item1;
                    }
                }
            }
            return reorder;
        }

        private bool CheckPage(Tuple<int, int> pageToCheck)
        {
            foreach (var rule in PageOrderRules)
            {
                if (rule.Item1 == pageToCheck.Item2 && rule.Item2 == pageToCheck.Item1) { return false; }
                if (rule.Item1 == pageToCheck.Item1 && rule.Item2 == pageToCheck.Item2) { return true; }
            }
            return true;
        }
    }


    public class PagesProcessor
    {
        protected static IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);
        public static PageOrderInfo ReadData(string filePath)
        {
            var result = new PageOrderInfo();
            var lines = Readlines(filePath);
            var procesRules = true;
            foreach (var line in lines)
            {
                if (line == "") { procesRules = false; continue; }
                if (procesRules)
                {
                    var lineData = line.Split('|');
                    var pageOrder = new Tuple<int, int>(Convert.ToInt32(lineData[0]), Convert.ToInt32(lineData[1]));
                    result.PageOrderRules.Add(pageOrder);
                }
                else
                {
                    result.PagePrints.Add(line.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(number => Convert.ToInt32(number)).ToArray());
                }
            }
            return result;
        }
    }
}
