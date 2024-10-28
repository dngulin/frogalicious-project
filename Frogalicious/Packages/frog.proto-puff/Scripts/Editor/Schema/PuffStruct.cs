using Frog.Collections;

namespace Frog.ProtoPuff.Editor.Schema
{
    [NoCopy]
    internal struct PuffStruct
    {
        public string Name;
        public RefList<PuffField> Fields;
    }

    internal struct PuffField
    {
        public byte Id;
        public string Name;
        public string Type;
        public bool IsRepeated;

        public PuffField(byte id, string name, string type, bool isRepeated)
        {
            Id = id;
            Name = name;
            Type = type;
            IsRepeated = isRepeated;
        }
    }
}