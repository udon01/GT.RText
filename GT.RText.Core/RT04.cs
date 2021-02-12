using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    /// <summary>
    /// Gran Turismo 5 Localization Strings
    /// </summary>
    public class RT04 : RTextBase
    {
        public static readonly string Magic = "RT04";
        public const int HeaderSize = 0x10;

        private readonly ILogWriter _logWriter;


        public RT04(string filePath, ILogWriter logWriter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();


            _logWriter = logWriter;
        }

        public RT04(ILogWriter logWriter = null)
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
            using (var reader = new EndianBinReader(ms, EndianType.LITTLE_ENDIAN))
            {
                reader.BaseStream.Position = 0;
                if (reader.ReadString(4) != Magic)
                    throw new Exception($"Invalid magic, doesn't match {Magic}.");

                reader.ReadInt32(); // Relocation Ptr
                reader.ReadUInt32(); // Empty
                int entryCount = reader.ReadInt32();

                for (int i = 0; i < entryCount; i++)
                {
                    ms.Position = HeaderSize + (i * 0x10);
                    var page = new RT04Page(_logWriter);
                    page.Read(reader);
                    _pages.Add(page.Name, page);
                }
            }
        }

        public override void Save(string fileName)
        {
            using (var ms = new FileStream(fileName, FileMode.Create))
            using (var bs = new EndianBinWriter(ms, EndianType.LITTLE_ENDIAN))
            {
                bs.Write(Encoding.ASCII.GetBytes(Magic));
                bs.Write(0);
                bs.Write(0);
                bs.Write(_pages.Count);

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
