using System;
using System.IO;
using System.Text;
using Frog.Collections;
using UnityEngine;

namespace Frog.ProtoPuff.Editor.Lexer
{
    internal static class ProtoPuffLexer
    {
        [NoCopyReturn]
        public static RefList<Token> Read(Stream input, int bufferSize = 1024)
        {
            var state = new LexerState();
            var tokens = RefList.WithCapacity<Token>(16);

            var buffer = new byte[bufferSize];
            var offset = 0;
            int bytesRead;

            while ((bytesRead = input.Read(buffer, offset, buffer.Length - offset)) > 0)
            {
                var len = offset + bytesRead;
                var span = buffer.AsSpan(0, len);
                var bytesProcessed = ReadTokens(ref state, ref tokens, span, offset);

                var reminder = len - bytesProcessed;
                Array.Copy(buffer, bytesProcessed, buffer, 0, reminder);
                offset = reminder;

                if (offset == buffer.Length)
                    Array.Resize(ref buffer, buffer.Length * 2);
            }

            if (offset != 0)
            {
                TerminateDanglingBlock(ref state, ref tokens, buffer.AsSpan(0, offset));
            }

            return tokens;
        }

        private static int ReadTokens(ref LexerState state, ref RefList<Token> tokens, in ReadOnlySpan<byte> span, int offset)
        {
            var processed = 0;
            var blockStartedAt = 0;

            for (var i = offset; i < span.Length; i++)
            {
                var value = span[i];
                switch (state.CurrBlock)
                {
                    case LexerBlock.None:
                        if (!TryStartBlock(ref state, ref tokens, value))
                        {
                            processed++;
                        }
                        else
                        {
                            blockStartedAt = i;
                        }
                        break;

                    case LexerBlock.Commentary:
                        if (!TryExtendCommetnary(ref state, value))
                        {
                            Debug.Assert(state.CurrBlock == LexerBlock.None);
                            goto case LexerBlock.None;
                        }

                        processed++;
                        break;

                    case LexerBlock.NameIdentifier:
                        if (!TryExtendNameIdentifier(ref state, ref tokens, value))
                        {
                            var blockLen = i - blockStartedAt;
                            processed += blockLen;
                            SetLastTokenValue(ref tokens, span.Slice(blockStartedAt, blockLen));

                            Debug.Assert(state.CurrBlock == LexerBlock.None);
                            goto case LexerBlock.None;
                        }
                        break;

                    case LexerBlock.Number:
                        if (!TryExtendNumber(ref state, ref tokens, value))
                        {
                            var blockLen = i - blockStartedAt;
                            processed += blockLen;
                            SetLastTokenValue(ref tokens, span.Slice(blockStartedAt, blockLen));

                            Debug.Assert(state.CurrBlock == LexerBlock.None);
                            goto case LexerBlock.None;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (value == '\n')
                {
                    state.CurrLocation.Line++;
                    state.CurrLocation.Column = 0;
                }
                else
                {
                    state.CurrLocation.Column++;
                }
            }

            return processed;
        }

        private static void SetLastTokenValue(ref RefList<Token> tokens, in ReadOnlySpan<byte> span)
        {
            ref var token = ref tokens.RefAt(tokens.Count() - 1);
            token.OptValue = Encoding.ASCII.GetString(span);
        }

        private static bool TryStartBlock(ref LexerState state, ref RefList<Token> tokens, byte value)
        {
            switch (value)
            {
                // Skip whitespaces & newlines
                case (byte)' ':
                case (byte)'\t':
                case (byte)'\n':
                {
                    return false;
                }

                // Trivial tokens
                case (byte)':':
                {
                    AddToken(ref tokens, TokenType.Colon, state.CurrLocation);
                    return false;
                }
                case (byte)';':
                {
                    AddToken(ref tokens, TokenType.Semicolon, state.CurrLocation);
                    return false;
                }
                case (byte)'=':
                {
                    AddToken(ref tokens, TokenType.Assignment, state.CurrLocation);
                    return false;
                }
                case (byte)'{':
                {
                    AddToken(ref tokens, TokenType.CurlyBraceLeft, state.CurrLocation);
                    return false;
                }
                case (byte)'}':
                {
                    AddToken(ref tokens, TokenType.CurlyBraceRight, state.CurrLocation);
                    return false;
                }

                // Commentary
                case (byte)'#':
                {
                    state.CurrBlock = LexerBlock.Commentary;
                    state.CurrBlockLocation = state.CurrLocation;
                    return true;
                }

                // Identifier
                case >= (byte)'a' and <= (byte)'z':
                case >= (byte)'A' and <= (byte)'Z':
                case (byte)'_':
                {
                    state.CurrBlock = LexerBlock.NameIdentifier;
                    state.CurrBlockLocation = state.CurrLocation;
                    return true;
                }

                // Number
                case >= (byte)'0' and <= (byte)'9':
                case (byte)'-':
                {
                    state.CurrBlock = LexerBlock.Number;
                    state.CurrBlockLocation = state.CurrLocation;
                    return true;
                }

                default:
                {
                    throw new CodeGenException($"Unexpected character '{(char)value}' at {state.CurrLocation}");
                }
            }
        }

        private static bool TryExtendCommetnary(ref LexerState state, byte value)
        {
            if (value == '\n')
            {
                state.CurrBlock = LexerBlock.None;
                state.CurrBlockLocation = default;
                return false;
            }

            return true;
        }

        private static bool TryExtendNumber(ref LexerState state, ref RefList<Token> tokens, byte value)
        {
            switch (value)
            {
                case >= (byte)'0' and <= (byte)'9':
                {
                    return true;
                }

                // Do not mix with names
                case >= (byte)'a' and <= (byte)'z':
                case >= (byte)'A' and <= (byte)'Z':
                case  (byte)'_' or (byte)'-':
                {
                    throw new CodeGenException($"Unexpected character '{(char)value}' at {state.CurrLocation}");
                }

                default:
                {
                    AddToken(ref tokens, TokenType.Number, state.CurrBlockLocation);
                    state.CurrBlock = LexerBlock.None;
                    state.CurrBlockLocation = default;
                    return false;
                }
            }
        }

        private static bool TryExtendNameIdentifier(ref LexerState state, ref RefList<Token> tokens, byte value)
        {
            switch (value)
            {
                case >= (byte)'a' and <= (byte)'z':
                case >= (byte)'A' and <= (byte)'Z':
                case >= (byte)'0' and <= (byte)'9':
                case (byte)'_':
                {
                    return true;
                }

                // Do not mix with numbers
                case (byte)'-':
                {
                    throw new CodeGenException($"Unexpected character '{(char)value}' at {state.CurrLocation}");
                }

                default:
                {
                    AddToken(ref tokens, TokenType.Identifier, state.CurrBlockLocation);
                    state.CurrBlock = LexerBlock.None;
                    state.CurrBlockLocation = default;
                    return false;
                }
            }
        }

        private static void AddToken(ref RefList<Token> tokens, TokenType type, in Location location)
        {
            ref var token = ref tokens.RefAdd();
            token.Type = type;
            token.Location = location;
        }

        private static void TerminateDanglingBlock(ref LexerState state, ref RefList<Token> tokens, in ReadOnlySpan<byte> span)
        {
            switch (state.CurrBlock)
            {
                case LexerBlock.None:
                    throw new CodeGenException("Internal lexer error: dangling none-block");

                case LexerBlock.Commentary:
                    break;

                case LexerBlock.NameIdentifier:
                case LexerBlock.Number:
                    ref var token = ref tokens.RefAdd();
                    token.Type = state.CurrBlock == LexerBlock.NameIdentifier ? TokenType.Identifier : TokenType.Number;
                    token.Location = state.CurrLocation;
                    token.OptValue = Encoding.ASCII.GetString(span);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}