using System.Runtime.Serialization;

namespace Dag17
{
    public enum CubeStatus
    {
        [EnumMember(Value=".")]
        Inactive,
        [EnumMember(Value="#")]
        Active
    }
}
