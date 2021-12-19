using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
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
            var numbers = new Parser().ParseFile(inputFile);
            var result = numbers.First();
            for (int i = 1; i < numbers.Count; i++)
            {
                result += numbers[i];
                ((SnailfishNumberPair)result).Reduce();
            }
            return result.Magnitude;
        }

        static long Second(string inputFile)
        {
            var parser = new Parser();
            var numbers = parser.ParseFile(inputFile);
            var maxMagnitude = 0L;
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = 0; j < numbers.Count; j++)
                {
                    if (i!=j)
                    {
                        var n = Parser.ParseNumber(numbers[i].ToString()) + Parser.ParseNumber(numbers[j].ToString());
                        n.Reduce();
                        var magnitude = n.Magnitude;
                        if (magnitude>maxMagnitude) { maxMagnitude = magnitude; }
                    }
                }
            }
            return maxMagnitude;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(4140, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(3993, result);
        }
    }

    [TestCategory("2021")]
    [TestClass]
    public class Parser : LineParser
    {
        public List<SnailfishNumber> ParseFile(string filePath)
        {
            var numbers = new List<SnailfishNumber>();
            foreach (var line in ReadData(filePath))
            {
                SnailfishNumberPair currentPair = ParseNumber(line);
                numbers.Add(currentPair);
            }
            return numbers;
        }

        internal static SnailfishNumberPair ParseNumber(string numberString)
        {
            SnailfishNumberPair currentPair = null;
            Stack<SnailfishNumberPair> pairs = new Stack<SnailfishNumberPair>();
            foreach (var letter in numberString)
            {
                switch (letter)
                {
                    case '[':
                        //add pair
                        if (currentPair != null) { pairs.Push(currentPair); }
                        currentPair = new SnailfishNumberPair();
                        var parent = pairs.Count > 0 ? pairs.Peek() : null;
                        if (parent != null)
                        {
                            currentPair.Parent = parent;
                            if (parent?.LeftNumber == null)
                            {
                                parent.SetLeft(currentPair);
                            }
                            else
                            {
                                parent.SetRight(currentPair);
                            }
                        }
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        var number = new SnailfishRegularNumber(Convert.ToInt32(letter.ToString()));
                        if (currentPair?.LeftNumber == null)
                        {
                            currentPair.SetLeft(number);
                        }
                        else
                        {
                            currentPair.SetRight(number);
                        }
                        break;
                    case ']':
                        currentPair = pairs.Count > 0 ? pairs.Pop() : currentPair;
                        break;
                    case ',':
                    //do nothing, logic is in set number
                    default:
                        break;
                }
            }

            return currentPair;
        }

        [TestMethod]
        public void TestParseByCounts()
        {
            var result = ParseFile("test.txt");
            Assert.AreEqual(10, result.Count);
        }

        [DataTestMethod]
        [DataRow("[[1,2],[[3,4],5]]", 143)]
        [DataRow("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384)]
        [DataRow("[[[[1,1],[2,2]],[3,3]],[4,4]]", 445)]
        [DataRow("[[[[3,0],[5,3]],[4,4]],[5,5]]", 791)]
        [DataRow("[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137)]
        [DataRow("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488)]
        [DataRow("[[[[7,8],[6,6]],[[6,0],[7,7]]],[[[7,8],[8,8]],[[7,9],[0,6]]]]", 3993)]
        public void TestParseMagnitued(string line, long expectedMagnitude)
        {
            var result = ParseNumber(line);
            Assert.AreEqual(expectedMagnitude, result.Magnitude);
        }

        [DataTestMethod]
        [DataRow("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
        [DataRow("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
        [DataRow("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
        [DataRow("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
        [DataRow("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
        public void TestReduce(string number1, string expected)
        {
            var a = ParseNumber(number1);
            a.Reduce();
            Assert.AreEqual(expected, a.ToString());
        }

        [DataTestMethod]
        [DataRow("[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]", "[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]", "[[[[7,8],[6,6]],[[6,0],[7,7]]],[[[7,8],[8,8]],[[7,9],[0,6]]]]")]
        [DataRow("[[[[4,3],4],4],[7,[[8,4],9]]]", "[1,1]", "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
        [DataRow("[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]", "[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]", "[[[[4,0],[5,4]],[[7,7],[6,0]]],[[8,[7,7]],[[7,9],[5,0]]]]")]
        [DataRow("[[[[4,0],[5,4]],[[7,7],[6,0]]],[[8,[7,7]],[[7,9],[5,0]]]]", "[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]", "[[[[6,7],[6,7]],[[7,7],[0,7]]],[[[8,7],[7,7]],[[8,8],[8,0]]]]")]
        public void TestAdditionWithReduce(string number1, string number2, string expected)
        {
            var n1 = ParseNumber(number1);
            var n2 = ParseNumber(number2);
            var a = n1 + n2;
            a.Reduce();
            Assert.AreEqual(expected, a.ToString());
        }

    }
}
