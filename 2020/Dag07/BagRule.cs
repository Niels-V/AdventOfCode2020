using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dag07
{
    public class BagRule
    {
        public string Color { get; set; }
        public Dictionary<string,int> ShouldContain { get; }

        public BagRule()
        {
            ShouldContain = new Dictionary<string, int>();
        }
    }

    public class Bag
    {
        public string Color { get; set; }
        public List<Bag> Contents { get; }
        public int ContentsCount(List<BagRule> rules)
        {
            return 1+Contents.Sum(b => b.ContentsCount(rules) * rules.First(c => c.Color == Color).ShouldContain[b.Color]);
        }

        public Bag()
        {
            Contents = new List<Bag>();
        }
    }
}
