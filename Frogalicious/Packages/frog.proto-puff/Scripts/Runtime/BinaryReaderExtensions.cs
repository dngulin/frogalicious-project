using System;
using System.IO;

namespace Frog.ProtoPuff
{
    public static class BinaryReaderExtensions
    {
        public static int ReadLenPrefix(this BinaryReader br, LenPrefixSize lps)
        {
            return lps switch
            {
                LenPrefixSize._0 => 0,
                LenPrefixSize._8 => br.ReadByte(),
                LenPrefixSize._16 => br.ReadUInt16(),
                LenPrefixSize._32 => checked((int)br.ReadUInt32()),
                _ => throw new InvalidOperationException()
            };
        }

        public static void SkipValue(this BinaryReader br, ValueQualifier q)
        {
            var len = q.Kind switch
            {
                ValueKind.Primitive => q.PrimitiveSize.InBytes(),
                _ => br.ReadLenPrefix(q.LenPrefixSize),
            };

            br.BaseStream.Position += len;
        }
    }
}