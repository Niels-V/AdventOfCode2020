using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dag23
{
    [DebuggerDisplay("[{Position}] {CurrentLabel}{Label} ")]
    public class Cup
    {
        public int Label { get; set; }
        public Cup PreviousLabel { get; set; }
        public Cup NextLabel { get; set; }

        public Cup PreviousPosition { get; set; }
        public Cup NextPosition { get; set; }
    }

    public class Game
    {
        public List<Cup> Cups { get; internal set; }
        public int CupCount { get; }

        public Cup Current { get; set; }
        public Cup FirstLabel { get; }
        public Game(int[] labels, int maxCups)
        {
            Cups = new List<Cup>(maxCups);
            CupCount = maxCups;

            Cup lastCup = null;
            for (int i = 0; i < labels.Count(); i++)
            {
                var cup = new Cup {Label = labels[i]};
                if (i==0) { Current = cup; }
                if (lastCup != null)
                {
                    lastCup.NextPosition = cup;
                    cup.PreviousPosition = lastCup;
                }
                lastCup = cup;
                Cups.Add(cup);
            }
            FirstLabel = Cups.Single(c => c.Label == 1);
            var lastLabelCup = FirstLabel;
            foreach (var cup in Cups.OrderBy(c=>c.Label).Skip(1))
            {
                lastLabelCup.NextLabel = cup;
                cup.PreviousLabel = lastLabelCup;
                lastLabelCup = cup;
            }
            for (int i=labels.Count(); i<maxCups; i++)
            {
                var cup = new Cup { Label = i+1 };
                lastLabelCup.NextLabel = cup;
                cup.PreviousLabel = lastLabelCup;
                lastCup.NextPosition = cup;
                cup.PreviousPosition = lastCup;
                lastCup = cup;
                lastLabelCup = cup;
                Cups.Add(cup);
            }
            Current.PreviousPosition = lastCup;
            lastCup.NextPosition = Current;
            FirstLabel.PreviousLabel = lastLabelCup;
            lastLabelCup.NextLabel = FirstLabel;
        }

        public int CyclicPosition(int position)
        {
            position %= CupCount;
            return (position == 0) ? CupCount : position;
        }

        public void Play(int moves)
        {
            for (int i = 0; i < moves; i++)
            {
                DoMove();
            }
        }

        public string LabelsOrderFinished()
        {
            var sb = new StringBuilder();
            var currentCup = FirstLabel.NextPosition;
            for (int i = 1; i < CupCount; i++)
            {
                sb.Append(currentCup.Label);
                currentCup = currentCup.NextPosition;
            }
            return sb.ToString();
        }
        public long LabelFirstTwoNextProduct()
        {
            var next1 = FirstLabel.NextPosition;
            var next2 = next1.NextPosition;
            return ((long)next1.Label) * ((long)next2.Label);
        }

        public void DoMove()
        {
            //The crab picks up the three cups that are immediately clockwise of the current cup.
            var removed1 = Current.NextPosition;
            var removed2 = removed1.NextPosition;
            var removed3 = removed2.NextPosition;

            //They are removed from the circle; cup spacing is adjusted as necessary to maintain the circle.
            Current.NextPosition = removed3.NextPosition;
            removed3.NextPosition.PreviousPosition = Current;

            //The crab selects a destination cup: 
            //the cup with a label equal to the current cup's label minus one. 
            //If this would select one of the cups that was just picked up, the crab will keep subtracting one until it finds a cup that wasn't just picked up. 
            //If at any point in this process the value goes below the lowest value on any cup's label, it wraps around to the highest value on any cup's label instead.
            var destination = Current.PreviousLabel;
            while (destination.Label == removed1.Label ||
                    destination.Label == removed2.Label ||
                    destination.Label == removed3.Label)
            {
                destination = destination.PreviousLabel;
            }
            //The crab places the cups it just picked up so that they are immediately clockwise of the destination cup.They keep the same order as when they were picked up.
            var nextCup = destination.NextPosition;
            destination.NextPosition = removed1;
            removed1.PreviousPosition = destination;
            removed3.NextPosition = nextCup;
            nextCup.PreviousPosition = removed3;
            //The crab selects a new current cup: the cup which is immediately clockwise of the current cup.
            Current = Current.NextPosition;
        }
    }
}
