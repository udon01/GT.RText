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
    public class RT05Category : IRTextCategory
    {
        private readonly ILogWriter _logWriter;
        private readonly byte[] _data;
        private readonly int _categoryIndex;
        private RT05Header _header;
        private RT05CategoryMeta _categoryMeta;

        public string Name { get; set; }
        public List<(int Index, int Id, string Label, string Data)> Entries { get; set; }

        public RT05Category(RT05Header header, byte[] data, int categoryIndex, ILogWriter logWriter = null)
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
            using (var reader = new EndianBinReader(ms))
            {
                reader.BaseStream.Position = Constants.RT05_HEADER_SIZE + _categoryIndex * 0x10;
                _categoryMeta = new RT05CategoryMeta(reader);
            }
        }

        private void ReadCategoryName()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms))
            {
                reader.BaseStream.Position = _categoryMeta.TitleOffset;
                Name = reader.ReadNullTerminatedString();
            }
        }

        private void ReadEntries()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms))
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
            reader.BaseStream.Position = _categoryMeta.EntryOffset + (index * 0x10);
            var id = reader.ReadInt32();
            var labelLength = reader.ReadUInt16();
            var dataLength = reader.ReadUInt16();
            var labelOffset = reader.ReadUInt32();
            var dataOffset = reader.ReadUInt32();

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
                // TODO: Solve how the next xor key is derived
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

        public static uint BruteForceNextXorKey(byte[] data, uint currentXorKey, byte[] expectedData)
        {
            uint xorKey = 0;
            for (uint i = 0; i < 0xFFFFFFFF; i++)
            {
                var key = BitConverter.GetBytes(currentXorKey + i).Reverse().ToArray();
                byte[] buffer = new byte[data.Length];
                for (int j = 0; j < data.Length; j++)
                {
                    buffer[j] = (byte)(data[j] ^ key[j]);
                }

                if (buffer[0] == expectedData[0] && buffer[1] == expectedData[1] && buffer[2] == expectedData[2] &&
                    buffer[3] == expectedData[3])
                {
                    xorKey = currentXorKey + i;
                    goto keyFound;
                }
            }

            keyFound:
            return xorKey;
        }

        #region Saving
        private void Write(int baseOffset, EndianBinWriter writer)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
