using System;
using GT.Shared;
using GT.Shared.Polyphony;

namespace GT.RText.Core.Structs
{
    public struct _50TRHeader
    {
        public uint Magic { get; set; }
        public uint EntryCount { get; set; }
        public byte Obfuscated { get; set; }

        public _50TRHeader(EndianBinReader reader)
        {
            reader.BaseStream.Position = 0;
            if ((Magic = reader.ReadUInt32(EndianType.BIG_ENDIAN)) != Constants._50TR_MAGIC)
                throw new Exception("Invalid magic, doesn't match 50TR.");

            EntryCount = reader.ReadUInt32();
            Obfuscated = reader.ReadByte();
        }

        public void Save(EndianBinWriter writer)
        {
            var magicBytes = BitConverter.GetBytes(Magic);
            Array.Reverse(magicBytes);
            writer.Write(magicBytes);
            writer.Write(EntryCount);
            writer.Write(Obfuscated);
            writer.Write(new byte[] { 0x00, 0x00, 0x00 });
            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }
    }
}
