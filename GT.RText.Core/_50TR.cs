using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GT.RText.Core.Structs;
using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    public class _50TR : IRText
    {
        private readonly ILogWriter _logWriter;
        private readonly byte[] _data;
        private _50TRHeader _header;

        private List<IRTextCategory> _categories { get; set; }

        public _50TR(string filePath, ILogWriter logWriter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            _data = File.ReadAllBytes(filePath);
            _logWriter = logWriter;

            Read();
        }

        public _50TR(byte[] data, ILogWriter logWriter = null)
        {
            _data = data;
            _logWriter = logWriter;

            Read();
        }

        public List<IRTextCategory> GetCategories()
        {
            return _categories;
        }

        private void Read()
        {
            ReadHeader();
            ReadCategories();
        }

        private void ReadHeader()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms, EndianType.LITTLE_ENDIAN))
            {
                _header = new _50TRHeader(reader);
            }
        }

        private void ReadCategories()
        {
            _categories = new List<IRTextCategory>();
            for (int i = 0; i < _header.EntryCount; i++)
            {
                var category = new _50TRCategory(_header, _data, i, _logWriter);
                category.Read();
                _categories.Add(category);
            }
        }

        public void Save(string filePath)
        {
            File.WriteAllBytes(filePath, Write());
        }

        private byte[] Write()
        {
            using (var ms = new MemoryStream())
            using (var ms2 = new MemoryStream())
            using (var ms3 = new MemoryStream())
            using (var headerWriter = new EndianBinWriter(ms, EndianType.LITTLE_ENDIAN))
            using (var categoryMetaWriter = new EndianBinWriter(ms2, EndianType.LITTLE_ENDIAN))
            using (var entryWriter = new EndianBinWriter(ms3, EndianType.LITTLE_ENDIAN))
            {
                // Write the header
                _header.EntryCount = (uint)_categories.Count;
                _header.Save(headerWriter);

                // Write categories
                foreach (var category in _categories)
                {
                    // Write category title offset in the meta data
                    categoryMetaWriter.Write((uint)(headerWriter.BaseStream.Length + (_categories.Count * 0x18) + entryWriter.BaseStream.Length));
                    categoryMetaWriter.Write(0x00000000);
                    categoryMetaWriter.Write(category.Entries.Count);
                    categoryMetaWriter.Write(0x00000000);
                    entryWriter.WriteNullTerminatedString(category.Name, 4);
                    categoryMetaWriter.Write((int)(headerWriter.BaseStream.Length + (_categories.Count * 0x18) + entryWriter.BaseStream.Length));
                    categoryMetaWriter.Write(0x00000000);

                    category.Save((int)(headerWriter.BaseStream.Length + (_categories.Count * 0x18) + entryWriter.BaseStream.Length), entryWriter);
                }

                headerWriter.Write(ms2.ToArray());
                headerWriter.Write(ms3.ToArray());

                return ms.ToArray();
            }
        }
    }
}
