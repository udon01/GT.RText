using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    /// <summary>
    /// Gran Turismo 6 Localization Strings (Encryption)
    /// </summary>
    public class RT05 : RTextBase
    {
        public static readonly string Magic = "RT05";
        public const int HeaderSize = 0x20;

        private readonly ILogWriter _logWriter;

        public RT05(string filePath, ILogWriter logWriter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();


            _logWriter = logWriter;
        }

        public RT05(ILogWriter logWriter = null)
        {
            _logWriter = logWriter;
        }

        public void GetCategoryByIndex()
        {
            throw new NotImplementedException();
        }

        public void GetStringByCategory(int categoryIndex)
        {
            throw new NotImplementedException();
        }

        public override void Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new EndianBinReader(ms, EndianType.BIG_ENDIAN))
            {
                reader.BaseStream.Position = 0;
                if (reader.ReadString(4) != Magic)
                    throw new Exception($"Invalid magic, doesn't match {Magic}.");

                int entryCount = reader.ReadInt32();

                // Relocation ptr is at 0x10

                // Data starts at 0x20

                for (int i = 0; i < entryCount; i++)
                {
                    ms.Position = HeaderSize + (i * 0x10);
                    var page = new RT05Page(_logWriter);
                    page.Read(reader);
                    _pages.Add(page.Name, page);
                }
            }
        }

        public override void Save(string fileName)
        {
            using (var ms = new FileStream(fileName, FileMode.Create))
            using (var bs = new EndianBinWriter(ms, EndianType.BIG_ENDIAN))
            {
                bs.Write(Encoding.ASCII.GetBytes(Magic));
                bs.Write(_pages.Count);
                bs.Write((byte)1); // "Obfuscated", we don't know, eboot doesn't read it
                bs.BaseStream.Position = HeaderSize;

                int i = 0;
                int baseDataPos = HeaderSize + (_pages.Count * 0x10);
                foreach (var pagePair in _pages)
                {
                    int baseEntryOffset = (int)HeaderSize + (i * 0x10);
                    pagePair.Value.Write(bs, baseEntryOffset, baseDataPos);
                    baseDataPos = (int)ms.Position;
                    i++;
                }
            }
        }
    }
}
