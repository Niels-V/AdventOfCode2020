using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2023")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            //Time:        48     93     84     66
            //Distance:   261   1192   1019   1063
            var result = First(new List<RaceSolver>{
                new RaceSolver(48,261),
                new RaceSolver(93,1192),
                new RaceSolver(84,1019),
                new RaceSolver(66,1063)
            });
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second(new RaceSolver(48938466, 261119210191063));
            Console.WriteLine("Program succes, found result: {0}", result2);
        }

        static long First(List<RaceSolver> input)
        {
            return input.Aggregate(1L,(acc, el)=>acc*el.NumberOfWinningTimes);
        }

        static long Second(RaceSolver input)
        {
            return input.NumberOfWinningTimes;
        }

        [TestMethod]
        public void TestPart1()
        {
            //Time:      7  15   30
            //Distance: 9  40  200
            var result = First(new List<RaceSolver>{
                new RaceSolver(7,9),
                new RaceSolver(15,40),
                new RaceSolver(30,200)
            });
            Assert.AreEqual(288, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second(new RaceSolver(71530, 940200));
            Assert.AreEqual(71503, result);
        }

        public class RaceSolver {
            public RaceSolver(long time, long distance)
            {
                Racetime=time;
                MinDistance=distance;
            }
            public long Racetime {get;set;}
            public long MinDistance {get;set;}

            public double Determinant => Math.Sqrt(Math.Pow(Racetime,2)-4*MinDistance) ;
            public long MinButtontime => Convert.ToInt32(Math.Ceiling((Racetime-Determinant)/2) < 0 ? 0:Math.Floor((Racetime-Determinant)/2)+1);
            public long MaxButtontime => Convert.ToInt32(Math.Floor((Racetime+Determinant)/2> Racetime ? Racetime : Math.Ceiling((Racetime+Determinant)/2)-1));
            public long NumberOfWinningTimes => MaxButtontime-MinButtontime+1;
        }
    }
}
