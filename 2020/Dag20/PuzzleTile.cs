using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Dag20
{
    public class PuzzleTile
    {
        const int tileSize = 10;
        public int TileId { get; set; }
        public TileElement[,] TileData { get; set; }

        public short Top => TilePositions[(int)Orientation][0];
        public short Right => TilePositions[(int)Orientation][1];
        public short Bottom => TilePositions[(int)Orientation][2];
        public short Left => TilePositions[(int)Orientation][3];

        public TileOrientation Orientation { get; set; }

        public List<short[]> TilePositions { get; set; }

        public PuzzleTile()
        {
            TilePositions = new List<short[]>(8);
        }

        public void CalculateBorderData(bool firstWithInverseOnly)
        {
            TilePositions.Clear();
            var topElements = new BitArray(System.Linq.Enumerable.Range(0, 10).Select(i => TileData[i, 0] == TileElement.Hash).ToArray());
            var rightElements = new BitArray(System.Linq.Enumerable.Range(0, 10).Select(i => TileData[tileSize - 1, i] == TileElement.Hash).ToArray());
            var bottomElements = new BitArray(System.Linq.Enumerable.Range(0, 10).Select(i => TileData[i, tileSize - 1] == TileElement.Hash).ToArray());
            var leftElements = new BitArray(System.Linq.Enumerable.Range(0, 10).Select(i => TileData[0, i] == TileElement.Hash).ToArray());

            AddPositions(topElements, rightElements, bottomElements, leftElements);
            if (firstWithInverseOnly)
            {
                topElements = BitsReverse(topElements);
                leftElements = BitsReverse(leftElements);
                bottomElements = BitsReverse(bottomElements);
                rightElements = BitsReverse(rightElements);
                AddPositions(topElements, rightElements, bottomElements, leftElements);
                return;
            }
            //rotate 3 times, so
            for (int i = 0; i < 3; i++)
            {
                var tempTop = topElements;
                topElements = BitsReverse(leftElements);
                leftElements = bottomElements;
                bottomElements = BitsReverse(rightElements);
                rightElements = tempTop;

                AddPositions(topElements, rightElements, bottomElements, leftElements);
            }
            //rotate back to original
            var tempTop2 = topElements;
            topElements = BitsReverse(leftElements);
            leftElements = bottomElements;
            bottomElements = BitsReverse(rightElements);
            rightElements = tempTop2;

            //now flip one time
            var tempLeft = leftElements;
            topElements = BitsReverse(topElements);
            leftElements = rightElements;
            bottomElements = BitsReverse(bottomElements);
            rightElements = tempLeft;
            AddPositions(topElements, rightElements, bottomElements, leftElements);
            //rotate 3 times
            for (int i = 0; i < 3; i++)
            {
                var tempTop = topElements;
                topElements = BitsReverse(leftElements);
                leftElements = bottomElements;
                bottomElements = BitsReverse(rightElements);
                rightElements = tempTop;

                AddPositions(topElements, rightElements, bottomElements, leftElements);
            }
        }

        internal TileElement TilePoint(int m, int n)
        {
            bool flipped = ((int)Orientation) > 3;
            n = flipped ? n : (tileSize-1 - n) ;
            int temp;
            switch (Orientation)
            {
                default:
                case TileOrientation.Rotate270:
                    n = tileSize - n - 1;
                    m = tileSize - m - 1;
                    return TileData[m, n];
                case TileOrientation.FlippedRotate270:
                    return TileData[m, n];
                case TileOrientation.Original:
                case TileOrientation.Flipped:
                    temp = n;
                    n = m;
                    m = tileSize - temp - 1;
                    return TileData[m, n];
                case TileOrientation.Rotate90:
                    return TileData[m, n];
                case TileOrientation.FlippedRotate90:
                    n = tileSize - n - 1;
                    m = tileSize - m - 1;
                    return TileData[m, n];
                case TileOrientation.Rotate180:
                case TileOrientation.FlippedRotate180:
                    temp = n;
                    n = tileSize - m - 1;
                    m = temp;
                    return TileData[m, n];
            }
        }

        private void AddPositions(BitArray topElements, BitArray rightElements, BitArray bottomElements, BitArray leftElements)
        {
            short topNumber = ToInt16(topElements);
            short rightNumber = ToInt16(rightElements);
            short bottomNumber = ToInt16(bottomElements);
            short leftNumber = ToInt16(leftElements);
            TilePositions.Add(new[] { topNumber, rightNumber, bottomNumber, leftNumber });
        }

        internal static short ToInt16(BitArray bitArray)
        {
            var array = new byte[2];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToInt16(array, 0);
        }
        internal static short ReverseToInt16(BitArray bitArray)
        {
            var array = new byte[2];
            BitsReverse(bitArray).CopyTo(array, 0);
            return BitConverter.ToInt16(array, 0);
        }
        internal static BitArray BitsReverse(BitArray bits)
        {
            int len = bits.Count;
            BitArray a = new BitArray(bits);
            BitArray b = new BitArray(bits);

            for (int i = 0, j = len - 1; i < len; ++i, --j)
            {
                a[i] = a[i] ^ b[j];
                b[j] = a[i] ^ b[j];
                a[i] = a[i] ^ b[j];
            }
            return a;
        }
    }
}
