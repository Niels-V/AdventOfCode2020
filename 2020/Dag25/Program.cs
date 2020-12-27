using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Dag25
{
    [TestClass]
    public class Program
    {
        static void Main(string[] args)
        {
            var result = First(2084668, 3704642);
            Console.WriteLine("Program succes, found result: {0}", result);

            var result2 = Second("input.txt");
            Console.WriteLine("Program succes, found result: {0}", result2);

        }

        static long First(long cardPublicKey, long doorPublicKey)
        {
            var lockSystem = new LockSystem() { CardPublicKey = cardPublicKey, DoorPublicKey = doorPublicKey };
            lockSystem.ReverseCardLoopSize();
            lockSystem.ReverseDoorLoopSize();
            lockSystem.ReverseEncryptionKey();
            return lockSystem.EncryptionKey;
        }

        static int Second(string inputFile)
        {
            return -2;
        }

        [DataTestMethod]
        [DataRow(5764801, 17807724, 14897079)]
        public void TestPart1(long cardPublicKey, long doorPublicKey, long expectedResult)
        {
            var result = First(cardPublicKey, doorPublicKey);
            Assert.AreEqual(expectedResult, result);
        }


        [DataTestMethod]
        [DataRow("test.txt", 0)]
        public void TestPart2(string inputFile, int expectedResult)
        {
            var result = Second(inputFile);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
