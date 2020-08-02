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
            throw new NotImplementedException();
        }
    }
}
