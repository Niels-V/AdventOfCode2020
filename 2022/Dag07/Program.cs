using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    [TestCategory("2022")]
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
            var lines = System.IO.File.ReadLines(inputFile);
            var fileSystem = ParseOutput(lines);
            var folders = new List<DirectoryInfo>();
            FillFolders(folders, fileSystem);
            var targetFolders = folders.Where(f=>f.Size <=100000);
            return targetFolders.Sum(f=>f.Size);
        }
        
        private static void FillFolders(List<DirectoryInfo> folders, DirectoryInfo dir) {
            var subFolders = dir.Items.Where(d=>d.GetType()==typeof(DirectoryInfo)).ToList();
            foreach (var folder in subFolders) {
                folders.Add((DirectoryInfo)folder);
                FillFolders(folders, (DirectoryInfo)folder);
            }
        }

        private static DirectoryInfo ParseOutput(IEnumerable<string> lines)
        {
            var rootDirectory = new DirectoryInfo { Name = "/" };
            var currentDirectory = rootDirectory;
            foreach (var line in lines)
            {
                if (line == "$ cd /")
                {
                    currentDirectory = rootDirectory;
                    continue;
                }
                if (line.StartsWith("$"))
                {
                    if (line.StartsWith("$ cd "))
                    {
                        var dirName = line.Substring(5);
                        if (dirName == "..")
                        {
                            currentDirectory = currentDirectory.Parent;
                        }
                        else
                        {
                            var directory = (DirectoryInfo)currentDirectory.Items.First(d => d.Name == dirName);
                            currentDirectory = directory;
                        }
                    }
                    else
                    {
                        //it should be the list command, don't need anything special for that.
                    }

                }
                else
                {
                    var lineComponents = line.Split(' ');
                    if (lineComponents[0] == "dir")
                    {
                        var directory = new DirectoryInfo { Name = lineComponents[1], Parent = currentDirectory };
                        currentDirectory.Items.Add(directory);
                    }
                    else
                    {
                        var file = new FileInfo(Convert.ToInt64(lineComponents[0])) { Name = lineComponents[1], Parent = currentDirectory };
                        currentDirectory.Items.Add(file);
                    }
                }
            }
            return rootDirectory;
        }

        static long Second(string inputFile)
        {
            var lines = System.IO.File.ReadLines(inputFile);
            var fileSystem = ParseOutput(lines);
            var folders = new List<DirectoryInfo>();
            FillFolders(folders, fileSystem);
            var flattenFolders = folders.Select(f=> new {Name = f.Name, Size = f.Size}).ToList();
            var totalSize = 70000000L;
            var neededSizeFree = 30000000L;
            var currentSizeFree = totalSize-fileSystem.Size;
            var minFreeSize = neededSizeFree-currentSizeFree;
            
            var folder = flattenFolders.Where(f=>f.Size>=minFreeSize).OrderBy(f=>f.Size).First();
            return folder.Size;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(95437, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(24933642, result);
        }
    }
    public abstract class FsoObject {
        public abstract long Size { get; }
        public string Name {get; set;}
        public DirectoryInfo Parent {get;set;}
    }
    public class DirectoryInfo : FsoObject {
        public List<FsoObject> Items {get;}
        public DirectoryInfo()
        {
            Items = new List<FsoObject>();
        }

        public override long Size => Items.Sum(fso=>fso.Size);
    }
    public class FileInfo:FsoObject {
        private long _size = 0L;
        public FileInfo(long size)
        {
            _size=size;
        }   
        public override long Size => _size;
    }
}
