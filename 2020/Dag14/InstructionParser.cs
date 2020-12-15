using System;
using System.Linq;
using Common;
using System.Collections;

namespace Dag14
{
    class InstructionParser : Parser<Instruction>
    {
        static readonly bool[] _prefix;
        static InstructionParser()
        {
            _prefix =  new bool[28]; 
        }
        internal static ulong ToUInt64(BitArray bitArray)
        {
            var array = new byte[8];
            BitsReverse(bitArray).CopyTo(array, 0);
            return BitConverter.ToUInt64(array, 0);
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

        protected override Instruction ParseLine(string line)
        {
            var operation = line.StartsWith("mask") ? OperationType.Mask : OperationType.Memory;
            if (operation == OperationType.Memory) 
            {
                var arg1 = Convert.ToUInt64(line.Substring(4, line.IndexOf(']')-4));
                var arg2 = Convert.ToUInt64(line.Substring(4+line.IndexOf(']')));
                return new Instruction { Operation = operation, Argument1 = arg1, Argument2=arg2};
            } else {
                string mask = line.Substring(7);
                var arg3 = new BitArray((mask.Select(c=>c=='1')).ToArray());
                var arg4 = new BitArray((mask.Select(c=>c!='X')).ToArray());

                return new Instruction { Operation = operation, Argument1 = ToUInt64(arg3), Argument2= ToUInt64(arg4)};
            }
        }
    }

    class InstructionParser2 : Parser<Instruction>
    {
        static readonly bool[] _prefix;
        static InstructionParser2()
        {
            _prefix = new bool[28];
        }
        internal static ulong ToUInt64(BitArray bitArray)
        {
            var array = new byte[8];
            BitsReverse(bitArray).CopyTo(array, 0);
            return BitConverter.ToUInt64(array, 0);
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

        protected override Instruction ParseLine(string line)
        {
            var operation = line.StartsWith("mask") ? OperationType.Mask : OperationType.Memory;
            if (operation == OperationType.Memory)
            {
                var arg1 = Convert.ToUInt64(line.Substring(4, line.IndexOf(']') - 4));
                var arg2 = Convert.ToUInt64(line.Substring(4 + line.IndexOf(']')));
                return new Instruction { Operation = operation, Argument1 = arg1, Argument2 = arg2 };
            }
            else
            {
                string mask = line.Substring(7);
                var arg3 = new BitArray((mask.Select(c => c == '1')).ToArray());
                var arg4 = new BitArray((mask.Select(c => c == 'X')).ToArray());

                return new Instruction { Operation = operation, Argument1 = ToUInt64(arg3), Argument2 = ToUInt64(arg4) };
            }
        }
    }
}
