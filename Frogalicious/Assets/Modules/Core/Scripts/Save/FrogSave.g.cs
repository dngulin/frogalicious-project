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

        public static void SerialiseTo(this in FrogSave self, ref RefList<byte> buf)
        {
            var len = self.GetSerialisedSize();
            buf.SetSize(1 + ProtoPuffUtil.GetLenPrefixSize(len) + len);

            var pos = buf.Count();
            buf.Prepend(self, ref pos);
            buf.PrependLenPrefix(len, ref pos, out var lps);
            buf.Prepend(ValueQualifier.Struct(lps).Pack(), ref pos);

            Debug.Assert(pos == 0, $"{pos} != 0");
        }

        public static void Prepend(this ref RefList<byte> buf, in FrogSave data, ref int pos)
        {
            if (data.LevelIdx != default)
            {
                buf.Prepend(data.LevelIdx, ref pos);
                buf.Prepend(ValueQualifier.PackedI32, ref pos);
                buf.Prepend((byte)1, ref pos);
            }

            if (data.ChapterIdx != default)
            {
                buf.Prepend(data.ChapterIdx, ref pos);
                buf.Prepend(ValueQualifier.PackedI32, ref pos);
                buf.Prepend((byte)0, ref pos);
            }

        }

        public static void DeserialiseFrom(this ref FrogSave self, in RefList<byte> buf)
        {
            var pos = 0;
            var vq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = buf.ReadLenPrefix(vq.LenPrefixSize, ref pos) + pos;
            self.UpdateValueFrom(buf, ref pos, endPos);
        }

        public static void UpdateValueFrom(this ref FrogSave self, in RefList<byte> buf, ref int pos, int endPos)
        {
            while (pos < endPos)
            {
                var fieldId = buf.ReadByte(ref pos);
                switch (fieldId)
                {
                    case 0:
                    {
                        var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                        ProtoPuffUtil.EnsureI32(fvq);
                        self.ChapterIdx = buf.ReadInt32(ref pos);
                        break;
                    }
                    case 1:
                    {
                        var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                        ProtoPuffUtil.EnsureI32(fvq);
                        self.LevelIdx = buf.ReadInt32(ref pos);
                        break;
                    }
                    default:
                    {
                        var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                        buf.SkipValue(fvq, ref pos);
                        break;
                    }
                }
            }
        }
    }

}
