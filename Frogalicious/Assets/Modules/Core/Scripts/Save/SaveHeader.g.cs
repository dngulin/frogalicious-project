using System;
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

        public static void SerialiseTo(this in MigrationInfo self, BinaryWriter bw)
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

        public static void Prepend(this BinaryWriter bw, in MigrationInfo data)
        {
            if (data.TimeStamp != default)
            {
                bw.Prepend(data.TimeStamp);
                bw.Prepend(ValueQualifier.PackedI64);
                bw.Prepend((byte)1);
            }

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

        }

        public static void DeserialiseFrom(this ref MigrationInfo self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.ReadLenPrefix(vq.LenPrefixSize) + br.BaseStream.Position;
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
                        var fieldEndPos = br.ReadLenPrefix(fvq.LenPrefixSize) + br.BaseStream.Position;
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
    public struct SaveHeader
    {
        public RefList<MigrationInfo> Migrations;

        public static bool operator ==(in SaveHeader l, in SaveHeader r)
        {
            if (l.Migrations.Count() != r.Migrations.Count()) return false;
            for (int i = 0; i < l.Migrations.Count(); i++)
            {
                if (l.Migrations.RefReadonlyAt(i) != r.Migrations.RefReadonlyAt(i)) return false;
            }

            return true;
        }

        public static bool operator !=(in SaveHeader l, in SaveHeader r) => !(l == r);

        public override bool Equals(object other) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public static class ProtoPuffExtensions_SaveHeader
    {
        public static void Clear(ref this SaveHeader self)
        {
            self.Migrations.Clear();
        }

        public static int GetSerialisedSize(this in SaveHeader self)
        {
            var result = 0;

            if (self.Migrations.Count() > 0)
            {
                var len = 0;
                for (int i = 0; i < self.Migrations.Count(); i++)
                {
                    var itemLen = self.Migrations.RefReadonlyAt(i).GetSerialisedSize();
                    len += 1 + ProtoPuffUtil.GetLenPrefixSize(itemLen) + itemLen;
                }
                result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            }

            return result;
        }

        public static void SerialiseTo(this in SaveHeader self, BinaryWriter bw)
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

        public static void Prepend(this BinaryWriter bw, in SaveHeader data)
        {
            if (data.Migrations.Count() > 0)
            {
                var posAfterField = bw.BaseStream.Position;
                for (int i = data.Migrations.Count() - 1; i >= 0 ; i--)
                {
                    var posAfterItem = bw.BaseStream.Position;
                    bw.Prepend(data.Migrations.RefReadonlyAt(i));
                    var itemLen = checked((int)(posAfterItem - bw.BaseStream.Position));
                    bw.PrependLenPrefix(itemLen, out var itemLps);
                    bw.Prepend(ValueQualifier.Struct(itemLps).Pack());
                }
                var len = checked((int)(posAfterField - bw.BaseStream.Position));
                bw.PrependLenPrefix(len, out var lps);
                bw.Prepend(ValueQualifier.RepeatedStruct(lps).Pack());
                bw.Prepend((byte)0);
            }

        }

        public static void DeserialiseFrom(this ref SaveHeader self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.ReadLenPrefix(vq.LenPrefixSize) + br.BaseStream.Position;
            self.UpdateValueFrom(br, endPos);
        }

        public static void UpdateValueFrom(this ref SaveHeader self, BinaryReader br, long endPos)
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
                        var fieldEndPos = br.ReadLenPrefix(fvq.LenPrefixSize) + br.BaseStream.Position;
                        while (br.BaseStream.Position < fieldEndPos)
                        {
                            var ivq = ValueQualifier.Unpack(br.ReadByte());
                            ProtoPuffUtil.EnsureStruct(ivq);
                            var itemEndPos = br.ReadLenPrefix(ivq.LenPrefixSize) + br.BaseStream.Position;
                            self.Migrations.RefAdd().UpdateValueFrom(br, itemEndPos);
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
