using System.Linq;

namespace Common
{
    public class ExtendedMath
    {
        /// <summary>
        /// Greatest Common Divisor
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static long GreatestCommonDivisor(long n1, long n2)
        {
            if (n2 == 0)
            {
                return n1;
            }
            else
            {
                return GreatestCommonDivisor(n2, n1 % n2);
            }
        }

        public static long LeastCommonMultiple(long[] numbers)
        {
            return numbers.Aggregate((S, val) => S * val / GreatestCommonDivisor(S, val));
        }
    }
}