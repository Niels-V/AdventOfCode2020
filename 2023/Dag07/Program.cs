using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace AoC
{
    [TestCategory("2023")]
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

        static int First(string inputFile)
        {
            var parser = new HandParser();
            var hands = parser.ReadData(inputFile);

            var sorted = hands.OrderBy(h => h);
            var result = sorted.Select((hand, index) => hand.Bid*(index+1)).Sum();
            return result;
        }

        static long Second(string inputFile)
        {
            var parser = new HandWithJokerParser();
            var hands = parser.ReadData(inputFile);

            var sorted = hands.OrderBy(h => h);
            var result = sorted.Select((hand, index) => hand.Bid*(index+1)).Sum();
            return result;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(6440, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(5905, result);
        }
    }
    [TestClass]
    public class HandParser : Common.Parser<Hand>
    {
        protected override Hand ParseLine(string line)
        {
            return new Hand(line);
        }
        [TestMethod]
        [DataRow("AAAAA 1", HandType.FiveOfAKind, 1)]
        [DataRow("AA8AA 1", HandType.FourOfAKind, 1)]
        [DataRow("23332 1", HandType.FullHouse, 1)]
        [DataRow("TTT98 1", HandType.ThreeOfAKind, 1)]
        [DataRow("23432 1", HandType.TwoPair, 1)]
        [DataRow("A23A4 1", HandType.OnePair, 1)]
        [DataRow("23456 1", HandType.HighCard, 1)]
        public void TestParse(string line, HandType expectedHand, int expectedBid)
        {
            var hand = new Hand(line);
            Assert.AreEqual(expectedHand, hand.HandType);
            Assert.AreEqual(expectedBid, hand.Bid);
        }
    }

    [TestClass]
    public class HandWithJokerParser : Common.Parser<Hand>
    {
        protected override Hand ParseLine(string line)
        {
            return new Hand(line, true);
        }
        [TestMethod]
        [DataRow("AAAAA 1", HandType.FiveOfAKind, 1)]
        [DataRow("AA8AA 1", HandType.FourOfAKind, 1)]
        [DataRow("23332 1", HandType.FullHouse, 1)]
        [DataRow("TTT98 1", HandType.ThreeOfAKind, 1)]
        [DataRow("23432 1", HandType.TwoPair, 1)]
        [DataRow("A23A4 1", HandType.OnePair, 1)]
        [DataRow("23456 1", HandType.HighCard, 1)]
        [DataRow("32T3K 765", HandType.OnePair, 765)]
        [DataRow("T55J5 684", HandType.FourOfAKind, 684)]
        [DataRow("KK677 28", HandType.TwoPair, 28)]
        [DataRow("KTJJT 220", HandType.FourOfAKind, 220)]
        [DataRow("QQQJA 483", HandType.FourOfAKind, 483)]
        public void TestParse(string line, HandType expectedHand, int expectedBid)
        {
            var hand = new Hand(line,true);
            Assert.AreEqual(expectedHand, hand.HandType);
            Assert.AreEqual(expectedBid, hand.Bid);
        }
    }

    public class Hand : IComparable<Hand>, IComparer<Hand> 
    {
        public bool JokerRules {get; private set;}
        public HandType HandType { get; set; }
        public CardType[] Cards { get; set; } = new CardType[5];
        public int Bid { get; set; }
        public Hand(string entryLine, bool withJokers = false)
        {
            JokerRules = withJokers;
            var line = entryLine.Split(' ');
            var handstring = line[0];
            var bid = Convert.ToInt32(line[1]);
            Bid = bid;
            for (int i = 0; i < 5; i++)
            {
                Cards[i] = handstring[i].ToString().ParseEnumValue<CardType>();
            }
            HandType = DetermineHandType();
        }

        public HandType DetermineHandType()
        {
            var query = from r in Cards
                        group r by r into g
                        select new { Count = g.Count(), Value = g.Key };
            if (query.Count()==1)
            {
                return HandType.FiveOfAKind;
            }
            if (query.Count() == 2 && (query.ElementAt(0).Count == 1|| query.ElementAt(0).Count == 4))
            {
                if (JokerRules && query.ElementAt(0).Value == CardType.Jack ||
                    query.ElementAt(1).Value == CardType.Jack)
                    {
                        return HandType.FiveOfAKind;
                    }
                return HandType.FourOfAKind;
            }
            if (query.Count() == 2 && (query.ElementAt(0).Count == 2 || query.ElementAt(0).Count == 3))
            {
                if (JokerRules && query.ElementAt(0).Value == CardType.Jack ||
                    query.ElementAt(1).Value == CardType.Jack)
                    {
                        return HandType.FiveOfAKind;
                    }
                return HandType.FullHouse;
            }
            if (query.Count() == 3 && (query.ElementAt(0).Count == 3 || query.ElementAt(1).Count == 3 || query.ElementAt(2).Count == 3))
            {
                if (JokerRules &&   
                    (query.ElementAt(0).Value == CardType.Jack) ||
                     (query.ElementAt(1).Value == CardType.Jack) ||
                     (query.ElementAt(2).Value == CardType.Jack)
                ) {
                    return HandType.FourOfAKind;
                }
                return HandType.ThreeOfAKind;
            }
            if (query.Count() == 3 && (query.ElementAt(0).Count == 2 || query.ElementAt(1).Count == 2 || query.ElementAt(2).Count == 2))
            {
                if (JokerRules &&   (
                     (query.ElementAt(0).Count==2 && query.ElementAt(0).Value == CardType.Jack) ||
                     (query.ElementAt(1).Count==2 && query.ElementAt(1).Value == CardType.Jack) ||
                     (query.ElementAt(2).Count==2 && query.ElementAt(2).Value == CardType.Jack)
                )) {
                    return HandType.FourOfAKind;
                }
                if (JokerRules &&  ( 
                     (query.ElementAt(0).Count==1 && query.ElementAt(0).Value == CardType.Jack) ||
                     (query.ElementAt(1).Count==1 && query.ElementAt(1).Value == CardType.Jack) ||
                     (query.ElementAt(2).Count==1 && query.ElementAt(2).Value == CardType.Jack)
                )) {
                    return HandType.FullHouse;
                }
                return HandType.TwoPair;
            }
            if (query.Count() == 4)
            {
                if (JokerRules &&   (
                     (query.ElementAt(0).Value == CardType.Jack) ||
                     (query.ElementAt(1).Value == CardType.Jack) ||
                     (query.ElementAt(2).Value == CardType.Jack) ||
                     (query.ElementAt(3).Value == CardType.Jack))
                ) {
                    return HandType.ThreeOfAKind;
                }
                return HandType.OnePair;
            }
            if (JokerRules && (
                     (query.ElementAt(0).Value == CardType.Jack) ||
                     (query.ElementAt(1).Value == CardType.Jack) ||
                     (query.ElementAt(2).Value == CardType.Jack) ||
                     (query.ElementAt(3).Value == CardType.Jack) ||
                     (query.ElementAt(4).Value == CardType.Jack))
                ) {
                    return HandType.OnePair;
                }
            return HandType.HighCard;
        }

        public int CompareTo(Hand other)
        {
            if (this.HandType == other.HandType)
            {
                for (int i = 0; i < 5; i++)
                {
                    var compareResult = Cards[i].CompareTo(other.Cards[i]);
                    if (JokerRules && compareResult!=0) {
                        if (Cards[i] == CardType.Jack) {return -1;}
                        if (other.Cards[i] == CardType.Jack) {return 1;}
                    }
                    if (compareResult !=0)
                    {
                        return compareResult;
                    }
                }
            }
            return HandType.CompareTo(other.HandType);
        }

        public int Compare(Hand x, Hand y)
        {
            return x.CompareTo(y);
        }
    }

    public enum HandType {
        HighCard = 1, //where all cards' labels are distinct: 23456
        OnePair = 2, //where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
        TwoPair = 3, //where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
        ThreeOfAKind = 4, //where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
        FullHouse = 5, //where three cards have the same label, and the remaining two cards share a different label: 23332
        FourOfAKind = 6, //where four cards have the same label and one card has a different label: AA8AA
        FiveOfAKind = 7,// where all five cards have the same label: AAAAA
    }

    public enum CardType
    {
        [EnumMember(Value = "A")] 
        Ace = 14,
        [EnumMember(Value = "K")] 
        King = 13,
        [EnumMember(Value = "Q")] 
        Queen = 12,
        [EnumMember(Value = "J")] 
        Jack = 11,
        [EnumMember(Value = "T")] 
        Ten = 10,
        [EnumMember(Value = "9")] 
        Nine = 9,
        [EnumMember(Value = "8")]
        Eight = 8,
        [EnumMember(Value = "7")]
        Seven = 7,
        [EnumMember(Value = "6")] 
        Six = 6,
        [EnumMember(Value = "5")] 
        Five = 5,
        [EnumMember(Value = "4")] 
        Four = 4,
        [EnumMember(Value = "3")]
        Three = 3,
        [EnumMember(Value = "2")]
        Two = 2
    }
}
