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
            throw new NotImplementedException();
        }
    }
}
