using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GT.RText.Core.Structs;
using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    public class RT05 : IRText
    {
        private readonly ILogWriter _logWriter;
        private readonly byte[] _data;
        private RT05Header _header;

        private List<IRTextCategory> _categories { get; set; }

        public RT05(string filePath, ILogWriter logWriter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            _data = File.ReadAllBytes(filePath);
            _logWriter = logWriter;

            Read();
        }

        public RT05(byte[] data, ILogWriter logWriter = null)
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
            using (var reader = new EndianBinReader(ms))
            {
                _header = new RT05Header(reader);
            }
        }

        private void ReadCategories()
        {
            _categories = new List<IRTextCategory>();
            for (int i = 0; i < _header.EntryCount; i++)
            {
                var category = new RT05Category(_header, _data, i, _logWriter);
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
            using (var headerWriter = new EndianBinWriter(ms))
            using (var categoryMetaWriter = new EndianBinWriter(ms2))
            using (var entryWriter = new EndianBinWriter(ms3))
            {
                // Write the header
                _header.EntryCount = (uint)_categories.Count;
                _header.Save(headerWriter);

                // Write categories
                foreach (var category in _categories)
                {
                    // Write category title offset in the meta data
                    categoryMetaWriter.Write((uint)(headerWriter.BaseStream.Length + (_categories.Count * 0x10) + entryWriter.BaseStream.Length));
                    categoryMetaWriter.Write(category.Entries.Count);
                    categoryMetaWriter.Write(0x00000000);
                    entryWriter.WriteNullTerminatedString(category.Name, 4);
                    categoryMetaWriter.Write((int)(headerWriter.BaseStream.Length + (_categories.Count * 0x10) + entryWriter.BaseStream.Length));

                    category.Save((int)(headerWriter.BaseStream.Length + (_categories.Count * 0x10) + entryWriter.BaseStream.Length), entryWriter);
                }

                headerWriter.Write(ms2.ToArray());
                headerWriter.Write(ms3.ToArray());

                return ms.ToArray();
            }
        }
    }
}
