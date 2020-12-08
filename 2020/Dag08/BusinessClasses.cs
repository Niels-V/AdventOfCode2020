using System.Runtime.Serialization;

namespace Dag08
{
    public enum OperationType
    {
        [EnumMember(Value ="nop")]
        NoOperation,
        [EnumMember(Value ="acc")]
        Accumulator,
        [EnumMember(Value ="jmp")]
        Jumps
    }

    public class Instruction
    {
        public OperationType Operation { get; set; }
        public int Argument { get; set; }
    }
}
