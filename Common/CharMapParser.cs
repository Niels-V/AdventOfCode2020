using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// Converts a inputfile containing per character a meaning into a array/matric of the representation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CharMapParser<T> 
    {
        protected IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);

        public virtual T[,] ReadMap(string filePath)
        {
            var lines = Readlines(filePath).ToList();
            var firstLine = lines.First();
            var m = firstLine.Length;
            var n = lines.Count();
            var result = new T[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[i, j] = Convert(lines[i][j]);
                }
            }
            return result;
        }
        protected abstract T Convert(char input);
    }
}
