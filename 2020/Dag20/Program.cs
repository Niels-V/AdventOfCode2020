using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Diagnostics;

namespace Dag20
{
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
            var tileData = PuzzleTileMapParser.Instance.ReadTiles(inputFile, 10);
            foreach (var tile in tileData)
            {
                tile.CalculateBorderData(true);
            }
            var tileBorderData = tileData.SelectMany(x => x.TilePositions.SelectMany(p => p)).GroupBy(t => t).Select(s => new { Border = s.Key, Count = s.Count() }).Where(s => s.Count == 2).Select(s => s.Border).ToList();
            var cornerTiles = FindCorners(tileData, tileBorderData);

            var acc = 1L;
            {
                foreach (var (tile, borders) in cornerTiles)
                {
                    acc *= tile.TileId;
                }
            }
            return acc;
        }

        private static IList<Tuple<PuzzleTile, bool[]>> FindCorners(IList<PuzzleTile> tileData, IList<short> tileBorderData)
        {
            var cornertiles = tileData.Select(t => new Tuple<PuzzleTile, bool[]>(t, t.TilePositions.First().Select(p => tileBorderData.Contains(p)).ToArray())).Where(bt => bt.Item2.Count(b => b == true) == 2);

            if (cornertiles.Count() != 4) { throw new InvalidOperationException("Not exactly 4 corners found"); }
            return cornertiles.ToList();
        }
        private static IList<Tuple<PuzzleTile, bool[]>> FindBorders(IList<PuzzleTile> tileData, IList<short> tileBorderData)
        {
            var bordertiles = tileData.Select(t => new Tuple<PuzzleTile, bool[]>(t, t.TilePositions.First().Select(p => tileBorderData.Contains(p)).ToArray())).Where(bt => bt.Item2.Count(b => b == true) == 1);

            return bordertiles.ToList();
        }

        static int Second(string inputFile)
        {
            var tileData = PuzzleTileMapParser.Instance.ReadTiles(inputFile, 10);
            var gridSize = Convert.ToInt32(Math.Sqrt(tileData.Count));
            var puzzleData = PuzzleTile(tileData, gridSize);
            //for (int i = 0; i < gridSize; i++)
            //{
            //    for (int j = 0; j < gridSize; j++)
            //    {
            //        Console.Write(puzzleData[i, j].TileId + " ");
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine();

            for (int j = 0; j < 12; j++)
            {
                PuzzleTile lastTile = puzzleData[j, 0];
                for (int i = 1; i < 12; i++)
                {
                    var nextTile = puzzleData[j, i];
                    Debug.Assert(nextTile.Left == lastTile.Right);
                    lastTile = nextTile;
                }
            }
            for (int i = 0; i < 12; i++)
            {
                PuzzleTile lastTile = puzzleData[0, i];
                for (int j = 1; j < 12; j++)
                {
                    var nextTile = puzzleData[j, i];
                    Debug.Assert(nextTile.Top == lastTile.Bottom);
                    lastTile = nextTile;
                }
            }

            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"..\..\..\tile.txt"))
            //{
            //    for (int j = 0; j < gridSize; j++)
            //    {
            //        for (int i = 0; i < gridSize; i++)
            //        {
            //            file.Write(puzzleData[j, i].TileId + " ");
            //        }
            //        file.WriteLine();
            //    }
            //    file.Close();
            //}

            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"..\..\..\orientation.txt"))
            //{
            //    for (int j = 0; j < gridSize; j++)
            //    {
            //        for (int i = 0; i < gridSize; i++)
            //        {
            //            file.Write(puzzleData[j, i].Orientation + " ");
            //        }
            //        file.WriteLine();
            //    }
            //    file.Close();
            //}

            TileElement[,] combinedImage = CreateImage(gridSize, puzzleData);

            //using (System.IO.StreamWriter file =
            //new System.IO.StreamWriter(@"..\..\..\output.txt"))
            //{
            //    for (int j = 0; j< gridSize * 8; j++)
            //    {
            //        for (int i = gridSize*8-1; i>=0; i--)
            //        {
            //            file.Write(combinedImage[i, j] == TileElement.Hash ? '#' : '.');
            //            Console.Write(combinedImage[i, j] == TileElement.Hash ? '#' : '.');
            //        }
            //        Console.WriteLine();
            //        file.WriteLine();
            //    }
            //    Console.WriteLine();
            //    file.WriteLine();
            //    file.Close();
            //}

            var monsterHashCount = FindMonsters(combinedImage) * 15;
            var hashCount = FindHash(combinedImage);
            return hashCount - monsterHashCount;
        }

        private static int FindHash(TileElement[,] combinedImage)
        {
            int acc = 0;
            for (int i = 0; i < combinedImage.GetLength(0); i++)
            {
                for (int j = 0; j < combinedImage.GetLength(0); j++)
                {
                    if (combinedImage[i, j] == TileElement.Hash) { acc++; }
                }
            }
            return acc;
        }

        private static int FindMonsters(TileElement[,] combinedImage)
        {
            var monsterFilter1 = new[,]{{false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,false},
                                {true,false,false,false,false,true,true,false,false,false,false,true,true,false,false,false,false,true,true,true},
                                {false,true,false,false,true,false,false,true,false,false,true,false,false,true,false,false,true,false,false,false} };
            var monsterFilter2 = new bool[20, 3];
            var monsterFilter3 = new bool[3, 20];
            var monsterFilter4 = new bool[20, 3];
            var monsterFilter5 = new bool[3, 20];
            var monsterFilter6 = new bool[20, 3];
            var monsterFilter7 = new bool[3, 20];
            var monsterFilter8 = new bool[20, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    monsterFilter2[j, i] = monsterFilter1[i, j];
                    monsterFilter3[i, j] = monsterFilter1[i, 19 - j];
                    monsterFilter4[j, i] = monsterFilter3[i, j];
                    monsterFilter5[i, j] = monsterFilter1[2 - i, j];
                    monsterFilter6[j, i] = monsterFilter5[i, j];
                    monsterFilter7[i, j] = monsterFilter1[2 - i, 19 - j];
                    monsterFilter8[j, i] = monsterFilter7[i, j];
                }
            }
            int monsterCount1 = 0, monsterCount2 = 0, monsterCount3 = 0, monsterCount4 = 0, monsterCount5 = 0, monsterCount6 = 0, monsterCount7 = 0, monsterCount8 = 0;
            for (int i = 0; i < combinedImage.GetLength(0) - 19; i++)
            {
                for (int j = 0; j < combinedImage.GetLength(0) - 2; j++)
                {
                    var monster1 = 0;
                    var monster3 = 0;
                    var monster5 = 0;
                    var monster7 = 0;
                    for (int n = 0; n < 20; n++)
                    {
                        for (int m = 0; m < 3; m++)
                        {
                            if (monsterFilter1[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster1++;
                            if (monsterFilter3[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster3++;
                            if (monsterFilter5[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster5++;
                            if (monsterFilter7[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster7++;
                        }
                    }
                    if (monster1 == 15) { monsterCount1++; }
                    if (monster3 == 15) { monsterCount3++; }
                    if (monster5 == 15) { monsterCount5++; }
                    if (monster7 == 15) { monsterCount7++; }
                }
            }
            for (int i = 0; i < combinedImage.GetLength(0) - 2; i++) // [0..93]
            {
                for (int j = 0; j < combinedImage.GetLength(0) - 19; j++) // [0..76]
                {
                    var monster2 = 0;
                    var monster4 = 0;
                    var monster6 = 0;
                    var monster8 = 0;
                    for (int n = 0; n < 3; n++) //[0..2]
                    {
                        for (int m = 0; m < 20; m++) //[0..19]
                        {
                            if (monsterFilter2[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster2++; //[0..95,0..95]
                            if (monsterFilter4[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster4++;
                            if (monsterFilter6[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster6++;
                            if (monsterFilter8[m, n] && combinedImage[j + m, i + n] == TileElement.Hash) monster8++;
                        }
                    }
                    if (monster2 == 15) { monsterCount2++; }
                    if (monster4 == 15) { monsterCount4++; }
                    if (monster6 == 15) { monsterCount6++; }
                    if (monster8 == 15) { monsterCount8++; }
                }
            }
            if (monsterCount1 > 0) return monsterCount1;
            if (monsterCount2 > 0) return monsterCount2;
            if (monsterCount3 > 0) return monsterCount3;
            if (monsterCount4 > 0) return monsterCount4;
            if (monsterCount5 > 0) return monsterCount5;
            if (monsterCount6 > 0) return monsterCount6;
            if (monsterCount7 > 0) return monsterCount7;
            if (monsterCount8 > 0) return monsterCount8;
            throw new InvalidOperationException("no monsters found");
        }
        private static TileElement[,] CreateImage(int gridSize, PuzzleTile[,] puzzleData)
        {
            var combinedImageSize = gridSize * 8;
            var combinedImage = new TileElement[combinedImageSize, combinedImageSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    for (int n = 0; n < 8; n++)
                    {
                        for (int m = 0; m < 8; m++)
                        {
                            combinedImage[i * 8 + n, j * 8 + m] = puzzleData[i, j].TilePoint(n + 1, m + 1);
                        }
                    }
                }
            }

            return combinedImage;
        }

        private static PuzzleTile[,] PuzzleTile(IList<PuzzleTile> tileData, int gridSize)
        {
            var puzzled = new PuzzleTile[gridSize, gridSize];
            foreach (var tile in tileData)
            {
                tile.CalculateBorderData(true);
            }
            var tileBorderData = tileData.SelectMany(x => x.TilePositions.SelectMany(p => p)).GroupBy(t => t).Select(s => new { Border = s.Key, Count = s.Count() }).Where(s => s.Count == 1).Select(s => s.Border).ToList();

            var cornerTiles = FindCorners(tileData, tileBorderData);

            //take first corner tile, and lay it down with the right orientation
            var firstCornerTile = cornerTiles.First().Item1;
            firstCornerTile.CalculateBorderData(false);
            var firstCornerOrientationVector = cornerTiles.First().Item2;
            var firstCornerOrientation = TileOrientation.Original;
            if (firstCornerOrientationVector[0] && firstCornerOrientationVector[1])
            {
                firstCornerOrientation = TileOrientation.Rotate270;
            }
            else if (firstCornerOrientationVector[1] && firstCornerOrientationVector[2])
            {
                firstCornerOrientation = TileOrientation.Rotate180;
            }
            else if (firstCornerOrientationVector[2] && firstCornerOrientationVector[3])
            {
                firstCornerOrientation = TileOrientation.Rotate90;
            }
            firstCornerTile.Orientation = firstCornerOrientation;
            var n = gridSize - 1;
            puzzled[0, 0] = firstCornerTile;
            var lastTile = firstCornerTile;

            var borderTiles = FindBorders(tileData, tileBorderData);
            var nonBorderTiles = tileData.Where(t => !borderTiles.Any(b => b.Item1.TileId == t.TileId) && !cornerTiles.Any(b => b.Item1.TileId == t.TileId)).ToList();
            foreach (var tile in borderTiles)
            {
                tile.Item1.CalculateBorderData(false);
            }
            foreach (var tile in nonBorderTiles)
            {
                tile.CalculateBorderData(false);
            }
            PuzzleTile nextTile = null;
            //Top borders with the correct numbers
            for (int i = 1; i < n; i++)
            {
                nextTile = FindBorderTile(borderTiles, lastTile.Right, 0);
                puzzled[0, i] = nextTile;
                lastTile = nextTile;
                borderTiles.Remove(borderTiles.First(b => b.Item1.TileId == nextTile.TileId));
            }
            //find next corner
            nextTile = FindCornerTile(cornerTiles.Skip(1).Take(3), 1, lastTile.Right);
            puzzled[0, n] = nextTile;
            lastTile = nextTile;

            //find Right borders with the correct numbers
            for (int i = 1; i < n; i++)
            {
                nextTile = FindBorderTile(borderTiles, lastTile.Bottom, 1);
                puzzled[i, n] = nextTile;
                lastTile = nextTile;
                borderTiles.Remove(borderTiles.First(b => b.Item1.TileId == nextTile.TileId));
            }

            //find next corner
            nextTile = FindCornerTile(cornerTiles.Skip(1).Take(3), 2, lastTile.Bottom);
            puzzled[n, n] = nextTile;
            lastTile = nextTile;
            //find Bottom borders with the correct numbers
            for (int i = n - 1; i >= 1; i--)
            {
                nextTile = FindBorderTile(borderTiles, lastTile.Left, 2);
                puzzled[n, i] = nextTile;
                lastTile = nextTile;
                borderTiles.Remove(borderTiles.First(b => b.Item1.TileId == nextTile.TileId));
            }

            //find next corner
            nextTile = FindCornerTile(cornerTiles.Skip(1).Take(3), 3, lastTile.Left);
            puzzled[n, 0] = nextTile;
            lastTile = nextTile;
            //find Left borders with the correct numbers
            for (int i = n - 1; i >= 1; i--)
            {
                nextTile = FindBorderTile(borderTiles, lastTile.Top, 3);
                puzzled[i, 0] = nextTile;
                lastTile = nextTile;
                borderTiles.Remove(borderTiles.First(b => b.Item1.TileId == nextTile.TileId));
            }
            //check last piece for borders with the correct numbers
            if (lastTile.Top != firstCornerTile.Bottom)
            {
                throw new InvalidOperationException("last border tile mismatch with corner");
            }

            var fillSteps = (gridSize - 2) / 2 + (gridSize - 2) % 2;//5
            for (int s = 1; s <= fillSteps; s++) //[1 ..5]
            {
                for (int i = s; i < gridSize - s; i++) //[1..10] [2..9] [3..8] [4..7] [5..6]
                {
                    FillTile(puzzled, nonBorderTiles, i, s); //[1,1][2,1][3,1][4,1][5,1][6,1][7,1][8,1][9,1][10,1] / [2,2][3,2]..[9,2] / [3,3][4,3]..[8,3] / [4,4]..[7,4] / [5,5][6,5]  //10+8+6+4+2
                }
                for (int i = s + 1; i < gridSize - s; i++) //[2..10] [3..9] [4..8] [5..7] [6]
                {
                    FillTile(puzzled, nonBorderTiles, gridSize - s - 1, i);//[10,2][10,3][10,4][10,5][10,6][10,7][10,8][10,9][10,10] / [9,3][9,4]..[9,8][9,9] / [8,4]..[8,8] / [7,5]..[7,7] / [6,6] //9+7+5+3+1
                }
                for (int i = gridSize - s - 2; i > s; i--) //[9..2] [8..3][7..4][6..5]
                {
                    FillTile(puzzled, nonBorderTiles, i, gridSize - s - 1); //[9,10][8,10][7,10][6,10][5,10][4,10][3,10][2,10] / [8,9][7,9]..[3,9] / [7,8]..[4,8] / [6,7][5,7] //8+6+4+2
                }
                for (int i = gridSize - s - 1; i > s; i--) //[10..2][9..3][8..4][7..5][6]
                {
                    FillTile(puzzled, nonBorderTiles, s, i); //[1,10][1,9][1,8][1,7][1,6][1,5][1,4][1,3][1,2] / [2,9][2,8]..[2,3] / [3,8][3,7]..[3,4] / [4,7][4,6][4,5] / [5,6] //9+7+5+3+1
                }

            }
            return puzzled;
        }

        private static void FillTile(PuzzleTile[,] puzzled, List<PuzzleTile> nonBorderTiles, int posx, int posy)
        {
            short? needTop = posy > 0 && puzzled[posy - 1, posx] != null ? puzzled[posy - 1, posx].Bottom : new short?();
            short? needLeft = posx > 0 && puzzled[posy, posx - 1] != null ? puzzled[posy, posx - 1].Right : new short?();
            short? needBottom = posy < puzzled.GetLength(1) - 1 && puzzled[posy + 1, posx] != null ? puzzled[posy + 1, posx].Top : new short?();
            short? needRight = posx < puzzled.GetLength(0) - 1 && puzzled[posy, posx + 1] != null ? puzzled[posy, posx + 1].Left : new short?();

            PuzzleTile foundTile = null;
            foreach (var tile in nonBorderTiles)
            {
                for (int i = 0; i < 8; i++)
                {
                    var item = tile.TilePositions[i];
                    var match = (needTop.HasValue && item[0] == needTop.Value || !needTop.HasValue) &&
                        (needRight.HasValue && item[1] == needRight.Value || !needRight.HasValue) &&
                        (needBottom.HasValue && item[2] == needBottom.Value || !needBottom.HasValue) &&
                        (needLeft.HasValue && item[3] == needLeft.Value || !needLeft.HasValue);
                    if (match)
                    {
                        tile.Orientation = (TileOrientation)i;
                        puzzled[posy, posx] = tile;
                        foundTile = tile;
                        break;
                    }
                }
                if (foundTile != null)
                {
                    break;
                }
            }
            if (foundTile != null)
            {
                nonBorderTiles.Remove(foundTile);
            }
            else
            {
                throw new InvalidOperationException("No tile found");
            }
        }

        private static PuzzleTile FindCornerTile(IEnumerable<Tuple<PuzzleTile, bool[]>> cornerTiles, int cornerNumber, short lastPuzzle)
        {
            var inverse = FindInverse(lastPuzzle);
            foreach (var (tile, orientation) in cornerTiles)
            {
                var cornerOrientationStep = 0;

                tile.CalculateBorderData(false);
                var found = false;
                if (orientation[0] && orientation[1] && (tile.Left == lastPuzzle || tile.Left == inverse))
                {
                    found = true;
                }
                else if (orientation[1] && orientation[2] && (tile.Top == lastPuzzle || tile.Top == inverse))
                {
                    found = true;
                    cornerOrientationStep = 3;
                }
                else if (orientation[2] && orientation[3] && (tile.Right == lastPuzzle || tile.Right == inverse))
                {
                    found = true;
                    cornerOrientationStep = 2;
                }
                else if (orientation[3] && orientation[0] && (tile.Bottom == lastPuzzle || tile.Bottom == inverse))
                {
                    found = true;
                    cornerOrientationStep = 1;
                }
                else if (orientation[0] && orientation[1] && (tile.Bottom == lastPuzzle || tile.Bottom == inverse))
                {
                    found = true;
                    cornerOrientationStep = 5;
                }
                else if (orientation[1] && orientation[2] && (tile.Left == lastPuzzle || tile.Left == inverse))
                {
                    found = true;
                    cornerOrientationStep = 6;
                }
                else if (orientation[2] && orientation[3] && (tile.Top == lastPuzzle || tile.Top == inverse))
                {
                    found = true;
                    cornerOrientationStep = 7;
                }
                else if (orientation[3] && orientation[0] && (tile.Right == lastPuzzle || tile.Right == inverse))
                {
                    found = true;
                    cornerOrientationStep = 4;
                }
                if (found)
                {
                    var flipped = cornerOrientationStep >= 4 ? 4 : 0;
                    int rotate = (cornerOrientationStep + cornerNumber - 1) % 4;
                    tile.Orientation = (TileOrientation)(flipped + rotate);
                    return tile;
                }
            }
            throw new InvalidOperationException("No tile found");
        }

        private static PuzzleTile FindBorderTile(IList<Tuple<PuzzleTile, bool[]>> borderTiles, short lastPuzzle, int neededBorderPosition)
        {
            var inverse = FindInverse(lastPuzzle);
            foreach (var puzzle in borderTiles)
            {
                var foundBorderPosition = 0;
                while (!puzzle.Item2[foundBorderPosition])
                {
                    foundBorderPosition = (foundBorderPosition + 1) % 4;
                }
                var shouldRotateSteps = (neededBorderPosition - foundBorderPosition + 4) % 4;
                var orientation = (TileOrientation)shouldRotateSteps;

                var flipOrientation = (neededBorderPosition == 0 || neededBorderPosition == 2) ?
                    orientation == TileOrientation.Rotate90 ? TileOrientation.Rotate270 : orientation == TileOrientation.Rotate270 ? TileOrientation.Rotate90 : orientation :
                    orientation == TileOrientation.Original ? TileOrientation.Rotate180 : orientation == TileOrientation.Rotate180 ? TileOrientation.Original : orientation; //because of flip, the rotation is counter direction, but only on top and bottom?

                var lastPuzzleElementIndex = (neededBorderPosition + 3) % 4;
                //var lastPuzzleElementIndexFlipped = (neededBorderPosition + 1) % 4;
                if (puzzle.Item1.TilePositions[((int)orientation)][lastPuzzleElementIndex] == lastPuzzle ||
                    puzzle.Item1.TilePositions[((int)orientation)][lastPuzzleElementIndex] == inverse)
                {
                    puzzle.Item1.Orientation = orientation;
                    return puzzle.Item1;
                }
                else if (puzzle.Item1.TilePositions[((int)flipOrientation + 4)][lastPuzzleElementIndex] == lastPuzzle ||
                    puzzle.Item1.TilePositions[((int)flipOrientation + 4)][lastPuzzleElementIndex] == inverse)
                {
                    puzzle.Item1.Orientation = ((TileOrientation)(int)flipOrientation + 4);
                    return puzzle.Item1;
                }
            }
            throw new InvalidOperationException("No tile found");
        }

        private static short FindInverse(short lastPuzzle)
        {
            var ba = new BitArray(BitConverter.GetBytes(lastPuzzle));
            int len = 10;
            BitArray a = new BitArray(ba);
            BitArray b = new BitArray(ba);

            for (int i = 0, j = len - 1; i < len; ++i, --j)
            {
                a[i] = a[i] ^ b[j];
                b[j] = a[i] ^ b[j];
                a[i] = a[i] ^ b[j];
            }
            return Dag20.PuzzleTile.ToInt16(a);
        }

        [DataTestMethod]
        [DataRow("test.txt", 20899048083289)]
        public void TestPart1(string inputFile, long expectedResult)
        {
            var result = First(inputFile);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", 273)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }

        [DataTestMethod]
        [DataRow("test.txt", 9)]
        [DataRow("input.txt", 144)]
        public void TestTileDataParser(string inputFile, int expectedTiles)
        {
            var collection = PuzzleTileMapParser.Instance.ReadTiles(inputFile, 10);
            Assert.AreEqual(expectedTiles, collection.Count);
        }
    }
}
