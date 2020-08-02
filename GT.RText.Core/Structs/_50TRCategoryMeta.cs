namespace GT.RText.Core.Structs
{
    public struct _50TRCategoryMeta
    {
        public uint TitleOffset { get; set; }
        public uint Padding1 { get; set; } // Possible that it's not padding and is instead a long value
        public uint EntryCount { get; set; }
        public uint Padding2 { get; set; } // Possible that it's not padding and is instead a long value
        public uint EntryOffset { get; set; }

        public _50TRCategoryMeta(EndianBinReader reader)
        {
            TitleOffset = reader.ReadUInt32();
            Padding1 = reader.ReadUInt32();
            EntryCount = reader.ReadUInt32();
            Padding2 = reader.ReadUInt32();
            EntryOffset = reader.ReadUInt32();
        }
    }
}
