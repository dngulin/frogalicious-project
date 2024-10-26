using System;

namespace Frog.ProtoPuff.Editor.Schema
{
    [Serializable]
    public struct StructDefinition
    {
        public string Name;
        public FieldDefinition[] Fields;
        public byte[] ReservedIds;
    }

    [Serializable]
    public struct FieldDefinition
    {
        public byte Id;
        public string Name;
        public string Type;
        public bool IsRepeated;

        public FieldDefinition(byte id, string name, string type, bool isRepeated)
        {
            Id = id;
            Name = name;
            Type = type;
            IsRepeated = isRepeated;
        }
    }
}