using System.Runtime.Serialization;

namespace Dag11
{
    public enum SeatStatus
    {
        [EnumMember(Value=".")]
        Floor,
        [EnumMember(Value="L")]
        Empty,
        [EnumMember(Value="#")]
        Occupied
    }
}
