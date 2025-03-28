using System;
using System.IO;
using UnityEngine;
using Frog.Collections;
using Frog.ProtoPuff;

namespace Frog.Core.Save
{
    public struct FrogSave
    {
        public int ChapterIdx;
        public int LevelIdx;

        public static bool operator ==(in FrogSave l, in FrogSave r)
        {
            if (l.ChapterIdx != r.ChapterIdx) return false;
            if (l.LevelIdx != r.LevelIdx) return false;

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

            return result;
        }

        public static void SerialiseTo(this in FrogSave self, BinaryWriter bw)
        {
            var len = self.GetSerialisedSize();
            var prefixedLen = 1 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            if (bw.BaseStream.Position + prefixedLen > bw.BaseStream.Length)
            {
                bw.BaseStream.SetLength(bw.BaseStream.Position + prefixedLen);
            }
            bw.BaseStream.Position += prefixedLen;

            bw.Prepend(self);
            bw.PrependLenPrefix(len, out var lps);
            bw.Prepend(ValueQualifier.Struct(lps).Pack());

            Debug.Assert(bw.BaseStream.Position == 0, $"{bw.BaseStream.Position} != 0");
        }

        public static void Prepend(this BinaryWriter bw, in FrogSave data)
        {
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
