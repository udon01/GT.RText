using System;

namespace GT.NDB0.Core.Structs
{
    public struct EntryMetaData
    {
        public uint CarID { get; set; }
        public uint OrderID { get; set; }
        public uint DataOffset { get; set; }
        public byte[] LookupTableOffsets { get; set; }
        public byte Lookup1 { get; set; }
        public byte Lookup2 { get; set; }
        public byte Lookup3 { get; set; }
        public byte Lookup4 { get; set; }

        public string Data { get; set; }

        public EntryMetaData(EndianBinReader reader)
        {
            CarID = reader.ReadUInt32();
            OrderID = reader.ReadUInt32();
            DataOffset = reader.ReadUInt32();
            LookupTableOffsets = reader.ReadBytes(4);
            Lookup1 = LookupTableOffsets[0];
            Lookup2 = LookupTableOffsets[1];
            Lookup3 = LookupTableOffsets[2];
            Lookup4 = LookupTableOffsets[3];

            var saveOffset = reader.BaseStream.Position;
            reader.BaseStream.Position = DataOffset;
            Data = reader.ReadNullTerminatedString();
            reader.BaseStream.Position = saveOffset;
        }

        public void Save(EndianBinWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
