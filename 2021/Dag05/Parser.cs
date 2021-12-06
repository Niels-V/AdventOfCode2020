using Common;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC
{
    public class Parser : Parser<Line>
    {
        static readonly Regex instructionRule = new("^(?<startX>\\d+),(?<startY>\\d+) -> (?<endX>\\d+),(?<endY>\\d+)$", RegexOptions.Compiled);

        protected override Line ParseLine(string line)
        {
            var regexResult = instructionRule.Match(line);

            var lineStruct = new Line
            {
                Start = new Point { X= Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "startX").Value), Y = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "startY").Value) },
                End = new Point { X = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "endX").Value), Y = Convert.ToInt32(regexResult.Groups.Values.First(g => g.Name == "endY").Value) },
            };
            return lineStruct;
        }
    }
}
