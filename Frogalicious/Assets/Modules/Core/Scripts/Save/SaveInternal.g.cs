﻿using System;
using System.IO;
using UnityEngine;
using Frog.Collections;
using Frog.ProtoPuff;

namespace Frog.Core.Save
{
    [NoCopy]
    public struct MigrationInfo
    {
        public RefList<byte> Name;
        public long TimeStamp;

        public static bool operator ==(in MigrationInfo l, in MigrationInfo r)
        {
            if (l.Name.Count() != r.Name.Count()) return false;
            for (int i = 0; i < l.Name.Count(); i++)
            {
                if (l.Name.RefReadonlyAt(i) != r.Name.RefReadonlyAt(i)) return false;
            }
            if (l.TimeStamp != r.TimeStamp) return false;

            return true;
        }

        public static bool operator !=(in MigrationInfo l, in MigrationInfo r) => !(l == r);

        public override bool Equals(object other) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public static class ProtoPuffExtensions_MigrationInfo
    {
        public static void Clear(ref this MigrationInfo self)
        {
            self.Name.Clear();
            self.TimeStamp = default;
        }

        public static int GetSerialisedSize(this in MigrationInfo self)
        {
            var result = 0;

            if (self.Name.Count() > 0)
            {
                var len = sizeof(byte) * self.Name.Count();
                result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            }

            if (self.TimeStamp != default)
            {
                result += 2 + sizeof(long);
            }

            return result;
        }

        public static void SerialiseTo(this in MigrationInfo self, ref RefList<byte> buf)
        {
            var len = self.GetSerialisedSize();
            buf.SetSize(1 + ProtoPuffUtil.GetLenPrefixSize(len) + len);

            var pos = buf.Count();
            buf.Prepend(self, ref pos);
            buf.PrependLenPrefix(len, ref pos, out var lps);
            buf.Prepend(ValueQualifier.Struct(lps).Pack(), ref pos);

            Debug.Assert(pos == 0, $"{pos} != 0");
        }

        public static void Prepend(this ref RefList<byte> buf, in MigrationInfo data, ref int pos)
        {
            if (data.Name.Count() > 0)
            {
                var posAfterField = pos;
                for (int i = data.Name.Count() - 1; i >= 0 ; i--)
                {
                    buf.Prepend(data.Name.RefReadonlyAt(i), ref pos);
                }
                var len = posAfterField - pos;
                buf.PrependLenPrefix(len, ref pos, out var lps);
                buf.Prepend(ValueQualifier.RepeatedU8(lps).Pack(), ref pos);
                buf.Prepend((byte)0, ref pos);
            }

            if (data.TimeStamp != default)
            {
                buf.Prepend(data.TimeStamp, ref pos);
                buf.Prepend(ValueQualifier.PackedI64, ref pos);
                buf.Prepend((byte)1, ref pos);
            }

        }

        public static void SerialiseTo(this in MigrationInfo self, BinaryWriter bw)
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

        public static void Prepend(this BinaryWriter bw, in MigrationInfo data)
        {
            if (data.Name.Count() > 0)
            {
                var posAfterField = bw.BaseStream.Position;
                for (int i = data.Name.Count() - 1; i >= 0 ; i--)
                {
                    bw.Prepend(data.Name.RefReadonlyAt(i));
                }
                var len = checked((int)(posAfterField - bw.BaseStream.Position));
                bw.PrependLenPrefix(len, out var lps);
                bw.Prepend(ValueQualifier.RepeatedU8(lps).Pack());
                bw.Prepend((byte)0);
            }

            if (data.TimeStamp != default)
            {
                bw.Prepend(data.TimeStamp);
                bw.Prepend(ValueQualifier.PackedI64);
                bw.Prepend((byte)1);
            }

        }

        public static void DeserialiseFrom(this ref MigrationInfo self, in RefList<byte> buf)
        {
            var pos = 0;
            var vq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = pos + buf.ReadLenPrefix(vq.LenPrefixSize, ref pos);
            self.UpdateValueFrom(buf, ref pos, endPos);
        }

        public static void UpdateValueFrom(this ref MigrationInfo self, in RefList<byte> buf, ref int pos, int endPos)
        {
            while (pos < endPos)
            {
                var fieldId = buf.ReadByte(ref pos);
                switch (fieldId)
                {
                    case 0:
                    {
                        var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                        ProtoPuffUtil.EnsureRepeatedU8(fvq);
                        var fieldEndPos = pos + buf.ReadLenPrefix(fvq.LenPrefixSize, ref pos);
                        while (pos < fieldEndPos)
                        {
                            self.Name.RefAdd() = buf.ReadByte(ref pos);
                        }
                        break;
                    }
                    case 1:
                    {
                        var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                        ProtoPuffUtil.EnsureI64(fvq);
                        self.TimeStamp = buf.ReadInt64(ref pos);
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

        public static void DeserialiseFrom(this ref MigrationInfo self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.BaseStream.Position + br.ReadLenPrefix(vq.LenPrefixSize);
            self.UpdateValueFrom(br, endPos);
        }

        public static void UpdateValueFrom(this ref MigrationInfo self, BinaryReader br, long endPos)
        {
            while (br.BaseStream.Position < endPos)
            {
                var fieldId = br.ReadByte();
                switch (fieldId)
                {
                    case 0:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureRepeatedU8(fvq);
                        var fieldEndPos = br.BaseStream.Position + br.ReadLenPrefix(fvq.LenPrefixSize);
                        while (br.BaseStream.Position < fieldEndPos)
                        {
                            self.Name.RefAdd() = br.ReadByte();
                        }
                        break;
                    }
                    case 1:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureI64(fvq);
                        self.TimeStamp = br.ReadInt64();
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

    [NoCopy]
    public struct SaveInternal
    {
        public RefList<MigrationInfo> Migrations;
        public RefList<byte> Data;

        public static bool operator ==(in SaveInternal l, in SaveInternal r)
        {
            if (l.Migrations.Count() != r.Migrations.Count()) return false;
            for (int i = 0; i < l.Migrations.Count(); i++)
            {
                if (l.Migrations.RefReadonlyAt(i) != r.Migrations.RefReadonlyAt(i)) return false;
            }
            if (l.Data.Count() != r.Data.Count()) return false;
            for (int i = 0; i < l.Data.Count(); i++)
            {
                if (l.Data.RefReadonlyAt(i) != r.Data.RefReadonlyAt(i)) return false;
            }

            return true;
        }

        public static bool operator !=(in SaveInternal l, in SaveInternal r) => !(l == r);

        public override bool Equals(object other) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public static class ProtoPuffExtensions_SaveInternal
    {
        public static void Clear(ref this SaveInternal self)
        {
            self.Migrations.Clear();
            self.Data.Clear();
        }

        public static int GetSerialisedSize(this in SaveInternal self)
        {
            var result = 0;

            if (self.Migrations.Count() > 0)
            {
                var len = 0;
                for (int i = 0; i < self.Migrations.Count(); i++)
                {
                    var itemLen = self.Migrations.RefReadonlyAt(i).GetSerialisedSize();
                    len += 1 + ProtoPuffUtil.GetLenPrefixSize(len) + itemLen;
                }
                result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            }

            if (self.Data.Count() > 0)
            {
                var len = sizeof(byte) * self.Data.Count();
                result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            }

            return result;
        }

        public static void SerialiseTo(this in SaveInternal self, ref RefList<byte> buf)
        {
            var len = self.GetSerialisedSize();
            buf.SetSize(1 + ProtoPuffUtil.GetLenPrefixSize(len) + len);

            var pos = buf.Count();
            buf.Prepend(self, ref pos);
            buf.PrependLenPrefix(len, ref pos, out var lps);
            buf.Prepend(ValueQualifier.Struct(lps).Pack(), ref pos);

            Debug.Assert(pos == 0, $"{pos} != 0");
        }

        public static void Prepend(this ref RefList<byte> buf, in SaveInternal data, ref int pos)
        {
            if (data.Migrations.Count() > 0)
            {
                var posAfterField = pos;
                for (int i = data.Migrations.Count() - 1; i >= 0 ; i--)
                {
                    buf.Prepend(data.Migrations.RefReadonlyAt(i), ref pos);
                }
                var len = posAfterField - pos;
                buf.PrependLenPrefix(len, ref pos, out var lps);
                buf.Prepend(ValueQualifier.RepeatedStruct(lps).Pack(), ref pos);
                buf.Prepend((byte)0, ref pos);
            }

            if (data.Data.Count() > 0)
            {
                var posAfterField = pos;
                for (int i = data.Data.Count() - 1; i >= 0 ; i--)
                {
                    buf.Prepend(data.Data.RefReadonlyAt(i), ref pos);
                }
                var len = posAfterField - pos;
                buf.PrependLenPrefix(len, ref pos, out var lps);
                buf.Prepend(ValueQualifier.RepeatedU8(lps).Pack(), ref pos);
                buf.Prepend((byte)1, ref pos);
            }

        }

        public static void SerialiseTo(this in SaveInternal self, BinaryWriter bw)
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

        public static void Prepend(this BinaryWriter bw, in SaveInternal data)
        {
            if (data.Migrations.Count() > 0)
            {
                var posAfterField = bw.BaseStream.Position;
                for (int i = data.Migrations.Count() - 1; i >= 0 ; i--)
                {
                    bw.Prepend(data.Migrations.RefReadonlyAt(i));
                }
                var len = checked((int)(posAfterField - bw.BaseStream.Position));
                bw.PrependLenPrefix(len, out var lps);
                bw.Prepend(ValueQualifier.RepeatedStruct(lps).Pack());
                bw.Prepend((byte)0);
            }

            if (data.Data.Count() > 0)
            {
                var posAfterField = bw.BaseStream.Position;
                for (int i = data.Data.Count() - 1; i >= 0 ; i--)
                {
                    bw.Prepend(data.Data.RefReadonlyAt(i));
                }
                var len = checked((int)(posAfterField - bw.BaseStream.Position));
                bw.PrependLenPrefix(len, out var lps);
                bw.Prepend(ValueQualifier.RepeatedU8(lps).Pack());
                bw.Prepend((byte)1);
            }

        }

        public static void DeserialiseFrom(this ref SaveInternal self, in RefList<byte> buf)
        {
            var pos = 0;
            var vq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = pos + buf.ReadLenPrefix(vq.LenPrefixSize, ref pos);
            self.UpdateValueFrom(buf, ref pos, endPos);
        }

        public static void UpdateValueFrom(this ref SaveInternal self, in RefList<byte> buf, ref int pos, int endPos)
        {
            while (pos < endPos)
            {
                var fieldId = buf.ReadByte(ref pos);
                switch (fieldId)
                {
                    case 0:
                    {
                        var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                        ProtoPuffUtil.EnsureRepeatedStruct(fvq);
                        var fieldEndPos = pos + buf.ReadLenPrefix(fvq.LenPrefixSize, ref pos);
                        while (pos < fieldEndPos)
                        {
                            var ivq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                            ProtoPuffUtil.EnsureStruct(ivq);
                            var itemEndPos = pos + buf.ReadLenPrefix(ivq.LenPrefixSize, ref pos);
                            self.Migrations.RefAdd().UpdateValueFrom(buf, ref pos, itemEndPos);
                        }
                        break;
                    }
                    case 1:
                    {
                        var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));
                        ProtoPuffUtil.EnsureRepeatedU8(fvq);
                        var fieldEndPos = pos + buf.ReadLenPrefix(fvq.LenPrefixSize, ref pos);
                        while (pos < fieldEndPos)
                        {
                            self.Data.RefAdd() = buf.ReadByte(ref pos);
                        }
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

        public static void DeserialiseFrom(this ref SaveInternal self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.BaseStream.Position + br.ReadLenPrefix(vq.LenPrefixSize);
            self.UpdateValueFrom(br, endPos);
        }

        public static void UpdateValueFrom(this ref SaveInternal self, BinaryReader br, long endPos)
        {
            while (br.BaseStream.Position < endPos)
            {
                var fieldId = br.ReadByte();
                switch (fieldId)
                {
                    case 0:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureRepeatedStruct(fvq);
                        var fieldEndPos = br.BaseStream.Position + br.ReadLenPrefix(fvq.LenPrefixSize);
                        while (br.BaseStream.Position < fieldEndPos)
                        {
                            var ivq = ValueQualifier.Unpack(br.ReadByte());
                            ProtoPuffUtil.EnsureStruct(ivq);
                            var itemEndPos = br.BaseStream.Position + br.ReadLenPrefix(ivq.LenPrefixSize);
                            self.Migrations.RefAdd().UpdateValueFrom(br, itemEndPos);
                        }
                        break;
                    }
                    case 1:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureRepeatedU8(fvq);
                        var fieldEndPos = br.BaseStream.Position + br.ReadLenPrefix(fvq.LenPrefixSize);
                        while (br.BaseStream.Position < fieldEndPos)
                        {
                            self.Data.RefAdd() = br.ReadByte();
                        }
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