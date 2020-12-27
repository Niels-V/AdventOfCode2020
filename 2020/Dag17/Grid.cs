using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Dag17
{

    public struct Position
    {
        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public static Position operator +(Position a) => a;
        public static Position operator -(Position a) => new Position(-a.X, -a.Y, -a.Z);
        public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Position operator -(Position a, Position b) => a + (-b);

        public override string ToString() => $"[{X},{Y},{Z}]";

        public IEnumerable<Position> Neighbours()
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) { continue; }
                        yield return this + new Position(i, j, k);
                    }
                }
            }
        }
    }
    public class Cube
    {
        public Dictionary<string, Cube> Cubes{ get; }
        public Dictionary<int, bool> IsActive { get; private set; }
        public Position Position { get; set; }

        public Cube(Dictionary<string, Cube> cubes)
        {
            Cubes = cubes;
            IsActive = new Dictionary<int, bool>();
        }
        public IEnumerable<Cube> Neighbours() => Position.Neighbours().Select(n => Cubes.ContainsKey(n.ToString()) ? Cubes[n.ToString()] : null);
        public Cube Neighbour(Position position) => Cubes.ContainsKey(position.ToString()) ? Cubes[position.ToString()] : null;

        public bool IsActiveInTurn(int turnId) => IsActive.ContainsKey(turnId) && IsActive[turnId];

        public bool CalculateNextTurn(int turn)
        {
            var activeNeighbours = Neighbours().Count(neighbour => neighbour != null && neighbour.IsActiveInTurn(turn - 1));

            var isActive = IsActiveInTurn(turn - 1);
            //If a cube is active and exactly 2 or 3 of its neighbors are also active, the cube remains active. Otherwise, the cube becomes inactive.
            //If a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active. Otherwise, the cube remains inactive.
            if (isActive && (activeNeighbours == 2 || activeNeighbours ==3))
            {
                IsActive.Add(turn, true);
                return true;
            }
            if (!isActive && activeNeighbours == 3)
            {
                IsActive.Add(turn, true);
                return true;
            }
            IsActive.Add(turn, false);
            return false;
        }
    }

    public class CubeGameOfLife
    {
        public CubeGameOfLife()
        {
            Cubes = new Dictionary<string, Cube>();
        }
        public Dictionary<string, Cube> Cubes{ get; }

        public void DoTurn(int turn)
        {
            var keys = Cubes.Keys.ToArray();
            foreach (var cubePosition in keys)
            {
                var currentCube = Cubes[cubePosition];
                var nextTurnActive = currentCube.CalculateNextTurn(turn);
                if (nextTurnActive)
                {
                    AddNeighbours(currentCube);
                }
            }
        }

        private void AddNeighbours(Cube currentCube)
        {
            foreach (var neighbourPosition in currentCube.Position.Neighbours())
            {
                var neighbour = currentCube.Neighbour(neighbourPosition);
                if (neighbour == null)
                {
                    var newCube = new Cube(Cubes) { Position = neighbourPosition };
                    Cubes.Add(newCube.Position.ToString(), newCube);
                }
            }
        }

        public void FillStart(IEnumerable<Position> activeCubes)
        {
            foreach (var position in activeCubes)
            {
                var cube = new Cube(Cubes)
                {
                    Position = position
                };
                cube.IsActive.Add(0, true);
                Cubes.Add(position.ToString(), cube);
            }
            var keys = Cubes.Keys.ToArray();
            foreach (var CubePosition in keys)
            {
                var currentCube = Cubes[CubePosition];
                AddNeighbours(currentCube);
            }
        }
    }
}
