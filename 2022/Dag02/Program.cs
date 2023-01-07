using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AoC
{
    public enum OpponentOption
    {
        [EnumMember(Value="A")]
        Rock,
        [EnumMember(Value="B")]
        Paper,
        [EnumMember(Value="C")]
        Scissors
    }
    public enum MyOption
    {
        [EnumMember(Value="X")]
        Rock,
        [EnumMember(Value="Y")]
        Paper,
        [EnumMember(Value="Z")]
        Scissors
    }

    public enum ResultOption
    {
        [EnumMember(Value="X")]
        Lose=0,
        [EnumMember(Value="Y")]
        Draw=3,
        [EnumMember(Value="Z")]
        Win=6
    }

    public record Rpc2Turn {
        public OpponentOption Opponent;
        public ResultOption Outcome;
        public int Score() {
            return ((int)My()+1)+((int)Outcome);
        }
        public MyOption My() {
            if (Outcome==ResultOption.Draw){return (MyOption)Opponent;}
            if (Opponent==OpponentOption.Rock) {
                return Outcome==ResultOption.Win ? MyOption.Paper : MyOption.Scissors;
            }
            if (Opponent==OpponentOption.Paper) {
                return Outcome==ResultOption.Win ? MyOption.Scissors : MyOption.Rock;
            }
            if (Opponent==OpponentOption.Scissors) {
                return Outcome==ResultOption.Win ? MyOption.Rock : MyOption.Paper;
            }
            return 0;
        }
    }

    public record RpcTurn {
        public OpponentOption Opponent;
        public MyOption My;
        public int Score() {
            return ((int)My+1)+Outcome();
        }
        public int Outcome() {
            if (((int)My)==((int)Opponent)) {return 3;}
            if ((Opponent==OpponentOption.Rock && My==MyOption.Paper) ||
                (Opponent==OpponentOption.Paper && My==MyOption.Scissors) ||
                (Opponent==OpponentOption.Scissors && My==MyOption.Rock)) {return 6;}
            return 0;
        }
    }

    public class RpcParser : Common.Parser<RpcTurn> {
        protected override RpcTurn ParseLine(string line) {
            return new RpcTurn{
                Opponent = line[0].ToString().ParseEnumValue<OpponentOption>(),
                My = line[2].ToString().ParseEnumValue<MyOption>()
            };
        }
    }

    public class Rpc2Parser : Common.Parser<Rpc2Turn> {
        protected override Rpc2Turn ParseLine(string line) {
            return new Rpc2Turn{
                Opponent = line[0].ToString().ParseEnumValue<OpponentOption>(),
                Outcome = line[2].ToString().ParseEnumValue<ResultOption>()
            };
        }
    }

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
            var parser = new RpcParser();
            var turns = parser.ReadData(inputFile);
            return turns.Sum(t=>t.Score());
        }

        static long Second(string inputFile)
        {
            var parser = new Rpc2Parser();
            var turns = parser.ReadData(inputFile);
            return turns.Sum(t=>t.Score());
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(12, result);
        }
    }
}
