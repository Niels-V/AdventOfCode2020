using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Common.CubeAlgebra
{
    public class Point3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

    }
    public class AABB
    {
        public Point3 Min { get; set; }
        public Point3 Max { get; set; }

        public long Volume => ((long)(Max.X - Min.X+1)) * ((long)(Max.Y - Min.Y+1)) * ((long)(Max.Z - Min.Z+1));
        public AABB()
        {
        }
        public AABB(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            Min = new Point3 { X = minX, Y = minY, Z = minZ };
            Max = new Point3 { X = maxX, Y = maxY, Z = maxZ };
            if (!IsValid) {
                throw new ArgumentException("Invalid points");
            }
        }

        public override string ToString()
        {
            string result = "Min: (" + Min.X + ", " + Min.Y + ", " + Min.Z + "), ";
            result += "Max: ( " + Max.X + ", " + Max.Y + ", " + Max.Z + ")";
            return result;
        }
        public bool IsValid
        {
            get
            {
                return Min.X <= Max.X && Min.Y <= Max.Y && Min.Z <= Max.Z;
            }
        }

        public bool IntersectWith(AABB other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }

        public bool Contains(AABB other)
        {
            return Min.X <= other.Min.X && Max.X >= other.Max.X &&
                   Min.Y <= other.Min.Y && Max.Y >= other.Max.Y &&
                   Min.Z <= other.Min.Z && Max.Z >= other.Max.Z;
        }

        public IEnumerable<AABB> Union(AABB other)
        {
            if (!IntersectWith(other))
            {
                yield return this;
                yield return other;
            }
            else if (Contains(other))
            {
                yield return this;
            }
            else if (other.Contains(this))
            {
                yield return other;
            }
            else
            {
                //determine which planes got crossed;
                var crossMaxXPlane = other.Max.X > Max.X;
                var crossMaxYPlane = other.Max.Y > Max.Y;
                var crossMaxZPlane = other.Max.Z > Max.Z;
                var crossMinXPlane = other.Min.X < Min.X;
                var crossMinYPlane = other.Min.Y < Min.Y;
                var crossMinZPlane = other.Min.Z < Min.Z;
                yield return this;
                if (crossMaxXPlane)
                {
                    yield return new AABB { Max = other.Max, Min = new Point3 { X = Max.X, Y = other.Min.Y, Z = other.Min.Z } };
                }
                if (crossMinXPlane)
                {
                    yield return new AABB { Min = other.Min, Max = new Point3 { X = Min.X, Y = other.Max.Y, Z = other.Max.Z } };
                }
                if (crossMaxYPlane)
                {
                    yield return new AABB
                    {
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = other.Max.Y, Z = other.Max.Z },
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = Max.Y, Z = other.Min.Z }
                    };
                }
                if (crossMinYPlane)
                {
                    yield return new AABB
                    {
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = other.Min.Y, Z = other.Min.Z },
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = Min.Y, Z = other.Max.Z }
                    };
                }
                if (crossMaxZPlane)
                {
                    yield return new AABB
                    {
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = Math.Min(other.Max.Y, Max.Y), Z = other.Max.Z },
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = Math.Max(other.Min.Y, Min.Y), Z = Max.Z }
                    };
                }
                if (crossMinZPlane)
                {
                    yield return new AABB
                    {
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = Math.Max(other.Min.Y, Min.Y), Z = other.Min.Z },
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = Math.Min(other.Max.Y, Max.Y), Z = Min.Z }
                    };
                }
            }
        }

        public IEnumerable<AABB> Remove(AABB other)
        {
            if (!IntersectWith(other))
            {
                yield return this;
            }
            else if (Contains(other))
            {
                yield return new AABB { Max = Max, Min = new Point3 { X = other.Max.X+1, Y = Min.Y, Z = Min.Z } };
                yield return new AABB { Min = Min, Max = new Point3 { X = other.Min.X-1, Y = Max.Y, Z = Max.Z } };
                yield return new AABB
                {
                    Max = new Point3 { X = other.Max.X, Y = Max.Y, Z = Max.Z },
                    Min = new Point3 { X = other.Min.X, Y = other.Max.Y+1, Z = Min.Z }
                };
                yield return new AABB
                {
                    Min = new Point3 { X = other.Min.X, Y = Min.Y, Z = Min.Z },
                    Max = new Point3 { X = other.Max.X, Y = other.Min.Y-1, Z = Max.Z }
                };
                yield return new AABB
                {
                    Max = new Point3 { X = Math.Min(other.Max.X,Max.X), Y = other.Max.Y, Z = Max.Z },
                    Min = new Point3 { X = other.Min.X, Y = other.Min.Y, Z = other.Max.Z+1 }
                };
                yield return new AABB
                {
                    Min = new Point3 { X = other.Min.X, Y = other.Min.Y, Z = Min.Z+1 },
                    Max = new Point3 { X = other.Max.X, Y = other.Max.Y, Z = other.Min.Z-1 }
                };
            }
            else if (other.Contains(this))
            {
                yield break;
            }
            else
            {
                var crossMaxXPlane = other.Max.X >= Max.X;
                var crossMaxYPlane = other.Max.Y >= Max.Y;
                var crossMaxZPlane = other.Max.Z >= Max.Z;
                var crossMinXPlane = other.Min.X <= Min.X;
                var crossMinYPlane = other.Min.Y <= Min.Y;
                var crossMinZPlane = other.Min.Z <= Min.Z;
                if (!crossMaxXPlane)
                {
                    yield return new AABB {Min = new Point3 { X = other.Max.X+1, Y = Min.Y, Z = Min.Z }, Max = Max };
                }
                if (!crossMinXPlane)
                {
                    yield return new AABB { Min = Min, Max = new Point3 { X = other.Min.X-1, Y = Max.Y, Z = Max.Z } };
                }
                if (!crossMaxYPlane)
                {
                    yield return new AABB
                    {
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = other.Max.Y+1, Z = Min.Z },
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = Max.Y, Z = Max.Z }
                    };
                }
                if (!crossMinYPlane)
                {
                    yield return new AABB
                    {
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = Min.Y, Z = Min.Z },
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = other.Min.Y-1, Z = Max.Z }
                    };
                }
                if (!crossMaxZPlane)
                {
                    yield return new AABB
                    {
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = Math.Max(other.Min.Y, Min.Y), Z = other.Max.Z+1 },
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = Math.Min(other.Max.Y, Max.Y), Z = Max.Z },
                    };
                }
                if (!crossMinZPlane)
                {
                    yield return new AABB
                    {
                        Min = new Point3 { X = Math.Max(other.Min.X, Min.X), Y = Math.Max(other.Min.Y, Min.Y), Z = Min.Z },
                        Max = new Point3 { X = Math.Min(other.Max.X, Max.X), Y = Math.Min(other.Max.Y, Max.Y), Z = other.Min.Z-1 }
                    };
                }
            }
        }

        public void Fix()
        {
            if (Min.X > Max.X)
            {
                var max = Max.X;
                Max.X = Min.X;
                Min.X = max;
            }
            if (Min.Y > Max.Y)
            {
                var max = Max.Y;
                Max.Y = Min.Y;
                Min.Y = max;
            }
            if (Min.Z > Max.Z)
            {
                var max = Max.Z;
                Max.Z = Min.Z;
                Min.Z = max;
            }
        }
    }
}
