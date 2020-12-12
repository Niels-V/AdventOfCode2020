using Common;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dag12
{
    public enum OperationType
    {
        [EnumMember(Value = "F")]
        Forward,
        [EnumMember(Value = "N")]
        Nord,
        [EnumMember(Value = "S")]
        South,
        [EnumMember(Value = "E")]
        East,
        [EnumMember(Value = "W")]
        West,
        [EnumMember(Value = "R")]
        Right,
        [EnumMember(Value = "L")]
        Left
    }

    public class Instruction
    {
        public OperationType Operation { get; set; }
        public int Argument { get; set; }
    }
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

    }

    public class BoathWithWaypoint
    {
        public BoathWithWaypoint()
        {
            BoatPosition = new Position(0, 0);
            WaypointPosition = new Position(10, -1);
        }
        public Position BoatPosition { get; set; }
        public Position WaypointPosition { get; set; }
        public void TranslateWaypoint(Position m) => WaypointPosition += m;
        public void TranslateBoat(Position m) => BoatPosition += m;

        public void RotateWaypoint(int degrees)
        {
            var waypointDistance = Math.Sqrt(WaypointPosition.X * WaypointPosition.X + WaypointPosition.Y * WaypointPosition.Y);
            var currentAngleInRadians = Math.Atan2(WaypointPosition.Y, WaypointPosition.X);
            var newX = Convert.ToInt32(waypointDistance * Math.Cos(degrees.ToRadians() + currentAngleInRadians));
            var newY = Convert.ToInt32(waypointDistance * Math.Sin(degrees.ToRadians() + currentAngleInRadians));
            WaypointPosition = new Position(newX, newY);
        }

        public void Move(int distance)
        {
            int x = distance * WaypointPosition.X;
            int y = distance * WaypointPosition.Y;
            Position translation = new Position(x, y);
            TranslateBoat(translation);
        }

        public int ManhattanDistance => Math.Abs(BoatPosition.X) + Math.Abs(BoatPosition.Y);
    }
    
    public class Boat
    {
        public Boat()
        {
            Orientation = 0; //Is East direction, IE x=1,y=0
            Position = new Position(0, 0);
        }
        public Position Position { get; set; }
        public int Orientation { get; set; }
        public void Translate(Position m) => Position += m;

        public void Rotate(int degrees)
        {
            Orientation += degrees;
            if (Orientation >= 360) { Orientation -= 360; }
            if (Orientation < 0) { Orientation += 360; }
        }

        public void Move(int distance)
        {
            int x = distance * Convert.ToInt32(Math.Cos(Orientation.ToRadians()));
            int y = distance * Convert.ToInt32(Math.Sin(Orientation.ToRadians()));
            Position translation = new Position(x, y);
            Translate(translation);
        }

        public int ManhattanDistance => Math.Abs(Position.X) + Math.Abs(Position.Y);
    }
}
