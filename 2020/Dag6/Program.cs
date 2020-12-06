using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dag6
{
    class Program
    {
        static void Main(string[] args)
        {
            var testData = Parser.ReadData("test.txt");
            int sumOfTestCounts = FindAnyUniqueCounts(testData);
            Debug.Assert(11 == sumOfTestCounts, "Count mismatch");

            var inputData = Parser.ReadData("input.txt");
            int sumOfCounts = FindAnyUniqueCounts(inputData);
            Console.WriteLine("Sum of different answer counts: {0}", sumOfCounts);

            int sumOfTestCounts2 = FindAllPositiveCounts(testData);
            Debug.Assert(6 == sumOfTestCounts2, "Count mismatch");

            int sumOfCounts2 = FindAllPositiveCounts(inputData);
            Console.WriteLine("Sum of unique answer counts: {0}", sumOfCounts2);

        }
        static int FindAnyUniqueCounts(IEnumerable<AnswerForm> formData)
        {
            var groupedCounts = from f in formData
                                group f by f.GroupId into groupedForms
                                select new
                                {
                                    GroupId = groupedForms.Key,
                                    JoinedAnswers = string.Join(null, groupedForms.Select(s => s.PositiveAnswers)).Distinct(),
                                    Count = string.Join(null, groupedForms.Select(s => s.PositiveAnswers)).Distinct().Count()
                                };
            var sumOfCounts = groupedCounts.Sum(g => g.Count);
            return sumOfCounts;
        }

        static int FindAllPositiveCounts(IEnumerable<AnswerForm> formData)
        {
            var groupedCounts = from f in formData
                                group f by f.GroupId into groupedForms
                                select new
                                {
                                    GroupId = groupedForms.Key,
                                    CommonAnswers = groupedForms.Select(s => s.PositiveAnswers.ToArray()).Aggregate((s1,s2)=>Enumerable.Intersect(s1,s2).ToArray()),
                                    Count = groupedForms.Select(s => s.PositiveAnswers.ToArray()).Aggregate((s1, s2) => Enumerable.Intersect(s1, s2).ToArray()).Count()
                                };
            var sumOfCounts = groupedCounts.Sum(g => g.Count);
            return sumOfCounts;
        }

    }

}
