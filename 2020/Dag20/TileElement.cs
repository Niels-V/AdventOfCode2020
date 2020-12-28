using System.Runtime.Serialization;

namespace Dag20
{
    public enum TileElement
    {
        [EnumMember(Value = ".")]
        Dot,
        [EnumMember(Value = "#")]
        Hash
    }
}
