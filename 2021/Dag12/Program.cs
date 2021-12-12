using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    public class Cave
    {
        public Cave()
        {
            LinkedCaves = new List<Cave>();
        }
        public bool IsLarge { get; set; }
        public string Name { get; set; }
        public List<Cave> LinkedCaves { get; private set; }
    }

    [TestCategory("2021")]
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

        static long First(string inputFile)
        {
            var lines = new LineParser().ReadData(inputFile);
            var caveSystem = new Dictionary<string, Cave>();
            foreach (var line in lines)
            {
                var caves = line.Split('-');
                var cave1 = caveSystem.ContainsKey(caves[0]) ? caveSystem[caves[0]] : new Cave { Name = caves[0], IsLarge = char.IsUpper(caves[0][0]) };
                var cave2 = caveSystem.ContainsKey(caves[1]) ? caveSystem[caves[1]] : new Cave { Name = caves[1], IsLarge = char.IsUpper(caves[1][0]) };
                cave1.LinkedCaves.Add(cave2);
                cave2.LinkedCaves.Add(cave1);
                if (!caveSystem.ContainsKey(cave1.Name)) { caveSystem.Add(cave1.Name, cave1); }
                if (!caveSystem.ContainsKey(cave2.Name)) { caveSystem.Add(cave2.Name, cave2); }
            }
            var cavesToVisit = caveSystem.Where(cave => (cave.Value.IsLarge ||
                                                      cave.Value.LinkedCaves.Count > 1 ||
                                                      (cave.Value.LinkedCaves.Count == 1 && cave.Value.LinkedCaves[0].IsLarge))
                                                      && cave.Key != "start"
                                                      ).ToDictionary(c => c.Key, c => c.Value);
            var startPath = System.Linq.Enumerable.Repeat(caveSystem["start"], 1);
            var foundPaths = System.Linq.Enumerable.Repeat(startPath, 1);
            var caveDictionaries = new Dictionary<string, Dictionary<string, Cave>>
            {
                { "start", cavesToVisit }
            };
            var lastResults = 0;
            while (lastResults != caveDictionaries.Count)
            {
                //first: {start}
                lastResults = caveDictionaries.Count;
                var pathsForNextRun = new List<IEnumerable<Cave>>();
                foreach (var path in foundPaths)
                {
                    //{start}
                    var stringPath = string.Join(',', path.Select(p=>p.Name));
                    var cavesToUse = caveDictionaries[stringPath];
                    var nextPaths = FindNextPath(cavesToUse, path); // return: [{start,A},A],[{start,b},b]
                    pathsForNextRun.AddRange(nextPaths.Select(t => t.Item1));
                    
                    //Calculate new cave dictionaries for each path.
                    foreach (var item in nextPaths)
                    {
                        var stringNewPath = string.Join(',', item.Item1.Select(p=>p.Name));
                        if (!caveDictionaries.ContainsKey(stringNewPath))
                        {
                            //small cave, remove from available caves
                            var newCavesToUse = !item.Item2.IsLarge ? cavesToUse.Where(c => c.Key != item.Item2.Name).ToDictionary(c => c.Key, c => c.Value) : cavesToUse;
                            caveDictionaries.Add(stringNewPath, newCavesToUse);
                        }
                    }
                }
                foundPaths = pathsForNextRun;

            }
            var completePaths = caveDictionaries.Where(c => c.Key.EndsWith("end"));
            return completePaths.LongCount();
        }

        static IEnumerable<Tuple<IEnumerable<Cave>,Cave>> FindNextPath(Dictionary<string,Cave> caveSystem, IEnumerable<Cave> path)
        {
            var lastNodeVisited = path.Last();
            if (lastNodeVisited.Name == "end") { yield break; }
            foreach (var linkedCave in lastNodeVisited.LinkedCaves.Where(lc=> caveSystem.ContainsKey(lc.Name)))
            {
                yield return new (path.Append(linkedCave), linkedCave);
            }
        }

        static long Second(string inputFile)
        {
            var lines = new LineParser().ReadData(inputFile);
            var caveSystem = new Dictionary<string, Cave>();
            foreach (var line in lines)
            {
                var caves = line.Split('-');
                var cave1 = caveSystem.ContainsKey(caves[0]) ? caveSystem[caves[0]] : new Cave { Name = caves[0], IsLarge = char.IsUpper(caves[0][0]) };
                var cave2 = caveSystem.ContainsKey(caves[1]) ? caveSystem[caves[1]] : new Cave { Name = caves[1], IsLarge = char.IsUpper(caves[1][0]) };
                cave1.LinkedCaves.Add(cave2);
                cave2.LinkedCaves.Add(cave1);
                if (!caveSystem.ContainsKey(cave1.Name)) { caveSystem.Add(cave1.Name, cave1); }
                if (!caveSystem.ContainsKey(cave2.Name)) { caveSystem.Add(cave2.Name, cave2); }
            }
            var cavesToVisit = caveSystem.Where(cave => (cave.Value.IsLarge ||
                                                      cave.Value.LinkedCaves.Count > 1 ||
                                                      (cave.Value.LinkedCaves.Count == 1 && cave.Value.LinkedCaves[0].IsLarge))
                                                      && cave.Key != "start"
                                                      ).ToDictionary(c => c.Key, c => c.Value);
            var startPath = System.Linq.Enumerable.Repeat(caveSystem["start"], 1);
            var foundPaths = System.Linq.Enumerable.Repeat(startPath, 1);
            var caveDictionaries = new Dictionary<string, Tuple<Dictionary<string, Cave>,bool>>
            {
                { "start", new (cavesToVisit, false) }
            };
            var lastResults = 0;
            while (lastResults != caveDictionaries.Count)
            {
                //first: {start}
                lastResults = caveDictionaries.Count;
                var pathsForNextRun = new List<IEnumerable<Cave>>();
                foreach (var path in foundPaths)
                {
                    //{start}
                    var stringPath = string.Join(',', path.Select(p => p.Name));
                    var cavesToUse = caveDictionaries[stringPath].Item1;
                    var usedDoubleEntry = caveDictionaries[stringPath].Item2;
                    var nextPaths = FindNextPath(cavesToUse, path); // return: [{start,A},A],[{start,b},b]
                    pathsForNextRun.AddRange(nextPaths.Select(t => t.Item1));

                    //Calculate new cave dictionaries for each path.
                    foreach (var item in nextPaths)
                    {
                        var stringNewPath = string.Join(',', item.Item1.Select(p => p.Name));
                        if (!caveDictionaries.ContainsKey(stringNewPath))
                        {
                            //small cave, remove from available caves when visited twice
                            //do this to check if in the old path the item name already exists
                            var removeCave = !item.Item2.IsLarge && (usedDoubleEntry || stringPath.Contains("," + item.Item2.Name, StringComparison.Ordinal));
                            
                            var newCavesToUse = removeCave ? 
                                cavesToUse.Where(c => c.Key != item.Item2.Name).ToDictionary(c => c.Key, c => c.Value) : cavesToUse;
                            
                            caveDictionaries.Add(stringNewPath, new (newCavesToUse, usedDoubleEntry || (!item.Item2.IsLarge&&!removeCave)));
                        }
                    }
                }
                foundPaths = pathsForNextRun;

            }
            var completePaths = caveDictionaries.Where(c => c.Key.EndsWith("end")).OrderBy(c=>c.Key);
            return completePaths.LongCount();
        }

        [DataTestMethod]
        [DataRow("test.txt", 10L)]
        [DataRow("test-2.txt", 19L)]
        [DataRow("test-3.txt", 226L)]
        public void TestPart1(string inputFile, long paths)
        {
            var result = First(inputFile);
            Assert.AreEqual(paths, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 36L)]
        [DataRow("test-2.txt", 103L)]
        [DataRow("test-3.txt", 3509L)]
        public void TestPart2(string inputFile, long paths)
        {
            var result = Second(inputFile);
            Assert.AreEqual(paths, result);
        }
    }
}
