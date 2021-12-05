using Common;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag04
{
    public class Parser : LineParser
    {
        
        public Bingo ParseFile(string filePath)
        {
            var game = new Bingo();
            var mode = 0;
            var lastMode = mode;
            Board board = null;
            foreach (var line in ReadData(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    mode++;
                    continue;
                }
                if (mode == 0)
                {
                    var numbers = line.Split(',').Select(s => Convert.ToInt32(s));
                    game.DrawnNumbers.AddRange(numbers);
                    continue;
                }
                var cellnumbers = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt32(s));
                if (mode == 1) { game.BoardSize = cellnumbers.Count(); }
                if (mode != lastMode)
                {
                    board = new Board(game) { BoardNumber = lastMode };
                    game.Boards.Add(board);
                    lastMode = mode;
                }
                foreach (var n in cellnumbers) { board.AddCell(n); }
            }
            return game;
        }
    }
}
