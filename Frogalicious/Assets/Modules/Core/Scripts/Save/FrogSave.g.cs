using System;
using System.IO;
using UnityEngine;
using Frog.Collections;
using Frog.ProtoPuff;

namespace Frog.Core.Save
{
    public enum LanguageSetting : ushort
    {
        Detect = 0,
        English = 1,
        Russian = 2,
    }

    public struct FrogSave
    {
        public int ChapterIdx;
        public int LevelIdx;
        public LanguageSetting Language;

        public static bool operator ==(in FrogSave l, in FrogSave r)
        {
            if (l.ChapterIdx != r.ChapterIdx) return false;
            if (l.LevelIdx != r.LevelIdx) return false;
            if (l.Language != r.Language) return false;

            return true;
        }

        public static bool operator !=(in FrogSave l, in FrogSave r) => !(l == r);

        public override bool Equals(object other) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public static class ProtoPuffExtensions_FrogSave
    {
        public static void Clear(ref this FrogSave self)
        {
            self.ChapterIdx = default;
            self.LevelIdx = default;
            self.Language = default;
        }

        public static int GetSerialisedSize(this in FrogSave self)
        {
            var result = 0;

            if (self.ChapterIdx != default)
            {
                result += 2 + sizeof(int);
            }

            if (self.LevelIdx != default)
            {
                result += 2 + sizeof(int);
            }

            if (self.Language != default)
            {
                result += 2 + sizeof(ushort);
            }

            return result;
        }

        public static void SerialiseTo(this in FrogSave self, BinaryWriter bw)
        {
            var len = self.GetSerialisedSize();
            var prefixedLen = 1 + ProtoPuffUtil.GetLenPrefixSize(len) + len;

            var prePos = bw.BaseStream.Position;
            var endPos = prePos + prefixedLen;
            if (endPos > bw.BaseStream.Length)
            {
                bw.BaseStream.SetLength(endPos);
            }
            bw.BaseStream.Position = endPos;

            bw.Prepend(self);
            bw.PrependLenPrefix(len, out var lps);
            bw.Prepend(ValueQualifier.Struct(lps).Pack());

            Debug.Assert(bw.BaseStream.Position == prePos, $"Stream position doesn't match the data length");
            bw.BaseStream.Position = endPos;
        }

        public static void Prepend(this BinaryWriter bw, in FrogSave data)
        {
            if (data.Language != default)
            {
                bw.Prepend((ushort)data.Language);
                bw.Prepend(ValueQualifier.PackedU16);
                bw.Prepend((byte)2);
            }

            if (data.LevelIdx != default)
            {
                bw.Prepend(data.LevelIdx);
                bw.Prepend(ValueQualifier.PackedI32);
                bw.Prepend((byte)1);
            }

            if (data.ChapterIdx != default)
            {
                bw.Prepend(data.ChapterIdx);
                bw.Prepend(ValueQualifier.PackedI32);
                bw.Prepend((byte)0);
            }

        }

        public static void DeserialiseFrom(this ref FrogSave self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.ReadLenPrefix(vq.LenPrefixSize) + br.BaseStream.Position;
            self.UpdateValueFrom(br, endPos);
        }

        public static void UpdateValueFrom(this ref FrogSave self, BinaryReader br, long endPos)
        {
            while (br.BaseStream.Position < endPos)
            {
                var fieldId = br.ReadByte();
                switch (fieldId)
                {
                    case 0:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureI32(fvq);
                        self.ChapterIdx = br.ReadInt32();
                        break;
                    }
                    case 1:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureI32(fvq);
                        self.LevelIdx = br.ReadInt32();
                        break;
                    }
                    case 2:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureU16(fvq);
                        self.Language = (LanguageSetting)br.ReadUInt16();
                        break;
                    }
                    default:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        br.SkipValue(fvq);
                        break;
                    }
                }
            }
        }
    }

}
