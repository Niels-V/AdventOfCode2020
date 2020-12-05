using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dag5
{
    class Program
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);

        /// <summary>
        /// Returns the seat ID of the specified seat code
        /// As the explanation is quite complex, it the seat ID is just a bit array converted to an integer
        /// </summary>
        /// <param name="seat"></param>
        /// <returns>the seat ID</returns>
        public static int ParseSeat(string seat)
        {
            if (seat == null || seat.Length != 10) throw new ArgumentOutOfRangeException(nameof(seat));
            var seatBits = new BitArray(seat.Length);
            for (int i = 0; i< 10; i++)
            {
                seatBits[9-i] = seat[i] == 'B'||seat[i]=='R';
            }

            var result = new int[1];
            seatBits.CopyTo(result, 0);
            return result[0];
        }

        static void Main(string[] args)
        {
            Debug.Assert(357 == ParseSeat("FBFBBFFRLR"), "SeatId mismatch");
            Debug.Assert(567 == ParseSeat("BFFFBBFRRR"), "SeatId mismatch");
            Debug.Assert(119 == ParseSeat("FFFBBBFRRR"), "SeatId mismatch");
            Debug.Assert(820 == ParseSeat("BBFFBBFRLL"), "SeatId mismatch");

            var seatIds = readlines("input.txt").Select(ParseSeat).OrderBy(i=>i).ToList();

            Console.WriteLine("Highest seat id: {0}", seatIds.Last());
            
            for (int i = 1; i < seatIds.Count; i++)
            {
                //The missing seat should be the only item in the array where two adjacent existing 
                //items are two seatIds apart (as both seats left and right should exist).
                if (seatIds[i-1] +2 == seatIds[i] )
                {
                    Console.WriteLine("Found missing seat: {0}", seatIds[i - 1] + 1);
                }
            }
            Console.WriteLine("Should print only one missing seat!");
        }
    }
}
