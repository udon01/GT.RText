using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;
using GT.Shared;
using GT.Shared.Crypt;
using GT.Shared.Logging;

namespace GT.RText.Core
{
    public class RT05Page : RTextPageBase
    {
        private readonly ILogWriter _logWriter;
        public const int EntrySize = 0x10;

        public RT05Page(ILogWriter logWriter = null)
        {
            _logWriter = logWriter;
        }

        public override void Read(EndianBinReader reader)
        {
            var pageNameOffset = reader.ReadUInt32();
            var pairUnitCount = reader.ReadUInt32();
            reader.ReadUInt32(); // Unk
            var pairUnitOffset = reader.ReadUInt32();

            reader.BaseStream.Position = (int)pageNameOffset;
            Name = reader.ReadNullTerminatedString();

            for (int i = 0; i < pairUnitCount; i++)
            {
                reader.BaseStream.Position = pairUnitOffset + (i * EntrySize);
                int id = reader.ReadInt32();

                ushort labelLen = reader.ReadUInt16();
                ushort valueLen = reader.ReadUInt16();

                uint labelOffset = reader.ReadUInt32();
                uint valueOffset = reader.ReadUInt32();

                reader.BaseStream.Position = labelOffset;
                string label = ReadString(reader, labelLen);

                reader.BaseStream.Position = valueOffset;
                string value = ReadString(reader, valueLen);

                var pair = new RTextPairUnit(id, label, value);
                PairUnits.Add(label, pair);
            }
        }

        public override void Write(EndianBinWriter writer, int baseOffset, int baseDataOffset)
        {
            writer.BaseStream.Position = baseDataOffset;
            int baseNameOffset = (int)writer.BaseStream.Position;
            writer.WriteNullTerminatedString(Name, 0x04);
            int pairUnitOffset = (int)writer.BaseStream.Position;

            // Proceed to write the string tree, skip the entry map for now
            writer.BaseStream.Position += EntrySize * PairUnits.Count;
            int lastStringPos = (int)writer.BaseStream.Position;

            // Write our strings
            int j = 0;

            // Setup for binary searching - strings are sorted by length, then their encrypted values
            // Override the sorting logic - couldn't find a better way to structure this to make it work across all versions
            // Not the most efficient
            var orderedPairs = PairUnits.Values.OrderBy(
                e => Encrypt(Encoding.UTF8.GetBytes(e.Label), Constants.KEY.AlignString(0x20)),
                ByteBufferComparer.Default);

            foreach (var pair in orderedPairs)
            {
                writer.BaseStream.Position = lastStringPos;

                int labelOffset = (int)writer.BaseStream.Position;
                var encLabel = Encrypt(Encoding.UTF8.GetBytes(pair.Label), Constants.KEY.AlignString(0x20));
                writer.WriteAligned(encLabel, 4, nullTerminate: true);

                int valueOffset = (int)writer.BaseStream.Position;
                var encValue = Encrypt(Encoding.UTF8.GetBytes(pair.Value), Constants.KEY.AlignString(0x20));
                writer.WriteAligned(encValue, 4, nullTerminate: true);

                lastStringPos = (int)writer.BaseStream.Position;

                // Write the offsets
                writer.BaseStream.Position = pairUnitOffset + (j * EntrySize);
                writer.Write(pair.ID);
                writer.Write((ushort)(encLabel.Length + 1));
                writer.Write((ushort)(encValue.Length + 1));
                writer.Write(labelOffset);
                writer.Write(valueOffset);

                j++;
            }

            // Finish page toc entry
            writer.BaseStream.Position = baseOffset;
            writer.Write(baseNameOffset);
            writer.Write(PairUnits.Count);
            writer.Write(0); // Unk
            writer.Write(pairUnitOffset);

            // Seek to the end of it
            writer.BaseStream.Position = writer.BaseStream.Length;
        }

        private string ReadString(EndianBinReader reader, ushort length)
        {
            var buffer = reader.ReadBytes(length - 1);

            /* Haven't seen any evidence in the gt6 eboot upon getting a string that this corresponds to a rtext being encrypted */
            if (buffer.Length > 0)
                buffer = Decrypt(buffer, Constants.KEY.AlignString(0x20));
            return Encoding.UTF8.GetString(buffer);
        }

        static byte[] Decrypt(byte[] encrypted, string key)
        {
            using (SymmetricAlgorithm salsa20 = new Salsa20())
            {
                var dataKey = new byte[8];
                var keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] decrypted = new byte[encrypted.Length];
                using (var decrypt = salsa20.CreateDecryptor(keyBytes, dataKey))
                    decrypt.TransformBlock(encrypted, 0, encrypted.Length, decrypted, 0);

                return decrypted;
            }
        }

        static byte[] Encrypt(byte[] input, string key)
        {
            using (SymmetricAlgorithm salsa20 = new Salsa20())
            {
                var dataKey = new byte[8];
                var keyBytes = Encoding.UTF8.GetBytes(key);

                byte[] encrypted = new byte[input.Length];
                using (var encrypt = salsa20.CreateEncryptor(keyBytes, dataKey))
                    encrypt.TransformBlock(input, 0, input.Length, encrypted, 0);

                return encrypted;
            }
        }
    }
}
