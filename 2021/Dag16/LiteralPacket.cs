using System;
using System.Collections;

namespace AoC
{
    public class LiteralPacket : Packet
    {
        private long _literalValue;
        public override long Value => _literalValue;
        public override int VersionSum => Version;

        public LiteralPacket(BitArray bits) : base(bits)
        {
        }
        
        protected override void Parse()
        {
            base.Parse();
            if (TypeId != 4) { throw new InvalidOperationException("Invalid typeid"); }
            int bitIndex = 6;
            long returnValue = 0;
            const int valueBitLength = 4;
            var readNextSegment = true;
            while (readNextSegment)
            {
                readNextSegment = Bits[bitIndex++] == true;
                for (int i = 0; i < valueBitLength; i++)
                {
                    returnValue *= 2;
                    returnValue += Bits[bitIndex++] ? 1L : 0L;
                }
            }
            _literalValue = returnValue;
            LastBitIndex = bitIndex;
        }
    }
}
