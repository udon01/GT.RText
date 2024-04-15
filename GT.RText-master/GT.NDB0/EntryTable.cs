using System;
using System.Collections.Generic;
using System.IO;
using GT.NDB0.Core.Structs;
using GT.Shared.Logging;

namespace GT.NDB0.Core {
    public class EntryTable
    {
        private readonly ILogWriter _logWriter;
        private readonly EndianBinReader _reader;
        private CategoryMetaData _entryHeaderMeta;
        private List<EntryMetaData> _entryMeta;

        public EntryTable(EndianBinReader reader, ILogWriter logWriter = null)
        {
            _reader = reader;
            _logWriter = logWriter;

            Read();
        }

        public List<EntryMetaData> Entries => _entryMeta;

        private void Read()
        {
            _entryHeaderMeta = new CategoryMetaData(_reader);

            _reader.BaseStream.Position += 8;
            _entryMeta = new List<EntryMetaData>();
            for (int i = 0; i < _entryHeaderMeta.EntryCount; i++)
            { 
                _entryMeta.Add(new EntryMetaData(_reader));
            }
        }
    }
}
