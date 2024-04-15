namespace GT.RText.Core.Structs
{
    public struct RT05CategoryMeta
    {
        public uint TitleOffset { get; set; }
        public uint EntryCount { get; set; }
        public uint Padding { get; set; }
        public uint EntryOffset { get; set; }

        public RT05CategoryMeta(EndianBinReader reader)
        {
            TitleOffset = reader.ReadUInt32();
            EntryCount = reader.ReadUInt32();
            Padding = reader.ReadUInt32();
            EntryOffset = reader.ReadUInt32();
        }
    }
}
