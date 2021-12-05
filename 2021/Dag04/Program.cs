using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dag04
{
    public class NumberDrawedEventArgs : EventArgs
    {
        public NumberDrawedEventArgs(int number)
        {
            Number = number;
        }
        public int Number { get; set; }
    }
    public enum GameMode { FirstWins, LastWins }
    public class Bingo
    {
        public Bingo()
        {
            DrawnNumbers = new();
            Boards = new();
        }
        public event EventHandler<NumberDrawedEventArgs> RaiseNumberDrawedEvent;
        public List<int> DrawnNumbers { get; private set; }
        public Board WinningBoard { get; private set; }
        public List<Board> Boards { get; private set; }

        public int BoardSize { get; set; }

        private int BoardsWithBingo = 0;
        private HashSet<int> bingoBoards = new HashSet<int>();

        public void Play(GameMode mode)
        {
            var boardCount = Boards.Count();
            for (int turn = 0; turn < DrawnNumbers.Count; turn++)
            {
                var goOn = (mode == GameMode.FirstWins && BoardsWithBingo == 0) || mode == GameMode.LastWins && BoardsWithBingo < boardCount;
                if (goOn)
                {
                    CallNumber(DrawnNumbers[turn]);
                }
                else
                {
                    return;
                }
            }
        }

        internal void CallBingo(Board winningBoard)
        {
            if (!bingoBoards.Contains(winningBoard.BoardNumber))
            {
                bingoBoards.Add(winningBoard.BoardNumber);
                BoardsWithBingo++;
                WinningBoard = winningBoard;
            }
            Console.WriteLine(winningBoard.BoardNumber);
        }

        private void CallNumber(int number)
        {
            OnNumberDrawedEvent(new NumberDrawedEventArgs(number));
        }

        protected virtual void OnNumberDrawedEvent(NumberDrawedEventArgs e)
        {
            RaiseNumberDrawedEvent?.Invoke(this, e);
        }
    }

    public class Board
    {
        private int LastCalledNumber { get; set; }
        private List<Row> Rows { get; set; }
        private List<Column> Columns { get; set; }

        private Dictionary<int, Cell> Cells { get; set; }

        public Bingo Game { get; private set; }
        public int BoardNumber { get; internal set; }
        public long Score => Cells.Values.Where(c => !c.Marked).Sum(c => (long)c.Number) * LastCalledNumber;
        int fillIndexX = 0;
        int fillIndexY = 0;

        public Board(Bingo game)
        {
            game.RaiseNumberDrawedEvent += Game_RaiseNumberDrawedEvent;
            Game = game;
            Rows = new List<Row>(game.BoardSize);
            Columns = new List<Column>(game.BoardSize);
            for (int i = 0; i < game.BoardSize; i++)
            {
                Rows.Add(new(game, this));
                Columns.Add(new(game, this));
            }
            Cells = new Dictionary<int, Cell>(game.BoardSize * game.BoardSize);
        }

        private void Game_RaiseNumberDrawedEvent(object sender, NumberDrawedEventArgs e)
        {
            MarkNumber(e.Number);
        }

        public void AddCell(int value)
        {
            var column = Columns[fillIndexX];
            var row = Rows[fillIndexY];
            var cell = new Cell { Board = this, Column = column, Row = row, Number = value };
            column.Cells.Add(cell);
            row.Cells.Add(cell);
            Cells.Add(value, cell);
            fillIndexX++;
            if (fillIndexX == Game.BoardSize)
            {
                fillIndexX = 0;
                fillIndexY++;
            }
        }

        private void MarkNumber(int number)
        {
            LastCalledNumber = number;
            if (!Cells.ContainsKey(number)) { return; }
            var cell = Cells[number];
            if (cell.Mark())
            {
                Game.CallBingo(this);
            }
        }
    }

    public class Column
    {
        public Column(Bingo game, Board board)
        {
            Cells = new List<Cell>(game.BoardSize);
            Board = board;
        }
        public Board Board { get; set; }
        public int MarkedCount { get; set; }
        public List<Cell> Cells { get; set; }
    }

    public class Cell
    {
        public int Number { get; set; }
        public bool Marked { get; set; }
        public Column Column { get; set; }
        public Row Row { get; set; }
        public Board Board { get; set; }

        internal bool Mark()
        {
            Marked = true;
            Row.MarkedCount++;
            Column.MarkedCount++;
            return Row.MarkedCount == Board.Game.BoardSize || Column.MarkedCount == Board.Game.BoardSize;
        }
    }
    public class Row
    {
        public Row(Bingo game, Board board)
        {
            Cells = new List<Cell>(game.BoardSize);
            Board = board;
        }
        public Board Board { get; set; }
        public List<Cell> Cells { get; set; }
        public int MarkedCount { get; set; }
    }

    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static long First(string inputFile)
        {
            var game = new Parser().ParseFile(inputFile);
            game.Play(GameMode.FirstWins);
            return game.WinningBoard.Score;
        }

        static long Second(string inputFile)
        {
            var game = new Parser().ParseFile(inputFile);
            game.Play(GameMode.LastWins);
            return game.WinningBoard.Score;
        }



        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(4512, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(1924, result);
        }
        [TestMethod]
        public void TestPart2b()
        {
            var result = Second("input2.txt");
            Assert.AreEqual(3074334900, result);
        }
    }
}
