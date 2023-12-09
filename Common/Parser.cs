using System;
using System.Linq;
using System.Collections.Generic;
using Common;

namespace Common
{
    public abstract class Parser<T>
    {
        protected IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);
        public virtual IEnumerable<T> ReadData(string filePath) => Readlines(filePath).Select(ParseLine);

        protected abstract T ParseLine(string line);
    }
    /// <summary>
    /// Parses the first line as CSV integers.
    /// </summary>
    public class IntCsvLineParser : Parser<int>
    {
        public override IEnumerable<int> ReadData(string filePath) => Readlines(filePath).First().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(ParseLine);
        protected override int ParseLine(string line) => Convert.ToInt32(line);
    }
    public class IntSeriesParser : Parser<IEnumerable<int>>
    {
        private char _numberSeperator;
        public IntSeriesParser(char numberSeparator)
        {
            _numberSeperator = numberSeparator;
        }
        protected override IEnumerable<int> ParseLine(string line) => line.Split(_numberSeperator, StringSplitOptions.RemoveEmptyEntries).Select(number=> Convert.ToInt32(number));
    }

    public class IntParser : Parser<int>
    {
        protected override int ParseLine(string line) => Convert.ToInt32(line);
    }
    public class NullIntParser : Parser<int?>
    {
        protected override int? ParseLine(string line) => string.IsNullOrWhiteSpace(line) ? new int?() : new int?(Convert.ToInt32(line));
    }
    public class LongParser : Parser<long>
    {
        protected override long ParseLine(string line) => Convert.ToInt64(line);
    }

    public class LineParser : Parser<string>
    {
        protected override string ParseLine(string line) => line;
    }
    
}
