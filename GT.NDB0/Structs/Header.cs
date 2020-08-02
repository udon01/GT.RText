using System;
using GT.Shared;

namespace GT.NDB0.Core.Structs
{
    public struct Header
    {
        public uint Magic { get; set; }
        public uint Padding { get; set; }
        public uint SpeedUpTableOffset { get; set; }
        public uint EntryCount { get; set; }

        public Header(EndianBinReader reader)
        {
            reader.BaseStream.Position = 0;
            if ((Magic = reader.ReadUInt32()) != Constants.NDB0_MAGIC)
                throw new Exception("Invalid magic, doesn't match NDB0.");

            Padding = reader.ReadUInt32();
            SpeedUpTableOffset = reader.ReadUInt32();
            EntryCount = reader.ReadUInt32();
        }

        public void Save(EndianBinWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
