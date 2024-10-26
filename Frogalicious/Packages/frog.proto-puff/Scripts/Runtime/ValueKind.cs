
namespace Frog.ProtoPuff
{
    public enum ValueKind : byte
    {
        Primitive = 0,
        RepeatedPrimitive = 1,
        Struct = 2,
        RepeatedStruct = 3,
    }
}