using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// Converts a inputfile containing player decks per character a meaning into a array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DeckParser<T>
    {
        protected IEnumerable<string> Readlines(string filePath) => System.IO.File.ReadLines(filePath);

        public virtual IList<IEnumerable<T>> ReadDecks(string filePath)
        {
            List<IEnumerable<T>> decks = new List<IEnumerable<T>>();
            var lines = Readlines(filePath).ToList();
            List<T> deck = new List<T>();
            foreach (var line in lines)
            {
                if (line.StartsWith("Player "))
                {
                    decks.Add(deck);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(line)) {
                    deck = new List<T>();
                    continue; 
                }
                deck.Add(Convert(line));
            }
            return decks;
        }
        protected abstract T Convert(string line);
    }

    public class IntDeckParser : DeckParser<int>
    {
        protected override int Convert(string line) => System.Convert.ToInt32(line);
    }
}
