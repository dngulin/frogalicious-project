using System;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Lexer;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor.Parser
{
    internal static class ProtoPuffParser
    {
        [NoCopyReturn]
        public static PuffSchema Parse(in RefList<Token> tokens)
        {
            var schema = new PuffSchema();
            var state = new ParserState();

            var tokenIdx = 0;
            while (tokenIdx < tokens.Count())
            {
                ReadTokens(ref state, ref schema, tokens, ref tokenIdx);
            }

            return schema;
        }

        private static void ReadTokens(
            ref ParserState state, ref PuffSchema schema,
            in RefList<Token> tokens, ref int tokenIdx)
        {
            switch (state.ParsingTypeKind)
            {
                case ParsingTypeKind.None:
                    BeginType(ref state, ref schema, tokens, ref tokenIdx);
                    break;

                case ParsingTypeKind.Enum:
                    UpdateEnum(ref state, ref schema, tokens, ref tokenIdx);
                    break;

                case ParsingTypeKind.Struct:
                    UpdateStruct(ref state, ref schema, tokens, ref tokenIdx);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void BeginType(
            ref ParserState state, ref PuffSchema schema,
            in RefList<Token> tokens, ref int tokenIdx)
        {
            ref readonly var keyword = ref ReadIdentifierToken(tokens, tokenIdx++);
            switch (keyword.OptValue)
            {
                case "enum":
                {
                    state.ParsingTypeKind = ParsingTypeKind.Enum;

                    ref var puffEnum = ref schema.Enums.RefAdd();
                    puffEnum.Name = ReadTypeName(schema, tokens, tokenIdx++);

                    EnsureToken(TokenType.Colon, tokens, tokenIdx++);

                    var token = ReadIdentifierToken(tokens, tokenIdx++);
                    if (token.OptValue == "flags")
                    {
                        puffEnum.IsFlags = true;
                        token = ReadIdentifierToken(tokens, tokenIdx++);
                    }

                    puffEnum.UnderlyingType = GetEnumType(token);
                    break;
                }

                case "struct":
                {
                    state.ParsingTypeKind = ParsingTypeKind.Struct;
                    ref var puffStruct = ref schema.Structs.RefAdd();
                    puffStruct.Name = ReadTypeName(schema, tokens, tokenIdx++);
                    break;
                }

                default:
                    throw new CodeGenException($"Unexpected keyword '{keyword.OptValue}' at {keyword.Location}");
            }

            EnsureToken(TokenType.CurlyBraceLeft, tokens, tokenIdx++);
        }

        private static void UpdateEnum(
            ref ParserState state, ref PuffSchema schema,
            in RefList<Token> tokens, ref int tokenIdx)
        {
            if (tokens.RefReadonlyAt(tokenIdx).Type == TokenType.CurlyBraceRight)
            {
                tokenIdx++;
                state.ParsingTypeKind = ParsingTypeKind.None;
                return;
            }

            ref var puffEnum = ref schema.Enums.RefAt(schema.Enums.Count() - 1);

            var itemName = ReadEnumItemName(puffEnum.Items, tokens, tokenIdx);
            ref var puffItem = ref puffEnum.Items.RefAdd();
            puffItem.Name = itemName;

            EnsureToken(TokenType.Assignment, tokens, tokenIdx++);

            puffItem.Value = ReadNumberToken(puffEnum.UnderlyingType, tokens, tokenIdx++).OptValue;

            EnsureToken(TokenType.Semicolon, tokens, tokenIdx++);
        }

        private static void UpdateStruct(
            ref ParserState state, ref PuffSchema schema,
            in RefList<Token> tokens, ref int tokenIdx)
        {
            if (tokens.RefReadonlyAt(tokenIdx).Type == TokenType.CurlyBraceRight)
            {
                tokenIdx++;
                state.ParsingTypeKind = ParsingTypeKind.None;
                return;
            }

            ref var puffStruct = ref schema.Structs.RefAt(schema.Structs.Count() - 1);

            var filedId = ReadFieldId(puffStruct.Fields, tokens, tokenIdx++);
            var fieldName = ReadFieldName(puffStruct.Fields, tokens, tokenIdx++);

            ref var puffField = ref puffStruct.Fields.RefAdd();
            puffField.Id = filedId;
            puffField.Name = fieldName;

            EnsureToken(TokenType.Colon, tokens, tokenIdx++);

            var token = ReadIdentifierToken(tokens, tokenIdx++);
            if (token.OptValue == "repeated")
            {
                puffField.IsRepeated = true;
                token = ReadIdentifierToken(tokens, tokenIdx++);
            }

            if (!IsTypeKnown(token.OptValue, schema))
            {
                throw new CodeGenException($"Unknown type '{token.OptValue}' at {token.Location}");
            }

            puffField.Type = token.OptValue;

            EnsureToken(TokenType.Semicolon, tokens, tokenIdx++);
        }

        private static string ReadTypeName(in PuffSchema schema, in RefList<Token> tokens, int idx)
        {
            var token = ReadIdentifierToken(tokens, idx);

            if (IsTypeKnown(token.OptValue, schema))
            {
                throw new CodeGenException($"Already known type '{token.OptValue}' declared at {token.Location}");
            }

            return token.OptValue;
        }

        private static string ReadEnumItemName(in RefList<PuffEnumItem> items, in RefList<Token> tokens, int idx)
        {
            var t = ReadIdentifierToken(tokens, idx);

            foreach (ref readonly var item in items.RefReadonlyIter())
            {
                if (item.Name == t.OptValue)
                    throw new CodeGenException($"Duplicate enum item name at {t.Location}");
            }

            return t.OptValue;
        }

        private static string ReadFieldName(in RefList<PuffField> fields, in RefList<Token> tokens, int idx)
        {
            var t = ReadIdentifierToken(tokens, idx);

            foreach (ref readonly var field in fields.RefReadonlyIter())
            {
                if (field.Name == t.OptValue)
                    throw new CodeGenException($"Duplicate field name '{t.OptValue}' at {t.Location}");
            }

            return t.OptValue;
        }

        private static byte ReadFieldId(in RefList<PuffField> fields, in RefList<Token> tokens, int idx)
        {
            var t = ReadNumberToken(Primitive.U8, tokens, idx);

            var id = byte.Parse(t.OptValue);

            foreach (ref readonly var field in fields.RefReadonlyIter())
            {
                if (field.Id == id)
                    throw new CodeGenException($"Duplicate field id {id} at {t.Location}");
            }

            return id;
        }

        private static ref readonly Token ReadIdentifierToken(in RefList<Token> tokens, int idx)
        {
            if (idx >= tokens.Count())
                throw new CodeGenException($"Unexpected end of file (expected {TokenType.Identifier})");

            ref readonly var t = ref tokens.RefReadonlyAt(idx);

            if (t.Type != TokenType.Identifier)
                throw new CodeGenException($"Unexpected {t.Type} at {t.Location} (expected {TokenType.Identifier})");

            if (t.OptValue == null)
                throw new CodeGenException($"Internal error: missing token value at {t.Location}");

            return ref t;
        }

        private static ref readonly Token ReadNumberToken(Primitive p, in RefList<Token> tokens, int idx)
        {
            if (idx >= tokens.Count())
                throw new CodeGenException($"Unexpected end of file (expected {TokenType.Number})");

            ref readonly var t = ref tokens.RefReadonlyAt(idx);

            if (t.Type != TokenType.Number)
                throw new CodeGenException($"Unexpected {t.Type} at {t.Location} (expected {TokenType.Number})");

            if (!p.CanBeParsedFrom(t.OptValue))
                throw new CodeGenException($"Invalid enum value at {t.Location}");

            return ref t;
        }

        private static void EnsureToken(TokenType type, in RefList<Token> tokens, int idx)
        {
            if (idx >= tokens.Count())
                throw new CodeGenException($"Unexpected end of file (expected {type})");

            ref readonly var t = ref tokens.RefReadonlyAt(idx);
            if (t.Type != type)
                throw new CodeGenException($"Unexpected {t.Type} at {t.Location} (expected {type})");
        }

        private static Primitive GetEnumType(in Token token)
        {
            return token.OptValue switch
            {
                "i8" => Primitive.I8,
                "i16" => Primitive.I16,
                "i32" => Primitive.I32,
                "i64" => Primitive.I64,
                "u8" => Primitive.U8,
                "u16" => Primitive.U16,
                "u32" => Primitive.U32,
                "u64" => Primitive.U64,
                _ => throw new CodeGenException($"Unexpected enum type '{token.OptValue}' at {token.Location}")
            };
        }

        private static bool IsPrimitiveType(string type)
        {
            return type switch
            {
                "i8" => true,
                "i16" => true,
                "i32" => true,
                "i64" => true,
                "u8" => true,
                "u16" => true,
                "u32" => true,
                "u64" => true,
                "f32" => true,
                "f64" => true,
                "bool" => true,
                _ => false
            };
        }

        private static bool IsTypeKnown(string type, in PuffSchema schema)
        {
            foreach (ref readonly var puffEnum in schema.Enums.RefReadonlyIter())
            {
                if (puffEnum.Name == type)
                    return true;
            }

            foreach (ref readonly var puffStruct in schema.Structs.RefReadonlyIter())
            {
                if (puffStruct.Name == type)
                    return true;
            }

            return IsPrimitiveType(type);
        }
    }
}