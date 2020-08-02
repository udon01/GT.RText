using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GT.RText.Core.Structs;
using GT.Shared;
using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    public class _50TRCategory : IRTextCategory
    {
        private readonly ILogWriter _logWriter;
        private readonly byte[] _data;
        private readonly int _categoryIndex;
        private _50TRHeader _header;
        private _50TRCategoryMeta _categoryMeta;

        public string Name { get; set; }
        public List<(int Index, int Id, string Label, string Data)> Entries { get; set; }

        public _50TRCategory(_50TRHeader header, byte[] data, int categoryIndex, ILogWriter logWriter = null)
        {
            _header = header;
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
            throw new NotImplementedException();
        }

        public void EditRow(int index, int id, string label, string data)
        {
            throw new NotImplementedException();
        }

        public int AddRow(int id, string label, string data)
        {
            throw new NotImplementedException();
        }

        public void DeleteRow(int index)
        {
            throw new NotImplementedException();
        }

        private void ReadCategoryMetaData()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms, EndianType.LITTLE_ENDIAN))
            {
                reader.BaseStream.Position = Constants._50TR_HEADER_SIZE + _categoryIndex * 0x18;
                _categoryMeta = new _50TRCategoryMeta(reader);
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

        private (int Index, int Id, string Label, string Data) ReadEntry(EndianBinReader reader, int index)
        {
            reader.BaseStream.Position = _categoryMeta.EntryOffset + (index * 0x18);
            var id = reader.ReadInt32();
            var labelLength = reader.ReadUInt16();
            var dataLength = reader.ReadUInt16();
            var labelOffset = reader.ReadUInt32();
            var padding1 = reader.ReadUInt32();
            var dataOffset = reader.ReadUInt32();
            var padding2 = reader.ReadUInt32();

            var label = ReadString(reader, labelOffset, labelLength);
            var data = ReadString(reader, dataOffset, dataLength);

            return (index, id, label, data);
        }

        private string ReadString(EndianBinReader reader, uint offset, ushort length)
        {
            reader.BaseStream.Position = offset;

            var buffer = reader.ReadBytes(length);

            if (_header.Obfuscated == 1)
            {
                // TODO: xor stuff is yet to be figured out. Brute forced X number of keys so some shorter strings are working.
                XorEncrypt(buffer);
            }
            
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        public static void XorEncrypt(byte[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                if (i >= Constants.XOR_KEYS.Length) break;
                data[i] = (byte)(data[i] ^ Constants.XOR_KEYS[i]);
            }
        }

        #region Saving
        private void Write(int baseOffset, EndianBinWriter writer)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
