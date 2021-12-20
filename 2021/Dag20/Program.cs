using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
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
            var info = (new ImageParser()).ParseFile(inputFile);
            var orgImage = info.ImageData;
            var background = false;
            var newImage1 = FindNewImage(info, orgImage, background);
            var newBackgroundBit = background ? info.PixelData(511) : info.PixelData(0);
            var optimizedImage = StripBackground(newImage1, newBackgroundBit);
            var newImage2 = FindNewImage(info, optimizedImage, newBackgroundBit);
            newBackgroundBit = newBackgroundBit ? info.PixelData(511) : info.PixelData(0);
            optimizedImage = StripBackground(newImage2, newBackgroundBit);
            long count = 0;
            var m = optimizedImage.GetLength(0);
            var n = optimizedImage.GetLength(1);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n ; j++)
                {
                    if (optimizedImage[i, j]) { count++; }
                }
            }
            return count;
        }

        private static bool[,] StripBackground(bool[,] image, bool newBackgroundBit)
        {
            var m = image.GetLength(0);
            var n = image.GetLength(1);
            var minRow = 0;
            var minRowFound = false;
            var maxRow = m - 1;
            var maxRowFound = false;
            var minCol = 0;
            var minColFound = false;
            var maxCol = n - 1;
            var maxColFound = false;
            while (!minRowFound && minRow<m)
            {
                for (int j = 0; j < n; j++)
                {
                    if (newBackgroundBit != image[minRow,j])
                    {
                        minRowFound = true;
                        break;
                    }
                }
                if (!minRowFound) minRow++;
            }
            while (!maxRowFound && maxRow >= 0)
            {
                for (int j = 0; j < n; j++)
                {
                    if (newBackgroundBit != image[maxRow, j])
                    {
                        maxRowFound = true;
                        break;
                    }
                }
                if (!maxRowFound) maxRow--;
            }
            while (!minColFound && minCol < n)
            {
                for (int i = 0; i < m; i++)
                {
                    if (newBackgroundBit != image[i,minCol])
                    {
                        minColFound = true;
                        break;
                    }
                }
                if (!minColFound) minCol++;
            }
            while (!maxColFound && maxCol >= 0)
            {
                for (int i = 0; i < m; i++)
                {
                    if (newBackgroundBit != image[i,maxCol])
                    {
                        maxColFound = true;
                        break;
                    }
                }
                if (!maxColFound) maxCol--;
            }
            var newImage = new bool[maxRow - minRow+1, maxCol - minCol + 1];
            for (int i = 0; i <= maxRow - minRow; i++)
            {
                for (int j = 0; j <= maxCol - minCol; j++)
                {
                    newImage[i, j] = image[minRow + i, minCol + j];
                }
            }
            return newImage;
        }

        private static bool[,] FindNewImage(ImageInfo info, bool[,] orgImage, bool backgroundBit)
        {
            var m = orgImage.GetLength(0);
            var n = orgImage.GetLength(1);
            var newImage = new bool[m + 2, n + 2];
            for (int i = -1; i < m + 1; i++)
            {
                for (int j = -1; j < n + 1; j++)
                {
                    newImage[i + 1, j + 1] = NewPixel(i, j, m, n, orgImage, info, backgroundBit);
                }
            }
            return newImage;
        }

        private static bool NewPixel(int i, int j, int m, int n, bool[,] orgImage, ImageInfo info, bool backgroundBit)
        {
            var index = (((i >=  1 && j >=  1 && i <= m     && j <= n)     ? orgImage[i - 1, j - 1]   : backgroundBit) ? 256 : 0) +
                        (((i >=  1 && j >=  0 && i <= m     && j <  n)     ? orgImage[i - 1, j]       : backgroundBit) ? 128 : 0) +
                        (((i >=  1 && j >= -1 && i <= m     && j <  n - 1) ? orgImage[i - 1, j + 1]   : backgroundBit) ?  64 : 0) +
                        (((i >=  0 && j >=  1 && i  < m     && j <= n)     ? orgImage[i, j - 1]       : backgroundBit) ?  32 : 0) +
                        (((i >=  0 && j >=  0 && i  < m     && j <  n)     ? orgImage[i, j]           : backgroundBit) ?  16 : 0) +
                        (((i >=  0 && j >= -1 && i  < m     && j <  n - 1) ? orgImage[i, j + 1]       : backgroundBit) ?   8 : 0) +
                        (((i >= -1 && j >=  1 && i  < m - 1 && j <= n)     ? orgImage[i + 1, j - 1]   : backgroundBit) ?   4 : 0) +
                        (((i >= -1 && j >=  0 && i  < m - 1 && j < n)      ? orgImage[i + 1, j]       : backgroundBit) ?   2 : 0) +
                        (((i >= -1 && j >= -1 && i  < m - 1 && j < n - 1)  ? orgImage[i + 1, j + 1]   : backgroundBit) ?   1 : 0);
            return info.PixelData(index);
        }

        static long Second(string inputFile)
        {
            var info = (new ImageParser()).ParseFile(inputFile);
            var image = info.ImageData;
            var background = false;
            for (int step = 0; step < 50; step++)
            {
                var newImage = FindNewImage(info, image, background);
                background = background ? info.PixelData(511) : info.PixelData(0);
                var optimizedImage = StripBackground(newImage, background);
                image = optimizedImage;
            }
            long count = 0;
            var m = image.GetLength(0);
            var n = image.GetLength(1);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (image[i, j]) { count++; }
                }
            }
            return count;
        }

        [TestMethod]
        public void TestPart1()
        {
            var result = First("test.txt");
            Assert.AreEqual(35, result);
        }

        [TestMethod]
        public void TestPart2()
        {
            var result = Second("test.txt");
            Assert.AreEqual(3351, result);
        }
    }
    public class ImageInfo
    {
        public string EnhancementData { get; set; }
        public bool[,] ImageData { get; internal set; }

        public bool PixelData(int index)
        {
            return EnhancementData[index] == '#';
        }
    }

    public class ImageParser : CharMapParser<bool>
    {
        public override bool[,] ReadMap(string filePath)
        {
            var lines = Readlines(filePath);
            var restLines = lines.Skip(2).ToList();
            var n = restLines.First().Length;
            var m = restLines.Count();
            var result = new bool[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i, j] = Convert(restLines[i][j]);
                }
            }
            return result;
        }

        public ImageInfo ParseFile(string filePath)
        {
            var info = new ImageInfo();
            info.EnhancementData = Readlines(filePath).First();
            info.ImageData = ReadMap(filePath);
            return info;
        }

        protected override bool Convert(char input)
        {
            return input == '#';
        }
    }
}
