using System;
using GT.Shared;
using GT.Shared.Polyphony;

namespace GT.RText.Core.Structs
{
    public struct RT05Header
    {
        public uint Magic { get; set; }
        public uint EntryCount { get; set; }
        public byte Obfuscated { get; set; }

        public RT05Header(EndianBinReader reader)
        {
            reader.BaseStream.Position = 0;
            if ((Magic = reader.ReadUInt32()) != Constants.RT05_MAGIC)
                throw new Exception("Invalid magic, doesn't match RT05.");

            EntryCount = reader.ReadUInt32();
            Obfuscated = reader.ReadByte();
        }

        public void Save(EndianBinWriter writer)
        {
            writer.Write(Magic);
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
