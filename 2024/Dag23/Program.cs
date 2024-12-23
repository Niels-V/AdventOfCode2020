using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace AoC
{

    [TestCategory("2024")]
    [TestClass]
    public partial class Program
    {
        static void Main(string[] args)
        {
            var result = First("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);
        }


        static long First(string inputFile)
        {
            var result = -1;
            Dictionary<string, List<Connection>> lookup = LoadNetwork(inputFile);

            var firstNodes = lookup.Keys.Where(k => k.StartsWith('t')).ToList();
            var threeNodesList = new Dictionary<string, Tuple<Connection, Connection, Connection>>();
            foreach (var firstNode in firstNodes)
            {
                foreach (var connection in lookup[firstNode])
                {
                    var secondNode = connection.Node1 == firstNode ? connection.Node2 : connection.Node1;
                    foreach (var secondConnection in lookup[secondNode])
                    {
                        var thirdNode = secondConnection.Node1 == secondNode ? secondConnection.Node2 : secondConnection.Node1;
                        var thirdConnection = thirdNode != firstNode ? lookup[thirdNode].FirstOrDefault(n => n.Node1 == firstNode || n.Node2 == firstNode) : null;
                        if (thirdConnection != null)
                        {
                            var key = string.Join(',', (new[] { firstNode, secondNode, thirdNode }).Order());
                            if (!threeNodesList.ContainsKey(key))
                            {
                                threeNodesList.Add(key, new Tuple<Connection, Connection, Connection>(connection, secondConnection, thirdConnection));
                            }
                        }
                    }
                }
            }
            //normalize

            result = threeNodesList.Count;

            return result;
        }

        private static Dictionary<string, List<Connection>> LoadNetwork(string inputFile)
        {
            var parser = new NetworkParser();
            var connections = parser.ReadData(inputFile).ToList();
            Dictionary<string, List<Connection>> lookup = new Dictionary<string, List<Connection>>();
            foreach (var connection in connections)
            {
                if (lookup.ContainsKey(connection.Node1))
                {
                    lookup[connection.Node1].Add(connection);
                }
                else
                {
                    lookup[connection.Node1] = [connection];
                }
                if (lookup.ContainsKey(connection.Node2))
                {
                    lookup[connection.Node2].Add(connection);
                }
                else
                {
                    lookup[connection.Node2] = [connection];
                }
            }

            return lookup;
        }

        static string Second(string inputFile)
        {
            var result = string.Empty;
            Dictionary<string, List<Connection>> lookup = LoadNetwork(inputFile);

            var firstNodes = lookup.Keys;
            var foundNodesList = new HashSet<string>();
            //Not the most efficient solution: determine all triangles/
            foreach (var firstNode in firstNodes)
            {
                foreach (var connection in lookup[firstNode])
                {
                    var secondNode = connection.Node1 == firstNode ? connection.Node2 : connection.Node1;
                    foreach (var secondConnection in lookup[secondNode])
                    {
                        var thirdNode = secondConnection.Node1 == secondNode ? secondConnection.Node2 : secondConnection.Node1;
                        var thirdConnection = thirdNode != firstNode ? lookup[thirdNode].FirstOrDefault(n => n.Node1 == firstNode || n.Node2 == firstNode) : null;
                        if (thirdConnection != null)
                        {
                            var key = string.Join(',', (new[] { firstNode, secondNode, thirdNode }).Order());
                            foundNodesList.Add(key);
                        }
                    }
                }
            }
            //Now take each found subnetwork of size N and find a node that connects to each node in the subnetwork.
            //the hashset dedups the found networks of size N+1.
            var newList  = foundNodesList;
            HashSet<string> oldList = null;
            do {
                oldList = newList;
                newList = [];
                foreach (var connection in oldList)
                {
                    foreach (var nodeKey in lookup.Keys.Where(k => !connection.Contains(k)))
                    {
                        var connectionsFromNewNode = lookup[nodeKey];
                        var currentNodes = connection.Split(',');
                        if (currentNodes.All(nc => connectionsFromNewNode.Any(c => c.Node1 == nc || c.Node2 == nc)))
                        {
                            var key = string.Join(',', currentNodes.Append(nodeKey).Order());
                            newList.Add(key);
                        }
                    }
                }
            }
            while (newList.Count > 0);
            result = oldList.First();
            return result;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual("co,de,ka,ta", result);
        }
    }
    public class Connection
    {
        public string Node1 { get; set; }
        public string Node2 { get; set; }
    }

    public class NetworkParser : Parser<Connection>
    {
        protected override Connection ParseLine(string line)
        {
            var items = line.Split('-', StringSplitOptions.RemoveEmptyEntries);
            return new Connection { Node1 = items[0], Node2 = items[1] };
        }
    }
}
