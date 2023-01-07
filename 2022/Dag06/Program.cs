using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2022")]
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
            var line = System.IO.File.ReadLines(inputFile).First();
            return FindMarker(line,4);
        }

        static int FindMarker(string line, int markerLength) {
            char[] p = new char[markerLength];
            for (int j=markerLength-2;j>=0; j--) {
                p[j+1]=line[j];
            }
            for (int i=markerLength-1; i< line.Length; i++) {
                for (int j=0;j<markerLength-1;j++) {
                    p[j]=p[j+1];
                }
                p[markerLength-1]=line[i];
                var notEqual = true;
                for (int j = 0; j < markerLength; j++)
                {
                    for (int k = j+1; k < markerLength; k++)
                    {
                        if (p[j]==p[k]) {notEqual=false;
                            break;
                        }
                    }
                    if (notEqual==false) {break;}
                }
                if (notEqual) {
                    return i+1;
                }
            }
            return -1;
        }

        static long Second(string inputFile)
        {
            var line = System.IO.File.ReadLines(inputFile).First();
            return FindMarker(line,14);
        }

        [TestMethod]
        [DataRow(7,"mjqjpqmgbljsphdztnvjfqwrcgsmlb")]
        [DataRow(5,"bvwbjplbgvbhsrlpgdmjqwftvncz")]
        [DataRow(6,"nppdvjthqldpwncqszvftbrmjlhg")]
        [DataRow(10,"nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg")]
        [DataRow(11,"zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw")]
        public void TestPart1(int expected, string inputline)
        {
            var result = FindMarker(inputline,4);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(19,"mjqjpqmgbljsphdztnvjfqwrcgsmlb")]
        [DataRow(23,"bvwbjplbgvbhsrlpgdmjqwftvncz")]
        [DataRow(23,"nppdvjthqldpwncqszvftbrmjlhg")]
        [DataRow(29,"nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg")]
        [DataRow(26,"zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw")]
        public void TestPart2(int expected, string inputline)
        {
            var result = FindMarker(inputline,14);
            Assert.AreEqual(expected, result);
        }
    }
}
