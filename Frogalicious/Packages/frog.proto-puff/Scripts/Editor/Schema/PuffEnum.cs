using Frog.Collections;

namespace Frog.ProtoPuff.Editor.Schema
{
    [NoCopy]
    internal struct PuffEnum
    {
        public string Name;
        public Primitive UnderlyingType;
        public bool IsFlags;
        public RefList<PuffEnumItem> Items;
    }

    internal struct PuffEnumItem
    {
        public string Name;
        public string Value;
    }
}