using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            var result = -1;
            var parser = new MapParser();
            var map = parser.ReadMap(inputFile);
            parser.Map = map;
            var N = map.GetUpperBound(0);
            var M = map.GetUpperBound(1);
            while (parser.X>0 && parser.X<N && parser.Y > 0 && parser.Y < M)
            {
                parser.NextStep();
            }
            return parser.Step;
        }

        static long Second(string inputFile)
        {
            var result = -1;
            return result;
        }


        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(41, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(-1, result);
        }


    }
    public enum TileElement
    {
        [EnumMember(Value = ".")]
        Dot,
        [EnumMember(Value = "#")]
        Hash,
        [EnumMember(Value = "^")]
        Guard
    }

    public enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public class MapParser : CharMapParser<TileElement>
    {
        public int Step { get; set; } = 0;
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public Direction Direction { get; set; } = Direction.Up;
        public TileElement[,] Map { get; set; }

        protected override TileElement Convert(char input, int i, int j)
        {
            var result = Convert(input);
            if (result == TileElement.Guard)
            {
                X = i; Y = j; 
            }
            return result;
        }
        protected override TileElement Convert(char input)
        {
            return input.ToString().ParseEnumValue<TileElement>();
        }

        internal void NextStep()
        {
            var xNext = X;
            var yNext = Y;
            switch (Direction)
            {
                case Direction.Up:
                    yNext = yNext - 1;
                    break;
                case Direction.Right:
                    xNext = xNext + 1;
                    break;
                case Direction.Down:
                    yNext = yNext + 1;
                    break;
                case Direction.Left:
                    xNext = xNext - 1;
                    break;
                default:
                    break;
            }
            if (Map[xNext,yNext] == TileElement.Hash)
            {
                Direction = (Direction)(((int)Direction + 1) % 4);
            }
            else
            {
                X = xNext; Y = yNext;Step++;
            }
        }
    }
}
