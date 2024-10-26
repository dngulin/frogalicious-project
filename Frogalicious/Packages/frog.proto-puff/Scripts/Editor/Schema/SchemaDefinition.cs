using System;

namespace Frog.ProtoPuff.Editor.Schema
{
    [Serializable]
    public struct SchemaDefinition
    {
        public string Namespace;
        public EnumDefinition[] Enums;
        public StructDefinition[] Structs;
    }
}