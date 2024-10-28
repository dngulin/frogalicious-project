using Frog.Collections;

namespace Frog.ProtoPuff.Editor.Schema
{
    [NoCopy]
    internal struct PuffSchema
    {
        internal RefList<PuffEnum> Enums;
        internal RefList<PuffStruct> Structs;
    }
}