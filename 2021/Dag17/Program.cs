using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2021")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First(230, 283, -107, -57);
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second(230, 283, -107, -57);
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(int xmin, int xmax, int ymin, int ymax)
        {
            var minVx = FindMinVx(xmin);


            var xRangeV = System.Linq.Enumerable.Range(minVx, xmax - minVx);
            //how faster Vy will be, how higher the probe will get, so only need to evaluate from highest speed to lowest, and above 0.
            //Why the factor 3? No clue just a hunch. Something with a parabola and 3 parts
            for (int vy = -3*ymax; vy > 0; vy--)
            {
                foreach (var vx in xRangeV)
                {
                    bool hitsBox = HitsBox(vx, vy, xmin, xmax, ymin, ymax);
                    if (hitsBox)
                    {
                        return (vy + 1) * vy / 2; 
                    }
                }
            }

            return -1;
        }

        private static bool HitsBox(int vx0, int vy0, int xmin, int xmax, int ymin, int ymax)
        {
            var step = 0;
            var x = 0;
            var y = 0;
            var vx = vx0;
            var vy = vy0;
            while (true)
            {
                step++;
                x = x + vx;
                y = y + vy;
                vx = (vx > 0) ? vx - 1 : (vx < 0) ? vx + 1 : 0;
                vy = vy - 1;

                if (x >xmax || (x<xmin && vx==0) || y<ymin )
                {
                    //miss
                    return false;
                }
                if (x>= xmin && x<=xmax && y>=ymin && y<=ymax) 
                { 
                    //hit
                    return true; 
                }
            }
            throw new ArgumentException("Unkown");
        }


        private static int FindMinVx(int xmin)
        {
            var x = xmin;
            var vx = 1;
            var step = 1;
            while (x > 0)
            {
                step++;
                x -= vx++;
            }
            return vx - 1;
        }

        static long Second(int xmin, int xmax, int ymin, int ymax)
        {
            var minVx = FindMinVx(xmin);

            var counter = 0;
            var xRangeV = System.Linq.Enumerable.Range(minVx, xmax - minVx+1);
            for (int vy = -3 * ymax; vy > 3 * ymax; vy--)
            {
                foreach (var vx in xRangeV)
                {
                    bool hitsBox = HitsBox(vx, vy, xmin, xmax, ymin, ymax);
                    if (hitsBox)
                    {
                        counter++;
                        continue;
                    }
                }
            }

            return counter;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First(20, 30, -10, -5);
            Assert.AreEqual(45, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second(20, 30, -10, -5);
            Assert.AreEqual(112, result);
        }
    }
}
