using System.Runtime.Serialization;

namespace Dag14
{
    public enum OperationType
    {
        [EnumMember(Value = "mask")]
        Mask,
        [EnumMember(Value = "mem")]
        Memory
    }
}
