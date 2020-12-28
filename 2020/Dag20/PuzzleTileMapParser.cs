using Common;
using System.Linq;
using System.Collections.Generic;

namespace Dag20
{
    public class PuzzleTileMapParser : CharMapParser<TileElement>
    {
        private static PuzzleTileMapParser instance = null;
        public static PuzzleTileMapParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PuzzleTileMapParser();
                }
                return instance;
            }
        }
        protected override TileElement Convert(char input)
        {
            return input.ToString().ParseEnumValue<TileElement>();
        }

        public override TileElement[,] ReadMap(string filePath)
        {
            return null;
        }

        public IList<PuzzleTile> ReadTiles(string filePath, int tileSize)
        {
            var result = new List<PuzzleTile>();
            var lines = Readlines(filePath).ToList();
            var currentLine = 0;
            for (int t = 0; t < (lines.Count + 1) / (tileSize + 2); t++)
            {
                var tileId = System.Convert.ToInt32(lines[currentLine++].Substring(5, 4));
                var tileData = new TileElement[tileSize, tileSize];
                for (int i = 0; i < tileSize; i++)
                {
                    for (int j = 0; j < tileSize; j++)
                    {
                        tileData[i, j] = Convert(lines[currentLine][j]);
                    }
                    currentLine++;
                }
                currentLine++;
                var tile = new PuzzleTile() { TileId = tileId, TileData = tileData };
                result.Add(tile);
            }
            return result;
        }
    }
}
