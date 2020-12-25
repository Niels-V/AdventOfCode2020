using System;
using System.Linq;
using System.Collections.Generic;
using Common;

namespace Common
{
    public abstract class Parser<T>
    {
        protected IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);
        public virtual IEnumerable<T> ReadData(string filePath)
        {
            return Readlines(filePath).Select(ParseLine);
        }

        protected abstract T ParseLine(string line);
    }

    public class IntParser : Parser<int>
    {
        protected override int ParseLine(string line)
        {
            return Convert.ToInt32(line);
        }
    }
    public class LongParser : Parser<long>
    {
        protected override long ParseLine(string line)
        {
            return Convert.ToInt64(line);
        }
    }

    public class LineParser : Parser<string>
    {
        protected override string ParseLine(string line)
        {
            return line;
        }
    }
}
