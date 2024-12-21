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
            var memory = System.IO.File.ReadAllText(inputFile);
            var mulRegex = MultiplicationRegex();
            var matches = mulRegex.Matches(memory);
            return matches.Sum(m => Convert.ToInt32(m.Groups["op1"].Value) * Convert.ToInt32(m.Groups["op2"].Value));
        }

        static long Second(string inputFile)
        {
            var memory = System.IO.File.ReadAllText(inputFile);
            var dontRegex = DontRegex();
            Console.WriteLine(dontRegex.Matches(memory).Count);
            //filter all the disabled 'memory' with a regex.
            var cleanedMemory = dontRegex.Replace(memory, string.Empty);
            //do the same calculation on the filtered memory string.
            var mulRegex = MultiplicationRegex();
            var matches = mulRegex.Matches(cleanedMemory);
            return matches.Sum(m => Convert.ToInt32(m.Groups["op1"].Value) * Convert.ToInt32(m.Groups["op2"].Value));
        }

        
        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(161, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test2.txt");
            Assert.AreEqual(48, result);
            //138644259 is fout 
        }

        [GeneratedRegex(@"mul\((?<op1>\d{1,3})\,(?<op2>\d{1,3})\)", RegexOptions.Singleline | RegexOptions.Compiled)]
        private static partial Regex MultiplicationRegex();
        //((don't\(\)(.*?))+do\(\)) : Match as many don't() strings with lazy custom text until you find a do() OR
        //((don't\(\)(.*?))+$)      : Match as many don't() strings with lazy custom text until end of file
        [GeneratedRegex(@"((don't\(\)(.*?))+do\(\))|((don't\(\)(.*?))+$)", RegexOptions.Singleline | RegexOptions.Compiled)]
        private static partial Regex DontRegex();
    }
}
