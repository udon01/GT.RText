using System;

namespace GT.NDB0.Core.Structs
{
    public struct CategoryMetaData
    {
        public uint Version { get; set; } // Not confirmed to be version but assuming it is
        public uint EntryCount { get; set; }

        public CategoryMetaData(EndianBinReader reader)
        {
            Version = reader.ReadUInt32();
            EntryCount = reader.ReadUInt32();
        }

        public void Save(EndianBinWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
