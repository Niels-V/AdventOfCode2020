using System;
using System.Linq;
using System.Diagnostics;

namespace Dag4
{
    class Program
    {
        static void Main(string[] args)
        {
            var passports = PassportParser.ReadPassports("test.txt").ToList();
            Debug.Assert(4 == passports.Count);
            Debug.Assert(2 == passports.Count(p => p.IsValid()));

            var passports2 = PassportParser.ReadPassports("input.txt").ToList();
            var validNumber = passports2.Count(p => p.IsValid());
            Console.WriteLine("Valid for first part: {0}", validNumber);

            var passportsTest = PassportParser.ReadPassports("test2.txt").ToList();
            Debug.Assert(8 == passportsTest.Count);
            Debug.Assert(4 == passportsTest.Count(p => p.IsValid2()), "Not valid passports mismatch");
            Debug.Assert(0 == passportsTest.Take(4).Count(p => p.IsValid2()), "Not valid passports mismatch");

            var validNumber2 = passports2.Count(p => p.IsValid2());
            Console.WriteLine("Valid for second part: {0}", validNumber2);
        }
    }
}
