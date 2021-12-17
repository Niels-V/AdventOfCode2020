using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AoC
{
    [TestCategory("2021")]
    [TestClass]
    public class PacketTester
    {
        [DataTestMethod]
        [DataRow("D2FE28", 4)]
        [DataRow("38006F45291200", 6)]
        [DataRow("EE00D40C823060", 3)]
        [DataRow("8A004A801A8002F478", 2)]
        [DataRow("620080001611562C8802118E34", 0)]
        [DataRow("C0015000016115A2E0802F182340", 0)]
        [DataRow("A0016C880162017C3686B18A3D4780", 0)]
        public void TestTypeId(string hexString, int expectedTypeId)
        {
            var bits = Program.Hex2Bit(hexString);
            var packet = Packet.ParseBits(bits);
            Assert.AreEqual(expectedTypeId, packet.TypeId);
        }

        [DataTestMethod]
        [DataRow("D2FE28", 6)]
        [DataRow("8A004A801A8002F478", 16)]
        [DataRow("620080001611562C8802118E34", 12)]
        [DataRow("C0015000016115A2E0802F182340", 23)]
        [DataRow("A0016C880162017C3686B18A3D4780", 31)]
        public void TestVersionSum(string hexString, int expectedSum)
        {
            var bits = Program.Hex2Bit(hexString);
            var packet = Packet.ParseBits(bits);
            Assert.AreEqual(expectedSum, packet.VersionSum);
        }

        [DataTestMethod]
        [DataRow("38006F45291200", 0, 27)]
        [DataRow("EE00D40C823060", 1, 3)]
        [DataRow("8A004A801A8002F478", 1, 1)]
        public void TestLengthId(string hexString, int expectedTypeId, int expectedLength)
        {
            var bits = Program.Hex2Bit(hexString);
            var packet = Packet.ParseBits(bits) as OperatorPacket;
            Assert.AreEqual(expectedTypeId, packet.LengthTypeId);
            Assert.AreEqual(expectedLength, packet.Length);
        }

        [DataTestMethod]
        [DataRow("C200B40A82", 3)]
        [DataRow("04005AC33890", 54)]
        [DataRow("880086C3E88112", 7)]
        [DataRow("CE00C43D881120", 9)]
        [DataRow("D8005AC2A8F0", 1)]
        [DataRow("F600BC2D8F", 0)]
        [DataRow("9C005AC2F8F0", 0)]
        [DataRow("9C0141080250320F1802104A08", 1)]
        [DataRow("38006F45291200", 1)]
        [DataRow("EE00D40C823060", 3)]
        [DataRow("A0016C880162017C3686B18A3D4780",-1)]
        public void TestValue(string hexString, int expectedValue)
        {
            var bits = Program.Hex2Bit(hexString);
            var packet = Packet.ParseBits(bits) as OperatorPacket;
            Assert.AreEqual(expectedValue, packet.Value);
        }
    }
}
