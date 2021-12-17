using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AoC
{
    public class OperatorPacket : Packet
    {
        public override int VersionSum => Version + SubPackets.Sum(p => p.VersionSum);
        public override long Value => TypeId switch
        {
            0 => SubPackets.Sum(p => p.Value),
            1 => SubPackets.Aggregate(1L, (prod, p) => prod * p.Value),
            2 => SubPackets.Min(p => p.Value),
            3 => SubPackets.Max(p => p.Value),
            5 => SubPackets[0].Value > SubPackets[1].Value ? 1 : 0,
            6 => SubPackets[0].Value < SubPackets[1].Value ? 1 : 0,
            7 => SubPackets[0].Value == SubPackets[1].Value ? 1 : 0,
            _ => throw new InvalidOperationException("Unknown TypeId"),
        };

        public int LengthTypeId { get; private set; }
        public int Length { get; private set; }
        public List<Packet> SubPackets { get; }
        
        public OperatorPacket(BitArray bits) : base(bits)
        {
            SubPackets = new List<Packet>();
        }

        protected override void Parse()
        {
            base.Parse();
            if (TypeId == 4|| TypeId<0 || TypeId>7) { throw new InvalidOperationException("Invalid typeid"); }
            LengthTypeId = Bits[6] ? 1 : 0;

            int bitLength = LengthTypeId == 0 ? 15 : 11;
            int lengthValue = 0;
            for (int i = 7; i < 7 + bitLength; i++)
            {
                lengthValue *= 2;
                lengthValue += Bits[i] ? 1 : 0;
            }
            Length = lengthValue;

            if (LengthTypeId == 0)
            {
                ParseSubPacketsByLength(Length);
                LastBitIndex = 22 + Length;
            }
            else
            {
                var bitsProcessed = ParseSubPacketsByAmount(Length);
                LastBitIndex = 18 + bitsProcessed;
            }
        }

        private int ParseSubPacketsByAmount(int packetAmount)
        {
            var newBits = ((BitArray)Bits.Clone()).RightShift(18);
            int currentBitIndex = 0;
            for (int i = 0; i < packetAmount; i++)
            {
                newBits = ((BitArray)newBits.Clone()).RightShift(currentBitIndex);
                var packet = ParseBits(newBits);
                SubPackets.Add(packet);
                currentBitIndex = packet.LastBitIndex;
            }
            return SubPackets.Sum(p => p.LastBitIndex);
        }

        private void ParseSubPacketsByLength(int bitLength)
        {
            var newBits = ((BitArray)Bits.Clone()).RightShift(22);
            int currentBitIndex = 0;
            int lastPacketWidth = 0;
            do
            {
                newBits = ((BitArray)newBits.Clone()).RightShift(lastPacketWidth);
                var packet = ParseBits(newBits);
                SubPackets.Add(packet);
                currentBitIndex += packet.LastBitIndex;
                lastPacketWidth = packet.LastBitIndex;
            }
            while (currentBitIndex < bitLength);
        }
    }
}
