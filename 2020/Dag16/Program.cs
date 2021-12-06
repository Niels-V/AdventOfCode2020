using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag16
{
    [TestCategory("2020")]
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static int First(string inputFile)
        {
            var parser = new TicketParser();
            var info = parser.ParseFile(inputFile);
            var allRules = info.Properties.Select(p => p.ValidationRule());
            var allValues = info.NearbyTickets.SelectMany(t => t.Values);
            var invalidValues = allValues.Where(value => !allRules.Any(rule => rule(value)));
            var sumOfInvalidValues = invalidValues.Sum();
            return sumOfInvalidValues;
        }

        static long Second(string inputFile)
        {
            var parser = new TicketParser();
            var info = parser.ParseFile(inputFile);
            var allRules = info.Properties.Select(p => p.ValidationRule());

            var invalidTicketsRemoved = info.NearbyTickets.RemoveAll(t => t.Values.Any(value => !allRules.Any(rule => rule(value))));

            var ticketIndexes = info.MyTicket.Values.Select((_, i) => i).ToList();

            var indexValues = ticketIndexes.Select(i => new { Index = i, Values = info.NearbyTickets.Select(t => t.Values.ElementAt(i)) });

            var foundPropsWithIndexes = info.Properties.Select(
                prop => new
                {
                    Index = indexValues.Where(iv => iv.Values.All(r => prop.ValidationRule()(r))).Select(s => s.Index).ToList(),
                    Property = prop
                }
            ).ToDictionary(x=>x.Property, y=>y.Index);
            foreach (var result in foundPropsWithIndexes)
            {
                Console.WriteLine(result.Key.Name + " found with indexes " + string.Join(",", foundPropsWithIndexes[result.Key]));
            }

            //now reduce the options
            while (foundPropsWithIndexes.Values.Any(list=>list.Count() > 1))
            {
                var propertiesWithOneIndex = foundPropsWithIndexes.Where(item => foundPropsWithIndexes[item.Key].Count() == 1);
                foreach (var item in propertiesWithOneIndex)
                {
                    item.Key.TicketIndex = foundPropsWithIndexes[item.Key].Single();
                    foundPropsWithIndexes.Remove(item.Key);
                    foreach (var indexList in foundPropsWithIndexes.Values)
                    {
                        indexList.Remove(item.Key.TicketIndex.Value);
                    }
                }
                Console.WriteLine("New iteration:");
                foreach (var result in foundPropsWithIndexes)
                {
                    Console.WriteLine(result.Key.Name + " found with indexes " + string.Join(",", foundPropsWithIndexes[result.Key]));
                }

            }

            var departureIndexes = info.Properties.Where(prop => prop.Name.StartsWith("departure")).Select(p=>p.TicketIndex.Value);
            var myDepartureValues = departureIndexes.Select(i => info.MyTicket.Values.ElementAt(i));
            var myDepartureProduct = myDepartureValues.Aggregate(1L,(acc, el)=>acc*el);
            return myDepartureProduct;
        }

        static int SecondA(string inputFile)
        {
            var parser = new TicketParser();
            var info = parser.ParseFile(inputFile);
            var allRules = info.Properties.Select(p => p.ValidationRule());

            var invalidTicketsRemoved = info.NearbyTickets.RemoveAll(t => t.Values.Any(value => !allRules.Any(rule => rule(value))));
            return invalidTicketsRemoved;
        }


        [DataTestMethod]
        [DataRow("test.txt", 71)]
        public void TestPart1(string inputFile, int expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 3)]
        public void TestPart2A(string inputFile, int expectedResult)
        {
            var result = SecondA(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 0)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
