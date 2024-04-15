namespace GT.RText.Core.Structs
{
    public struct RT04CategoryMeta
    {
        public uint TitleOffset { get; set; }
        public uint EntryCount { get; set; }
        public uint EntryOffset { get; set; }
        public uint Padding { get; set; } // Just junk data?

        public RT04CategoryMeta(EndianBinReader reader)
        {
            TitleOffset = reader.ReadUInt32();
            EntryCount = reader.ReadUInt32();
            EntryOffset = reader.ReadUInt32();
            Padding = reader.ReadUInt32();
        }
    }
}
