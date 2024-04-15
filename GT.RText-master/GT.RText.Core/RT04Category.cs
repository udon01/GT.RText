using System.Collections.Generic;
using System.IO;
using GT.RText.Core.Structs;
using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    public class RT04Category : IRTextCategory
    {
        private readonly ILogWriter _logWriter;
        private readonly byte[] _data;
        private readonly int _categoryIndex;
        private RT04CategoryMeta _categoryMeta;

        public string Name { get; set; }
        public List<(int Index, int Id, string Label, string Data)> Entries { get; set; }

        public RT04Category(byte[] data, int categoryIndex, ILogWriter logWriter = null)
        {
            _data = data;
            _categoryIndex = categoryIndex;
            _logWriter = logWriter;
        }

        public void Read()
        {
            ReadCategoryMetaData();
            ReadCategoryName();
            ReadEntries();
        }

        public void Save(int baseOffset, EndianBinWriter writer)
        {
            Write(baseOffset, writer);
        }

        public void EditRow(int index, int id, string label, string data)
        {
            if (index < 0 || index > Entries.Count - 1) return;

            Entries[index] = (index, id, label, data);
        }

        public int AddRow(int id, string label, string data)
        {
            var index = Entries.Count;
            Entries.Add((index, id, label, data));
            return index;
        }

        public void DeleteRow(int index)
        {
            Entries.RemoveAt(index);
            Entries.ForEach(x =>
            {
                if (x.Index > index)
                    index--;
            });
        }

        private void ReadCategoryMetaData()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms, EndianType.LITTLE_ENDIAN))
            {
                reader.BaseStream.Position = (_categoryIndex + 1) * 0x10;
                _categoryMeta = new RT04CategoryMeta(reader);
            }
        }

        private void ReadCategoryName()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms, EndianType.LITTLE_ENDIAN))
            {
                reader.BaseStream.Position = _categoryMeta.TitleOffset;
                Name = reader.ReadNullTerminatedString();
            }
        }

        private void ReadEntries()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms, EndianType.LITTLE_ENDIAN))
            {
                Entries = new List<(int index, int Id, string Label, string Data)>();
                for (var i = 0; i < _categoryMeta.EntryCount; i++)
                {
                    Entries.Add(ReadEntry(reader, i));
                }
            }
        }

        private (int index, int Id, string Label, string Data) ReadEntry(EndianBinReader reader, int index)
        {
            reader.BaseStream.Position = _categoryMeta.EntryOffset + (index * 0x0C);
            var id = reader.ReadInt32();
            var labelOffset = reader.ReadUInt32();
            var dataOffset = reader.ReadUInt32();


            reader.BaseStream.Position = labelOffset;
            var label = reader.ReadNullTerminatedString();
            reader.BaseStream.Position = dataOffset;
            var data = reader.ReadNullTerminatedString();

            return (index, id, label, data);
        }

        #region Saving
        private void Write(int baseOffset, EndianBinWriter writer)
        {
            using (var ms = new MemoryStream())
            using (var dataWriter = new EndianBinWriter(ms, EndianType.LITTLE_ENDIAN))
            {
                // Write categories
                foreach (var entry in Entries)
                {
                    writer.Write(entry.Id);
                    writer.Write((uint)(baseOffset + (Entries.Count * 0x0C) + dataWriter.BaseStream.Length));
                    dataWriter.WriteNullTerminatedString(entry.Label, 4);
                    writer.Write((uint)(baseOffset + (Entries.Count * 0x0C) + dataWriter.BaseStream.Length));
                    dataWriter.WriteNullTerminatedString(entry.Data, 4);
                }

                writer.Write(ms.ToArray());
            }
        }
        #endregion
    }
}
