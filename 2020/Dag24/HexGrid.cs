using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dag24
{
    public struct Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public static Position operator +(Position a) => a;
        public static Position operator -(Position a) => new Position(-a.X, -a.Y);
        public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y);
        public static Position operator -(Position a, Position b) => a + (-b);

        public override string ToString() => $"[{X},{Y}]";

        public Position East => this + new Position(2, 0);
        public Position West => this + new Position(-2, 0);
        public Position SouthWest => this + new Position(-1, -1);
        public Position NorthEast => this + new Position(1, 1);
        public Position SouthEast => this + new Position(1, -1);
        public Position NorthWest => this + new Position(-1, 1);
    }
    public class HexTile
    {
        public Dictionary<string, HexTile> HexTiles { get; }

        public HexTile(Dictionary<string, HexTile> hexTiles)
        {
            HexTiles = hexTiles;
            IsBlack = new Dictionary<int, bool>();
        }
        public Position Position { get; set; }
        public HexTile EastNeighbour => HexTiles.ContainsKey(Position.East.ToString()) ? HexTiles[Position.East.ToString()] : null;
        public HexTile WestNeighbour => HexTiles.ContainsKey(Position.West.ToString()) ? HexTiles[Position.West.ToString()] : null;
        public HexTile SouthEastNeighbour => HexTiles.ContainsKey(Position.SouthEast.ToString()) ? HexTiles[Position.SouthEast.ToString()] : null;
        public HexTile NorthWestNeighbour => HexTiles.ContainsKey(Position.NorthWest.ToString()) ? HexTiles[Position.NorthWest.ToString()] : null;
        public HexTile SouthWestNeighbour => HexTiles.ContainsKey(Position.SouthWest.ToString()) ? HexTiles[Position.SouthWest.ToString()] : null;
        public HexTile NothEastNeighbour => HexTiles.ContainsKey(Position.NorthEast.ToString()) ? HexTiles[Position.NorthEast.ToString()] : null;
        public Dictionary<int,bool> IsBlack { get; private set; }

        public bool IsBlackInTurn(int turnId) => IsBlack.ContainsKey(turnId) && IsBlack[turnId];
        /// <summary>
        /// 
        /// </summary>
        /// <param name="turn"></param>
        /// <returns>true when the new tile is black, false otherwise</returns>
        public bool CalculateNextTurn(int turn)
        {
            var blackNeighbours = 0;
            if (EastNeighbour!= null && EastNeighbour.IsBlackInTurn(turn-1)) { blackNeighbours++; }
            if (WestNeighbour != null && WestNeighbour.IsBlackInTurn(turn-1)) { blackNeighbours++; }
            if (SouthEastNeighbour != null && SouthEastNeighbour.IsBlackInTurn(turn-1)) { blackNeighbours++; }
            if (NorthWestNeighbour != null && NorthWestNeighbour.IsBlackInTurn(turn-1)) { blackNeighbours++; }
            if (SouthWestNeighbour != null && SouthWestNeighbour.IsBlackInTurn(turn-1)) { blackNeighbours++; }
            if (NothEastNeighbour != null && NothEastNeighbour.IsBlackInTurn(turn-1)) { blackNeighbours++; }

            var isBlack = IsBlack.ContainsKey(turn-1) ? IsBlack[turn-1] : false;
            //Any black tile with zero or more than 2 black tiles immediately adjacent to it is flipped to white.
            //Any white tile with exactly 2 black tiles immediately adjacent to it is flipped to black.
            if (isBlack && (blackNeighbours ==0 || blackNeighbours >2 )) {
                IsBlack.Add(turn, false);
                return false;
            }
            if (!isBlack && blackNeighbours == 2) {
                IsBlack.Add(turn, true);
                return true;
            }
            IsBlack.Add(turn, isBlack);
            return isBlack;
        }
    }

    public class HexGameOfLife
    {
        public HexGameOfLife()
        {
            HexTiles = new Dictionary<string, HexTile>();
        }
        public Dictionary<string,HexTile> HexTiles { get; }
        public void DoTurn(int turn)
        {
            var keys = HexTiles.Keys.ToArray();
            foreach (var tilePosition in keys)
            {
                var currentTile = HexTiles[tilePosition];
                var nextTurnColorBlack = currentTile.CalculateNextTurn(turn);
                if (nextTurnColorBlack)
                {
                    AddNeighbours(currentTile);
                }
            }
        }

        private void AddNeighbours(HexTile currentTile)
        {
            //check if neighbours exist
            if (currentTile.EastNeighbour == null)
            {
                var newTile = new HexTile(HexTiles) { Position = currentTile.Position.East };
                HexTiles.Add(newTile.Position.ToString(), newTile);
            }
            if (currentTile.WestNeighbour == null)
            {
                var newTile = new HexTile(HexTiles) { Position = currentTile.Position.West };
                HexTiles.Add(newTile.Position.ToString(), newTile);
            }
            if (currentTile.NothEastNeighbour == null)
            {
                var newTile = new HexTile(HexTiles) { Position = currentTile.Position.NorthEast };
                HexTiles.Add(newTile.Position.ToString(), newTile);
            }
            if (currentTile.SouthWestNeighbour == null)
            {
                var newTile = new HexTile(HexTiles) { Position = currentTile.Position.SouthWest };
                HexTiles.Add(newTile.Position.ToString(), newTile);
            }
            if (currentTile.NorthWestNeighbour == null)
            {
                var newTile = new HexTile(HexTiles) { Position = currentTile.Position.NorthWest };
                HexTiles.Add(newTile.Position.ToString(), newTile);
            }
            if (currentTile.SouthEastNeighbour == null)
            {
                var newTile = new HexTile(HexTiles) { Position = currentTile.Position.SouthEast };
                HexTiles.Add(newTile.Position.ToString(), newTile);
            }
        }

        public void FillStart(IEnumerable<Position> blackTiles)
        {
            foreach (var position in blackTiles)
            {
                var hextTile = new HexTile(HexTiles)
                {
                    Position = position
                };
                hextTile.IsBlack.Add(0, true);
                HexTiles.Add(position.ToString(), hextTile);
            }
            var keys = HexTiles.Keys.ToArray();
            foreach (var tilePosition in keys)
            {
                var currentTile = HexTiles[tilePosition];
                AddNeighbours(currentTile);
            }
        }
    }

    [TestClass]
    public class HexGrid
    {
        //The hex grid can be used to navigate from the center of the tiles. Each tile can be identified with the coordinates of the center. 
        //Adjacent tiles east-west are two apart in X direction, 0 apart in Y direction.
        //Adjacent tiles NW-SE and NE-SW are one apart in X direction and 1 apart in Y direction.
        // So moving E = [+2,0] AND NE-SE is [+1,+1] + [+1,-1] = [+2,0]
        //Possible directions: e, se, sw, w, nw, and ne
        public static Regex SouthEast = new Regex("se");
        public static Regex SouthWest = new Regex("sw");
        public static Regex NorthEast = new Regex("ne");
        public static Regex NorthWest = new Regex("nw");
        public static Regex East = new Regex("e");
        public static Regex West = new Regex("w");
        
        public static Position Walk(string directions, Position startpoint)
        {
            var movement = Walk(directions);
            return startpoint + movement;
        }
        public static Position Walk(string directions)
        {
            var seCount = SouthEast.Matches(directions).Count;
            directions = SouthEast.Replace(directions, string.Empty);
            var swCount = SouthWest.Matches(directions).Count;
            directions = SouthWest.Replace(directions, string.Empty);
            var neCount = NorthEast.Matches(directions).Count;
            directions = NorthEast.Replace(directions, string.Empty);
            var nwCount = NorthWest.Matches(directions).Count;
            directions = NorthWest.Replace(directions, string.Empty);
            var wCount = West.Matches(directions).Count;
            var eCount = East.Matches(directions).Count;

            var xDirection = eCount * 2 - (wCount * 2) + neCount + seCount - nwCount - swCount;
            var yDirection = neCount + nwCount - seCount - swCount;
            var movement = new Position(xDirection, yDirection);
            return movement;
        }

        [DataTestMethod]
        [DataRow("swwswwsww", 3)]
        [DataRow("wswwswwsw", 3)]
        [DataRow("nwwswwswwsw", 3)]
        [DataRow("wwwnwwswwswwsw", 6)]
        [DataRow("eeenesweneenwwwwnwwswwswwsw", 6)]
        public void TestWalk(string input, int expectedResult)
        {
            var result = West.Matches(input).Count;
            Assert.AreEqual(expectedResult, result);
        }
    }
}
