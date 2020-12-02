using System;
using System.Linq;
using System.Collections.Generic;

namespace Dag2
{
    class Program
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        static IEnumerable<PasswordPolicy> readPasswords(string filePath) => readlines(filePath).Select(convertInputLine);
        static PasswordPolicy convertInputLine(string s)
        {
            var firstSplit = s.Split(": ");
            var policyPassword = firstSplit.Last();
            var policy = firstSplit.First();
            var charReq = policy.Last();
            var range = policy.Substring(0, policy.Length - 2);
            var min = Convert.ToInt32(range.Split("-").First());
            var max = Convert.ToInt32(range.Split("-").Last());
            return new PasswordPolicy { Password = policyPassword, RequiredChar = charReq, RequiredMin = min, RequiredMax = max };
        }
        static void Main(string[] args)
        {
            var list = readPasswords("input.txt").ToList();
            var validCount = list.AsParallel().Count(p => p.IsValid());
            var valid2Count = list.AsParallel().Count(p => p.IsValidToo());
            Console.WriteLine("{0} / {1}",validCount, valid2Count);
        }
    }

    struct PasswordPolicy
    {
        public string Password { get; set; }
        public char RequiredChar { get; set; }
        public int RequiredMin { get; set; }
        public int RequiredMax { get; set; }

        public bool IsValid()
        {
            var requiredChar = RequiredChar;
            var requiredCount = Password.Count(c=> c== requiredChar);
            return requiredCount >= RequiredMin && requiredCount <= RequiredMax;
        }
        public bool IsValidToo()
        {
            var pos1Check = Password[RequiredMin-1] == RequiredChar;
            var pos2Check = Password[RequiredMax-1] == RequiredChar;
            return pos1Check ^ pos2Check;
        }
    }
}
