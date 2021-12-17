using System.Collections;

namespace AoC
{
    public abstract class Packet
    {
        public abstract int VersionSum { get; }
        public abstract long Value { get; }
        public BitArray Bits { get; }
        public int Version { get; private set; }
        public int TypeId { get; private set; }
        public int LastBitIndex { get; protected set; }

        public Packet() { }

        public Packet(BitArray bits)
        {
            Bits = bits;
        }
        
        protected virtual void Parse()
        {
            Version = (Bits[0] ? 1 : 0) * 4 + (Bits[1] ? 1 : 0) * 2 + (Bits[2] ? 1 : 0);
            TypeId = (Bits[3] ? 1 : 0) * 4 + (Bits[4] ? 1 : 0) * 2 + (Bits[5] ? 1 : 0);
        }
        public static Packet ParseBits(BitArray bits)
        {
            Packet packet;
            if ((bits[3] ? 1 : 0) * 4 + (bits[4] ? 1 : 0) * 2 + (bits[5] ? 1 : 0) == 4)
            {
                packet = new LiteralPacket(bits);
            }
            else
            {
                packet = new OperatorPacket(bits);
            }
            packet.Parse();
            return packet;
        }
    }
}
