using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dag25
{
    [TestCategory("2020")]
    [TestClass]
    public class LockSystem
    {
        static LockSystem()
        {
            Subject = 7;
        }
        public static long Subject { get; }
        public long CardPublicKey { get; set; }
        public long DoorPublicKey { get; set; }
        public long CardLoopSize { get; set; }
        public long DoorLoopSize { get; set; }
        public long EncryptionKey { get; set; }

        public long CalcHandShake(long loopSize)
        {
            //The handshake used by the card and the door involves an operation that transforms a subject number.
            //To transform a subject number, start with the value 1.
            var acc = 1L;
            //Then, a number of times called the loop size, perform the following steps:
            for (int i = 0; i < loopSize; i++)
            {
                //Set the value to itself multiplied by the subject number.
                acc *= Subject;
                //Set the value to the remainder after dividing the value by 20201227.
                acc = acc % 20201227;
            }
            return acc;
        }

        public long CalcPublicKey(long subject, long loopSize)
        {
            //The handshake used by the card and the door involves an operation that transforms a subject number.
            //To transform a subject number, start with the value 1.
            var acc = 1L;
            //Then, a number of times called the loop size, perform the following steps:
            for (int i = 0; i < loopSize; i++)
            {
                //Set the value to itself multiplied by the subject number.
                acc *= subject;
                //Set the value to the remainder after dividing the value by 20201227.
                acc = acc % 20201227;
            }
            return acc;
        }

        public long ReverseCardLoopSize()
        {
            CardLoopSize= ReverseLoopSize(CardPublicKey);
            return CardLoopSize;
        }
        public long ReverseDoorLoopSize()
        {
            DoorLoopSize = ReverseLoopSize(DoorPublicKey);
            return DoorLoopSize;
        }

        public long ReverseEncryptionKey()
        {
            var doorLoopSize = ReverseDoorLoopSize();
            EncryptionKey = CalcPublicKey(CardPublicKey, doorLoopSize);
            return EncryptionKey;
        }

        public static long ReverseLoopSize(long publicKey)
        {
            var acc = 1L;
            //Then, a number of times called the loop size, perform the following steps:
            var loopSize = 0L;
            while (acc != publicKey)
            {
                loopSize++;
                //Set the value to itself multiplied by the subject number.
                acc *= Subject;
                //Set the value to the remainder after dividing the value by 20201227.
                acc = acc % 20201227;
            }
            return loopSize;
        }

        [TestMethod]
        public void TestReverseCardLoopSize()
        {
            var lockSystem = new LockSystem { CardPublicKey = 5764801 };
            var loopSize = lockSystem.ReverseCardLoopSize();
            Assert.AreEqual(loopSize, 8);
        }

        [TestMethod]
        public void TestReverseDoorLoopSize()
        {
            var lockSystem = new LockSystem { DoorPublicKey = 17807724 };
            var loopSize = lockSystem.ReverseDoorLoopSize();
            Assert.AreEqual(loopSize, 11);
        }


    }
}
