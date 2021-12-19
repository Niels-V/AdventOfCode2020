using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AoC
{
    public class SnailfishNumberPair : SnailfishNumber
    {
        public SnailfishNumberPair(SnailfishNumber n1, SnailfishNumber n2)
        {
            SetLeft(n1);
            SetRight(n2);
        }
        public SnailfishNumberPair()
        {
        }
        public void SetLeft(SnailfishNumber n)
        {
            LeftNumber = n;
            LeftNumber.Parent = this;
            LeftNumber.NumberPosition = PairPosition.Left;
        }
        public void SetRight(SnailfishNumber n)
        {
            RightNumber = n;
            RightNumber.Parent = this;
            RightNumber.NumberPosition = PairPosition.Right;
        }

        public SnailfishNumber LeftNumber { get; set; }
        public SnailfishNumber RightNumber { get; set; }

        //The magnitude of a pair is 3 times the magnitude of its left element plus 2 times the magnitude of its right element.
        public override long Magnitude => 3*LeftNumber.Magnitude + 2*RightNumber.Magnitude;

        public void Explode()
        {
            //To explode a pair, the pair's left value is added to the first regular number to the left of the exploding pair (if any),
            //and the pair's right value is added to the first regular number to the right of the exploding pair (if any).
            //Exploding pairs will always consist of two regular numbers. Then, the entire exploding pair is replaced with the regular number 0.

            //Change number to the left
            var current = this;
            while (current != null)
            {
                if (current.NumberPosition == PairPosition.Right)
                {
                    current.Parent.LeftNumber.AddToRight(((SnailfishRegularNumber)LeftNumber).Number);
                    break;
                }
                current = current.Parent;
            }
            //Change number to the right
            current = this;
            while (current != null)
            {
                if (current.NumberPosition == PairPosition.Left)
                {
                    current.Parent.RightNumber.AddToLeft(((SnailfishRegularNumber)RightNumber).Number);
                    break;
                }
                current = current.Parent;
            }

            //Replace this pair with a zero
            var currentParent = Parent;
            SnailfishRegularNumber replacement = new SnailfishRegularNumber(0)
            {
                Parent = currentParent
            };
            if (NumberPosition == PairPosition.Left)
            {
                replacement.NumberPosition = PairPosition.Left;
                replacement.Parent.LeftNumber = replacement;
            }
            else
            {
                replacement.NumberPosition = PairPosition.Right;
                replacement.Parent.RightNumber = replacement;
            }
        }
        public void Reduce()
        {
            var doExplode = true;
            var doSplit = true;
            while (doExplode || doSplit)
            {
                while (doExplode)
                {
                    doExplode = CheckExplode(1);
                }
                doSplit = CheckSplit();
                if (doSplit) { doExplode = true; }
            }
        }
        public override bool CheckExplode(int pairCount)
        {
            //To reduce a snailfish number, you must repeatedly do the first action in this list that applies to the snailfish number:
            //If any pair is nested inside four pairs, the leftmost such pair explodes.
            if (pairCount > 4)
            {
                Explode();
                return true;
            }
            else
            {
                if (LeftNumber.CheckExplode(pairCount + 1))
                {
                    return true;
                }
                if (RightNumber.CheckExplode(pairCount + 1))
                {
                    return true;
                }
                return false;
            }
        }

        public override bool CheckSplit() => LeftNumber.CheckSplit() || RightNumber.CheckSplit() || false;

        public override string ToString()
        {
            return $"[{LeftNumber},{RightNumber}]";
        }

        public override void AddToLeft(int number)
        {
            LeftNumber.AddToLeft(number);
        }

        public override void AddToRight(int number)
        {
            RightNumber.AddToRight(number);
        }
    }

    public class SnailfishRegularNumber : SnailfishNumber
    {
        public SnailfishRegularNumber(int n)
        {
            Number = n;
        }

        public int Number { get; set; }

        //The magnitude of a regular number is just that number.
        public override long Magnitude => Number;
        public override string ToString()
        {
            return Number.ToString();
        }

        public override bool CheckExplode(int pairCount) => false;

        public override bool CheckSplit()
        {
            //If any regular number is 10 or greater, the leftmost such regular number splits.
            if (Number >= 10)
            {
                Split();
                return true;
            }
            return false;
        }

        public void Split()
        {
            //To split a regular number, replace it with a pair;
            //the left element of the pair should be the regular number divided by two and rounded down,
            //while the right element of the pair should be the regular number divided by two and rounded up.
            //For example, 10 becomes [5,5], 11 becomes [5,6], 12 becomes [6,6], and so on.
            var half = Math.DivRem(Number, 2, out int remainder);
            Number = half;
            var right = new SnailfishRegularNumber(half + remainder);
            var currentParent = Parent;
            var currenPosition = NumberPosition;
            SnailfishNumberPair replacement = new SnailfishNumberPair(this, right);
            replacement.Parent = currentParent;
            replacement.NumberPosition = currenPosition;
            if (currenPosition == PairPosition.Left)
            {
                currentParent.LeftNumber = replacement;
            }
            else
            {
                currentParent.RightNumber = replacement;
            }
        }

        public override void AddToLeft(int number)
        {
            Number += number;
        }

        public override void AddToRight(int number)
        {
            Number += number;
        }
    }

    public abstract class SnailfishNumber
    {
        public SnailfishNumberPair Parent { get; set; }
        public PairPosition NumberPosition { get; set; }
        public abstract long Magnitude { get; }

        public static SnailfishNumber operator +(SnailfishNumber a) => a;

        public static SnailfishNumberPair operator +(SnailfishNumber left, SnailfishNumber right)
            => new SnailfishNumberPair(left, right);

        public abstract bool CheckExplode(int pairCount);
        public abstract bool CheckSplit();

        public abstract void AddToLeft(int number);
        public abstract void AddToRight(int number);
    }

    public enum PairPosition
    {
        Unknown,
        Left,
        Right
    }

    [TestCategory("2021")]
    [TestClass]
    public class ReduceTester
    {
        [TestMethod]
        public void SingleExplode()
        {
            //[[[[[9,8],1],2],3],4] => [[[[0,9],2],3],4]
            var pair = new SnailfishNumberPair(new SnailfishRegularNumber(9), new SnailfishRegularNumber(8));
            var pair2 = new SnailfishNumberPair(pair, new SnailfishRegularNumber(1));
            var pair3 = new SnailfishNumberPair(pair2, new SnailfishRegularNumber(2));
            var pair4 = new SnailfishNumberPair(pair3, new SnailfishRegularNumber(3));
            var pair5 = new SnailfishNumberPair(pair4, new SnailfishRegularNumber(4));

            pair5.Reduce();
            Assert.AreEqual(true, pair2.LeftNumber is SnailfishRegularNumber);
            Assert.AreEqual(true, pair2.RightNumber is SnailfishRegularNumber);
            Assert.AreEqual(0, ((SnailfishRegularNumber)pair2.LeftNumber).Number);
            Assert.AreEqual(9, ((SnailfishRegularNumber)pair2.RightNumber).Number);
        }
        [TestMethod]
        public void SingleMagnitude()
        {
            //[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]
            //    \1/   \2/     \3/   \4/       \5/   \6/    \7/
            //     ***8***       ***9***         ***10**      *
            //        ******11******                *****12****
            //              ************13***************
            var pair1 = new SnailfishNumberPair(new SnailfishRegularNumber(8), new SnailfishRegularNumber(7));
            var pair2 = new SnailfishNumberPair(new SnailfishRegularNumber(7), new SnailfishRegularNumber(7));
            var pair3 = new SnailfishNumberPair(new SnailfishRegularNumber(8), new SnailfishRegularNumber(6));
            var pair4 = new SnailfishNumberPair(new SnailfishRegularNumber(7), new SnailfishRegularNumber(7));
            var pair5 = new SnailfishNumberPair(new SnailfishRegularNumber(0), new SnailfishRegularNumber(7));
            var pair6 = new SnailfishNumberPair(new SnailfishRegularNumber(6), new SnailfishRegularNumber(6));
            var pair7 = new SnailfishNumberPair(new SnailfishRegularNumber(8), new SnailfishRegularNumber(7));

            var pair8 = new SnailfishNumberPair(pair1, pair2);
            var pair9 = new SnailfishNumberPair(pair3, pair4);
            var pair10 = new SnailfishNumberPair(pair5, pair6);
            var pair11 = new SnailfishNumberPair(pair8, pair9);
            var pair12 = new SnailfishNumberPair(pair10, pair7);
            var pair13 = new SnailfishNumberPair(pair11, pair12);

            Assert.AreEqual(3488, pair13.Magnitude);
        }
    }
}
