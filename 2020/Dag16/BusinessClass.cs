using System;
using System.Collections.Generic;
using System.Text;

namespace Dag16
{
    public class TicketProperty
    {
        public TicketProperty(string name, int min1, int max1, int min2, int max2)
        {
            Name = name;
            FirstRange = new TicketRange { Minimum = min1, Maximum = max1 };
            SecondRange = new TicketRange { Minimum = min2, Maximum = max2 };

        }

        public string Name { get; set; }
        public TicketRange FirstRange { get; set; }
        public TicketRange SecondRange { get; set; }

        public int? TicketIndex { get; set; }

        public bool ValidProperty(int value)
        {
            return ValidationRule()(value);
        }

        public Predicate<int> ValidationRule()
        {
            return (value)=>value >= FirstRange.Minimum && value <= FirstRange.Maximum || value >= SecondRange.Minimum && value <= SecondRange.Maximum;
        }
    }

    public struct TicketRange
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
    }
}
