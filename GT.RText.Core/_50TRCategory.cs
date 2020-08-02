using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GT.RText.Core.Exceptions;
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
            Write(baseOffset, writer);
        }

        public void EditRow(int index, int id, string label, string data)
        {
            CheckStringLength(label);
            CheckStringLength(data);

            if (index < 0 || index > Entries.Count - 1) return;

            Entries[index] = (index, id, label, data);
        }

        public int AddRow(int id, string label, string data)
        {
            CheckStringLength(label);
            CheckStringLength(data);

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
                if (i >= Constants.XOR_KEYS.Length) throw new XorKeyTooShortException("String outside of range for known xor keys.");

                data[i] = (byte)(data[i] ^ Constants.XOR_KEYS[i]);
            }
        }

        private void CheckStringLength(string stringData)
        {
            if (Encoding.UTF8.GetBytes($"{stringData}\0").Length > Constants.XOR_KEYS.Length)
                throw new ArgumentOutOfRangeException($"String {stringData.Take(10)}... is too long to be saved with obfuscation.");
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
                    var label = Encoding.UTF8.GetBytes($"{entry.Label}\0");
                    var data = Encoding.UTF8.GetBytes($"{entry.Data}\0");
                    if (_header.Obfuscated == 1)
                    {
                        XorEncrypt(label);
                        XorEncrypt(data);
                    }

                    writer.Write((ushort)label.Length);
                    writer.Write((ushort)data.Length);

                    writer.Write((uint)(baseOffset + (Entries.Count * 0x18) + dataWriter.BaseStream.Length));
                    writer.Write(0x00000000);
                    dataWriter.WriteAligned(label, 4);
                    writer.Write((uint)(baseOffset + (Entries.Count * 0x18) + dataWriter.BaseStream.Length));
                    writer.Write(0x00000000);
                    dataWriter.WriteAligned(data, 4);
                }

                writer.Write(ms.ToArray());
            }
        }
        #endregion
    }
}
