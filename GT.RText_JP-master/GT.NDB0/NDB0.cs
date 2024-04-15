using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GT.NDB0.Core.Structs;
using GT.Shared;
using GT.Shared.Logging;

namespace GT.NDB0.Core
{
    public class NDB0
    {
        private readonly ILogWriter _logWriter;
        private readonly byte[] _data;
        private Header _header;
        private List<EntryTable> _entryTables;
        private List<string> _speedUpTable;

        public NDB0(string filePath, ILogWriter logWriter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            _data = File.ReadAllBytes(filePath);
            _logWriter = logWriter;

            Read();
        }

        public NDB0(byte[] data, ILogWriter logWriter = null)
        {
            _data = data;
            _logWriter = logWriter;

            Read();
        }

        private void Read()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms))
            {
                _header = new Header(reader);

                _entryTables = new List<EntryTable>();
                for (int i = 0; i < _header.EntryCount; i++)
                {
                    reader.BaseStream.Position = Constants.NDB0_HEADER_SIZE + (i * 0x04);
                    reader.BaseStream.Position = reader.ReadUInt32();
                    _entryTables.Add(new EntryTable(reader, _logWriter));
                }

#if DEBUG
                for (int i = 0; i < _entryTables.Count; i++)
                {
                    var entryTable = _entryTables[i];
                    _logWriter?.WriteLine($"---- Entry Table {i} ({entryTable.Entries.Count}) ----");
                    for (int j = 0; j < entryTable.Entries.Count; j++)
                    {
                        var entryTableEntry = entryTable.Entries[j];

                        _logWriter?.WriteLine($"{j:X8} | {entryTableEntry.CarID:X8} - {entryTableEntry.OrderID:X8} - {entryTableEntry.DataOffset:X8} - {entryTableEntry.Lookup1:X2} - {entryTableEntry.Lookup2:X2} - {entryTableEntry.Lookup3:X2} - {entryTableEntry.Lookup4:X2} - {entryTableEntry.Data}");
                    }
                }
#endif

                _speedUpTable = new List<string>();
                reader.BaseStream.Position = _header.SpeedUpTableOffset;
                var count = reader.ReadUInt32();
                for (int i = 0; i < count; i++)
                {
                    reader.BaseStream.Position = _header.SpeedUpTableOffset + 0x04 + (i * 0x04);
                    reader.BaseStream.Position = reader.ReadUInt32();
                    _speedUpTable.Add(reader.ReadNullTerminatedString());
                }

#if DEBUG
                _logWriter?.WriteLine($"---- Speed Up Table ({_speedUpTable.Count}) ----");
                for (int i = 0; i < _speedUpTable.Count; i++)
                {
                    _logWriter?.WriteLine($"{i:X8} | {_speedUpTable[i]}");
                }


                var usageCount = new int[0xFF];
                for (int i = 0; i < _entryTables.Count; i++)
                {
                    var entryTable = _entryTables[i];
                    foreach (var entryTableEntry in entryTable.Entries.OrderBy(x => x.OrderID))
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (entryTableEntry.LookupTableOffsets[k] < 0xFF)
                                usageCount[entryTableEntry.LookupTableOffsets[k]]++;
                        }
                    }
                }
#endif


                for (int i = 0; i < _entryTables.Count; i++)
                {
                    using (var sw = new StreamWriter($"NDB0_Table_{i}.txt"))
                    {
                        var entryTable = _entryTables[i];
                        foreach (var entryTableEntry in entryTable.Entries.OrderBy(x => x.OrderID))
                        {
                            var sb = new StringBuilder();

                            for (int k = 0; k < 4; k++)
                            {
                                if (entryTableEntry.LookupTableOffsets[k] < 0xFF)
                                    sb.Append($"{_speedUpTable[entryTableEntry.LookupTableOffsets[k]]} ");
                            }

                            sb.Append(entryTableEntry.Data);
                            sw.WriteLine($"{entryTableEntry.CarID:X8} - {entryTableEntry.OrderID:X8} - {sb}");
                        }
                    }
                }
            }
        }
    }
}
