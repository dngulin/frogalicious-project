using System;

namespace Frog.ProtoPuff.Editor.Schema
{
    [Serializable]
    public struct EnumDefinition
    {
        public string Name;
        public string UnderlyingType;
        public bool Flags;
        public EnumItemDefinition[] Items;
    }

    [Serializable]
    public struct EnumItemDefinition
    {
        public string Name;
        public string Value;
    }
}