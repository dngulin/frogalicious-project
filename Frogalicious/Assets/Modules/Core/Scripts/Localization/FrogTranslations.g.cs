using System;
using System.IO;
using UnityEngine;
using Frog.Collections;
using Frog.ProtoPuff;

namespace Frog.Core.Save
{
    [NoCopy]
    public struct FrogTranslation
    {
        public uint TranslationIdHash;
        public byte PluralForm;
        public RefList<byte> Translation;

        public static bool operator ==(in FrogTranslation l, in FrogTranslation r)
        {
            if (l.TranslationIdHash != r.TranslationIdHash) return false;
            if (l.PluralForm != r.PluralForm) return false;
            if (l.Translation.Count() != r.Translation.Count()) return false;
            for (int i = 0; i < l.Translation.Count(); i++)
            {
                if (l.Translation.RefReadonlyAt(i) != r.Translation.RefReadonlyAt(i)) return false;
            }

            return true;
        }

        public static bool operator !=(in FrogTranslation l, in FrogTranslation r) => !(l == r);

        public override bool Equals(object other) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public static class ProtoPuffExtensions_FrogTranslation
    {
        public static void Clear(ref this FrogTranslation self)
        {
            self.TranslationIdHash = default;
            self.PluralForm = default;
            self.Translation.Clear();
        }

        public static int GetSerialisedSize(this in FrogTranslation self)
        {
            var result = 0;

            if (self.TranslationIdHash != default)
            {
                result += 2 + sizeof(uint);
            }

            if (self.PluralForm != default)
            {
                result += 2 + sizeof(byte);
            }

            if (self.Translation.Count() > 0)
            {
                var len = sizeof(byte) * self.Translation.Count();
                result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            }

            return result;
        }

        public static void SerialiseTo(this in FrogTranslation self, BinaryWriter bw)
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

        public static void Prepend(this BinaryWriter bw, in FrogTranslation data)
        {
            if (data.Translation.Count() > 0)
            {
                var posAfterField = bw.BaseStream.Position;
                for (int i = data.Translation.Count() - 1; i >= 0 ; i--)
                {
                    bw.Prepend(data.Translation.RefReadonlyAt(i));
                }
                var len = checked((int)(posAfterField - bw.BaseStream.Position));
                bw.PrependLenPrefix(len, out var lps);
                bw.Prepend(ValueQualifier.RepeatedU8(lps).Pack());
                bw.Prepend((byte)2);
            }

            if (data.PluralForm != default)
            {
                bw.Prepend(data.PluralForm);
                bw.Prepend(ValueQualifier.PackedU8);
                bw.Prepend((byte)1);
            }

            if (data.TranslationIdHash != default)
            {
                bw.Prepend(data.TranslationIdHash);
                bw.Prepend(ValueQualifier.PackedU32);
                bw.Prepend((byte)0);
            }

        }

        public static void DeserialiseFrom(this ref FrogTranslation self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.ReadLenPrefix(vq.LenPrefixSize) + br.BaseStream.Position;
            self.UpdateValueFrom(br, endPos);
        }

        public static void UpdateValueFrom(this ref FrogTranslation self, BinaryReader br, long endPos)
        {
            while (br.BaseStream.Position < endPos)
            {
                var fieldId = br.ReadByte();
                switch (fieldId)
                {
                    case 0:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureU32(fvq);
                        self.TranslationIdHash = br.ReadUInt32();
                        break;
                    }
                    case 1:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureU8(fvq);
                        self.PluralForm = br.ReadByte();
                        break;
                    }
                    case 2:
                    {
                        var fvq = ValueQualifier.Unpack(br.ReadByte());
                        ProtoPuffUtil.EnsureRepeatedU8(fvq);
                        var fieldEndPos = br.ReadLenPrefix(fvq.LenPrefixSize) + br.BaseStream.Position;
                        while (br.BaseStream.Position < fieldEndPos)
                        {
                            self.Translation.RefAdd() = br.ReadByte();
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

    [NoCopy]
    public struct FrogTranslations
    {
        public RefList<FrogTranslation> Entries;

        public static bool operator ==(in FrogTranslations l, in FrogTranslations r)
        {
            if (l.Entries.Count() != r.Entries.Count()) return false;
            for (int i = 0; i < l.Entries.Count(); i++)
            {
                if (l.Entries.RefReadonlyAt(i) != r.Entries.RefReadonlyAt(i)) return false;
            }

            return true;
        }

        public static bool operator !=(in FrogTranslations l, in FrogTranslations r) => !(l == r);

        public override bool Equals(object other) => throw new NotSupportedException();
        public override int GetHashCode() => throw new NotSupportedException();
    }

    public static class ProtoPuffExtensions_FrogTranslations
    {
        public static void Clear(ref this FrogTranslations self)
        {
            self.Entries.Clear();
        }

        public static int GetSerialisedSize(this in FrogTranslations self)
        {
            var result = 0;

            if (self.Entries.Count() > 0)
            {
                var len = 0;
                for (int i = 0; i < self.Entries.Count(); i++)
                {
                    var itemLen = self.Entries.RefReadonlyAt(i).GetSerialisedSize();
                    len += 1 + ProtoPuffUtil.GetLenPrefixSize(itemLen) + itemLen;
                }
                result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;
            }

            return result;
        }

        public static void SerialiseTo(this in FrogTranslations self, BinaryWriter bw)
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

        public static void Prepend(this BinaryWriter bw, in FrogTranslations data)
        {
            if (data.Entries.Count() > 0)
            {
                var posAfterField = bw.BaseStream.Position;
                for (int i = data.Entries.Count() - 1; i >= 0 ; i--)
                {
                    var posAfterItem = bw.BaseStream.Position;
                    bw.Prepend(data.Entries.RefReadonlyAt(i));
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

        public static void DeserialiseFrom(this ref FrogTranslations self, BinaryReader br)
        {
            var vq = ValueQualifier.Unpack(br.ReadByte());
            ProtoPuffUtil.EnsureStruct(vq);

            self.Clear();
            var endPos = br.ReadLenPrefix(vq.LenPrefixSize) + br.BaseStream.Position;
            self.UpdateValueFrom(br, endPos);
        }

        public static void UpdateValueFrom(this ref FrogTranslations self, BinaryReader br, long endPos)
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
                            self.Entries.RefAdd().UpdateValueFrom(br, itemEndPos);
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
