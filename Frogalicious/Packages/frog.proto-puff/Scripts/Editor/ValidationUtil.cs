using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    internal static class ValidationUtil
    {
        private static readonly Dictionary<string, Primitive> Primitives = new()
        {
            { "i8", Primitive.I8 },
            { "i16", Primitive.I16 },
            { "i32", Primitive.I32 },
            { "i64", Primitive.I64 },
            { "u8", Primitive.U8 },
            { "u16", Primitive.U16 },
            { "u32", Primitive.U32 },
            { "u64", Primitive.U64 },
            { "f32", Primitive.F32 },
            { "f64", Primitive.F64 },
            { "bool", Primitive.Bool },
        };

        private static readonly Regex SymbolNameRegEx = new(@"^[_a-zA-Z]\w*$");

        public static bool TryGetPrimitiveType(string name, out Primitive primitive)
        {
            return Primitives.TryGetValue(name, out primitive);
        }

        public static Primitive GetEnumUnderlyingType(string type)
        {
            if (!Primitives.TryGetValue(type, out var primitive) || !primitive.CanBeEnumBaseType())
                throw new Exception($"Invalid enum base type `{type}`");

            return primitive;
        }

        public static void ValidateEnumItemName(string name, in EnumDefinition enumDef)
        {
            if (!SymbolNameRegEx.IsMatch(name))
                throw new Exception($"Invalid enum item name `{enumDef.Name}.{name}`");

            if (enumDef.Items.Count(i => i.Name == name) > 1)
                throw new Exception($"Enum item `{enumDef.Name}.{name}` is defined more than once");
        }

        public static void ValidateEnumItemValue(in EnumItemDefinition item, in EnumDefinition enumDef, Primitive t)
        {
            if (!t.CanBeParsedFrom(item.Value))
                throw new Exception($"Enum item value `{enumDef.Name}.{item.Name}` cant be set from `{item.Value}`");
        }

        public static void ValidateFieldName(string name, in StructDefinition structDef)
        {
            if (!SymbolNameRegEx.IsMatch(name))
                throw new Exception($"Invalid struct field name `{structDef.Name}.{name}`");

            if (structDef.Fields.Count(i => i.Name == name) > 1)
                throw new Exception($"Struct field `{structDef.Name}.{name}` is defined more than once");
        }
    }
}