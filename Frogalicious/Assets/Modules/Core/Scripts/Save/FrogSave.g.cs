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
            if (data.ChapterIdx != default)
            {
                buf.Prepend(data.ChapterIdx, ref pos);
                buf.Prepend(ValueQualifier.PackedI32, ref pos);
                buf.Prepend((byte)0, ref pos);
            }

            if (data.LevelIdx != default)
            {
                buf.Prepend(data.LevelIdx, ref pos);
                buf.Prepend(ValueQualifier.PackedI32, ref pos);
                buf.Prepend((byte)1, ref pos);
            }

        }

        public static void SerialiseTo(this in FrogSave self, BinaryWriter bw)
        {
            var len = self.GetSerialisedSize();
            var prefixedLen = 1 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            bw.BaseStream.SetLength(prefixedLen);
            bw.BaseStream.Position = prefixedLen;

            bw.Prepend(self);
            bw.PrependLenPrefix(len, out var lps);
            bw.Prepend(ValueQualifier.Struct(lps).Pack());

            Debug.Assert(bw.BaseStream.Position == 0, $"{bw.BaseStream.Position} != 0");
        }

        public static void Prepend(this BinaryWriter bw, in FrogSave data)
        {
            if (data.ChapterIdx != default)
            {
                bw.Prepend(data.ChapterIdx);
                bw.Prepend(ValueQualifier.PackedI32);
                bw.Prepend((byte)0);
            }

            if (data.LevelIdx != default)
            {
                bw.Prepend(data.LevelIdx);
                bw.Prepend(ValueQualifier.PackedI32);
                bw.Prepend((byte)1);
            }

        }

        public static void DeserialiseFrom(this ref FrogSave self, in RefList<byte> buf)
        {
            var pos = 0;
            var vq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = pos + buf.ReadLenPrefix(vq.LenPrefixSize, ref pos);
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

        public static void DeserialiseFrom(this ref FrogSave self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.BaseStream.Position + br.ReadLenPrefix(vq.LenPrefixSize);
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
