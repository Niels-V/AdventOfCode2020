using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Dag18
{
    public class InputParser : Parser<long>
    {
        protected override long ParseLine(string line)
        {
            var calculationResult = Program.Calculate(line);
            return Convert.ToInt64(calculationResult);
        }
    }
    public class InputParser2 : Parser<long>
    {
        protected override long ParseLine(string line)
        {
            var calculationResult = Program.Calculate2(line);
            return Convert.ToInt64(calculationResult);
        }
    }
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
            var parser = new InputParser();
            var results = parser.ReadData(inputFile);
            return results.Sum();
        }
        static string CalculateSimple(string calculation)
        {
            if (calculation.Contains('(') || calculation.Contains(')'))
            {
                throw new InvalidOperationException("This method only calculates from left to right without parenthesis.");
            }
            var calculationComponents = calculation.Split(" ");
            var accumulator = 0L;
            var operation = OperationType.None;
            for (int i = 0; i < calculationComponents.Length; i++)
            {
                var action = calculationComponents[i];
                if (action == "*" || action == "+")
                {
                    operation = action == "*" ? OperationType.Multiply : OperationType.Add;
                }
                else
                {
                    var number = Convert.ToInt64(action);
                    switch (operation)
                    {
                        case OperationType.None:
                            accumulator = number;
                            break;
                        case OperationType.Add:
                            accumulator += number;
                            operation = OperationType.None;
                            break;
                        case OperationType.Multiply:
                            accumulator *= number;
                            operation = OperationType.None;
                            break;
                    }
                }
                
            }
            return accumulator.ToString();
        }

        static Regex parenthesisFinder = new Regex("\\([^()]+\\)");
        public static string Calculate(string calculation)
        {
            while (calculation.Contains("("))
            {
                calculation = parenthesisFinder.Replace(calculation, ReplaceParenthesis);
            }
            return CalculateSimple(calculation);
        }

        static Regex additionFinder = new Regex("\\d+\\s[+]\\s\\d+");
        public static string CalculateAddition(string calculation)
        {
            if (calculation.Contains('(') || calculation.Contains(')'))
            {
                throw new InvalidOperationException("This method only replaces additions without parenthesis.");
            }
            while (calculation.Contains("+"))
            {
                calculation = additionFinder.Replace(calculation, ReplaceAddition);
            }
            return CalculateSimple(calculation);
        }

        public static string Calculate2(string calculation)
        {
            while (calculation.Contains("("))
            {
                calculation = parenthesisFinder.Replace(calculation, ReplaceParenthesis2);
            }
            return CalculateAddition(calculation);
        }
        public static string ReplaceParenthesis(Match m)
        {
            var withoutParentheses = m.Value.Replace("(", string.Empty).Replace(")", string.Empty);
            return CalculateSimple(withoutParentheses);
        }

        public static string ReplaceAddition(Match m)
        {
            return CalculateSimple(m.Value);
        }

        public static string ReplaceParenthesis2(Match m)
        {
            var withoutParentheses = m.Value.Replace("(", string.Empty).Replace(")", string.Empty);
            return CalculateAddition(withoutParentheses);
        }


        static long Second(string inputFile)
        {
            var parser = new InputParser2();
            var results = parser.ReadData(inputFile);
            return results.Sum();
        }
        [DataTestMethod]
        [DataRow("1 + 2 * 3 + 4 * 5 + 6", "71")]
        [DataRow("1 + (2 * 3) + (4 * (5 + 6))", "51")]
        [DataRow("2 * 3 + (4 * 5)","26")]
        [DataRow("5 + (8 * 3 + 9 + 3 * 4 * 3)", "437")]
        [DataRow("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", "12240")]
        [DataRow("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", "13632")]
        public void TestCalculate(string calculation, string expectedResult)
        {
            var result = Calculate(calculation);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("1 + 2 * 3 + 4 * 5 + 6", "231")]
        [DataRow("1 + (2 * 3) + (4 * (5 + 6))", "51")]
        [DataRow("2 * 3 + (4 * 5)", "46")]
        [DataRow("5 + (8 * 3 + 9 + 3 * 4 * 3)", "1445")]
        [DataRow("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", "669060")]
        [DataRow("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", "23340")]
        public void TestCalculate2(string calculation, string expectedResult)
        {
            var result = Calculate2(calculation);
            Assert.AreEqual(expectedResult, result);
        }

    }
}
